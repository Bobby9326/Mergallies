using Photon.Pun;
using Photon.Realtime;  
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player2Name;

    public Image player1Image;  // รูปภาพของผู้เล่นคนที่ 1
    public Image player2Image;  // รูปภาพของผู้เล่นคนที่ 2
    public Button startButton;  // ปุ่มเริ่มเกม
    public Button leaveButton;  // ปุ่มออกจากห้อง
    private bool isLeavingRoom = false;  // ตัวแปรเพื่อป้องกันการออกจากห้องซ้ำ
    public Image player1Target;  // รูปภาพของเจ้าของห้อง (Master Client)
    public Image player2Target;  // รูปภาพของผู้เล่นคนอื่น

    void Start()
    {
        Debug.Log("เริ่มต้น LobbyManager");

        // ลองใช้การค้นหาวัตถุ TextMeshPro หากไม่ได้ถูกเชื่อมโยงใน Inspector
    

        PhotonNetwork.AutomaticallySyncScene = true;

        if (leaveButton != null)
        {
            leaveButton.onClick.AddListener(LeaveLobby);
        }
        if (roomName != null && PhotonNetwork.CurrentRoom != null)
        {
            roomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        }
    }




    void Update()
    {
        // Debug สถานะของ Photon เพื่อดูว่ามีปัญหาการเชื่อมต่อหรือไม่
        Debug.Log("สถานะของ Photon: " + PhotonNetwork.NetworkClientState);
        
        UpdatePlayerNames();


        if (PhotonNetwork.CurrentRoom != null)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            // แสดงรูปภาพตามจำนวนผู้เล่น
            if (playerCount == 1)
            {
                player1Image.gameObject.SetActive(true);
                player2Image.gameObject.SetActive(false);
                player1Target.gameObject.SetActive(true);
                player2Target.gameObject.SetActive(false);
            }
            else if (playerCount == 2)
            {
                player1Image.gameObject.SetActive(true);
                player2Image.gameObject.SetActive(true);
                if (PhotonNetwork.IsMasterClient)
                {
                    // ถ้าเป็นเจ้าของห้อง แสดงรูป 1 และซ่อนรูป 2
                    player1Target.gameObject.SetActive(true);
                    player2Target.gameObject.SetActive(false);
                }
                else
                {
                    // ถ้าไม่ใช่เจ้าของห้อง ซ่อนรูป 1 และแสดงรูป 2
                    player1Target.gameObject.SetActive(false);
                    player2Target.gameObject.SetActive(true);
                }
            }
            else
            {
                player1Image.gameObject.SetActive(false);
                player2Image.gameObject.SetActive(false);
                player1Target.gameObject.SetActive(false);
                player2Target.gameObject.SetActive(false);
            }

            // ปุ่ม start จะเปิดใช้งานเมื่อมีผู้เล่น 2 คน
            startButton.interactable = playerCount == 2;
        }
        else
        {
            Debug.LogWarning("ยังไม่มีการสร้างห้องหรือเข้าร่วมห้อง");
        }
        

    }

    void UpdatePlayerNames()
    {
        // ถ้ามีผู้เล่น 1 คน
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            player1Name.text = PhotonNetwork.PlayerList[0].NickName;
            player2Name.text = PhotonNetwork.PlayerList[1].NickName;
        }
        else if (PhotonNetwork.PlayerList.Length > 0)
        {
            player1Name.text = PhotonNetwork.PlayerList[0].NickName;
            player2Name.text = "";
        }
        else
        {
            player1Name.text = "";
            player2Name.text = "";
        }
    }

    // ฟังก์ชันออกจากห้อง
    public void LeaveLobby()
    {
        // ตรวจสอบว่าผู้เล่นอยู่ในห้องและยังไม่ได้อยู่ในกระบวนการออกจากห้อง
        if (PhotonNetwork.InRoom && !isLeavingRoom)
        {
            leaveButton.interactable = false; // ปิดการใช้งานปุ่มออกจากห้องชั่วคราว
            isLeavingRoom = true;  // ตั้งสถานะว่าอยู่ในกระบวนการออกจากห้อง
            PhotonNetwork.LeaveRoom();  // ออกจากห้อง
            Debug.LogError("สถานะของ Photon: " + PhotonNetwork.NetworkClientState);
        }
        else
        {
            Debug.LogError("ยังไม่สามารถออกจากห้องได้ในขณะนี้ เนื่องจากสถานะของ Photon: " + PhotonNetwork.NetworkClientState);
        }
    }

    // Callback เมื่อออกจากห้องสำเร็จ
    public override void OnLeftRoom()
    {
        Debug.Log("ออกจากห้องสำเร็จ");
        isLeavingRoom = false;  // รีเซ็ตสถานะเมื่อออกจากห้องเสร็จแล้ว
        leaveButton.interactable = true;  // เปิดใช้งานปุ่มอีกครั้ง
        PhotonNetwork.LoadLevel("MainMenuScene"); // กลับไปที่หน้า Main Menu
    }

    // ฟังก์ชันเริ่มเกม
    public void StartGame()
    {
        // ตรวจสอบว่าเป็นผู้เล่นที่เป็น Master Client
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("เริ่มเกมโดย Master Client");
            PhotonNetwork.LoadLevel("StoryScene"); // ทุกคนในห้องจะถูกซิงโครไนซ์ไปยัง Scene "Level1TutorialScene"
        }
        else
        {
            Debug.LogWarning("เฉพาะ Master Client เท่านั้นที่สามารถเริ่มเกมได้");
        }
    }

    // Callback เมื่อ Master Client เปลี่ยนไป
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log("Master Client ถูกเปลี่ยนเป็น: " + newMasterClient.NickName);
    }


}
