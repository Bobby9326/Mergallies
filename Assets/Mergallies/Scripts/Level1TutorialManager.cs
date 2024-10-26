using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level1TutorialManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerCountText;
    public GameObject playerPrefab;
    public Image HammerIMG;
    public Image FanIMG;
    public Image TorchIMG;
    public Image BotleIMG;
    
    private bool findHammer = false; 
    private bool findFan = false; 
    private bool findTorch = false; 
    private bool findBottle = false; 

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        UpdatePlayerCount();
        HammerIMG.color = new Color(0f, 0f, 0f, 1f); 
        FanIMG.color = new Color(0f, 0f, 0f, 1f); 
        TorchIMG.color = new Color(0f, 0f, 0f, 1f); 
        BotleIMG.color = new Color(0f, 0f, 0f, 1f); 
    }

    void Update()
    {
        UpdatePlayerCount();
        CheckIfAllItemsFound(); // ตรวจสอบว่าพบของครบแล้วหรือยัง
    }

    void UpdatePlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            playerCountText.text = "Players in Room: " + playerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        }
        else
        {
            playerCountText.text = "Not in a room";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " เข้าร่วมห้อง");
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " ออกจากห้อง");
        UpdatePlayerCount();

        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            LeaveRoomAndGoToMainMenu();
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    void LeaveRoomAndGoToMainMenu()
    {
        Debug.Log("กำลังออกจากห้องและกลับไปหน้า Main Menu...");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenuScene");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void FindHammer()
    {
        findHammer = true;
        HammerIMG.color = new Color(1f, 1f, 1f, 1f); 
        CheckIfAllItemsFound();  // ตรวจสอบทุกครั้งเมื่อพบไอเทมใหม่
    }

    public void FindFan()
    {
        findFan = true;
        FanIMG.color = new Color(1f, 1f, 1f, 1f); 
        CheckIfAllItemsFound();
    }

    public void FindTorch()
    {
        findTorch = true;
        TorchIMG.color = new Color(1f, 1f, 1f, 1f); 
        CheckIfAllItemsFound();
    }

    public void FindBottle()
    {
        findBottle = true;
        BotleIMG.color = new Color(1f, 1f, 1f, 1f); 
        CheckIfAllItemsFound();
    }

    // ฟังก์ชันตรวจสอบว่าพบของครบ 4 ชิ้นหรือยัง
    void CheckIfAllItemsFound()
    {
        if (findHammer && findFan && findTorch && findBottle)
        {
            Debug.Log("พบของครบทั้ง 4 ชิ้นแล้ว!");
            PhotonNetwork.LoadLevel("ResultScene"); // เปลี่ยนไปยังหน้าจบเกม
        }
    }
}
