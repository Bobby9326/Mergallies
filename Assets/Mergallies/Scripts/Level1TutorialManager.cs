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
    public Button MapButton; 
    public Button ExitMapButton; 
    public Button HintButton; 
    public Button ExitHintButton; 
    public Image Map;
    public Image Hint;
    public Image HammerIMG;
    public Image FanIMG;
    public Image TorchIMG;
    public Image BotleIMG;
    
    private bool findHammer = false; 
    private bool findFan = false; 
    private bool findTorch = false; 
    private bool findBottle = false; 
    private bool isSceneLoading = false; 

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        UpdatePlayerCount();
        HammerIMG.color = new Color(0f, 0f, 0f, 1f); 
        FanIMG.color = new Color(0f, 0f, 0f, 1f); 
        TorchIMG.color = new Color(0f, 0f, 0f, 1f); 
        BotleIMG.color = new Color(0f, 0f, 0f, 1f); 

        MapButton.onClick.AddListener(OpenMap);
        ExitMapButton.onClick.AddListener(CloseMap);
        HintButton.onClick.AddListener(OpenHint);
        ExitHintButton.onClick.AddListener(CloseHint);

        ExitMapButton.gameObject.SetActive(false);
        Map.gameObject.SetActive(false);
        ExitHintButton.gameObject.SetActive(false);
        Hint.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdatePlayerCount();
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
    public void OpenMap()
    {
        ExitMapButton.gameObject.SetActive(true);
        Map.gameObject.SetActive(true);
        MapButton.interactable = false;
    }

    // ฟังก์ชันปิดแผนที่สำหรับผู้เล่นที่กดปุ่ม
    public void CloseMap()
    {
        ExitMapButton.gameObject.SetActive(false);
        Map.gameObject.SetActive(false);
        MapButton.interactable = true;
    }
    public void OpenHint()
    {
        ExitHintButton.gameObject.SetActive(true);
        Hint.gameObject.SetActive(true);
        HintButton.interactable = false;
    }

    // ฟังก์ชันปิดแผนที่สำหรับผู้เล่นที่กดปุ่ม
    public void CloseHint()
    {
        ExitHintButton.gameObject.SetActive(false);
        Hint.gameObject.SetActive(false);
        HintButton.interactable = true;
    }

    public void FindHammer()
    {
        findHammer = true;
        HammerIMG.color = new Color(1f, 1f, 1f, 1f); 

    }

    public void FindFan()
    {
        findFan = true;
        FanIMG.color = new Color(1f, 1f, 1f, 1f); 

    }

    public void FindTorch()
    {
        findTorch = true;
        TorchIMG.color = new Color(1f, 1f, 1f, 1f); 

    }

    public void FindBottle()
    {
        findBottle = true;
        BotleIMG.color = new Color(1f, 1f, 1f, 1f); 

    }

    public void GoFinish()
    {
        Debug.Log("พบ Spawn");
        if (findHammer && findFan && findTorch && findBottle)
        {
            

            // ตรวจสอบว่าเป็น MasterClient หรือไม่
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GoToResultSceneRPC", RpcTarget.All); // เรียกชื่อฟังก์ชัน RPC ที่ถูกต้อง
            }
        }
    }

    // ฟังก์ชันตรวจสอบว่าพบของครบ 4 ชิ้นหรือยัง
    void CheckIfAllItemsFound()
    {
        if (findHammer && findFan && findTorch && findBottle)
        {
            Debug.Log("พบของครบทั้ง 4 ชิ้นแล้ว!");

            // ตรวจสอบว่าเป็น MasterClient หรือไม่
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("GoToResultSceneRPC", RpcTarget.All); // เรียกชื่อฟังก์ชัน RPC ที่ถูกต้อง
            }
        }
    }


    [PunRPC] // ทำเครื่องหมายฟังก์ชันนี้ว่าเป็น RPC
    void GoToResultSceneRPC()
    {
        GoToResultScene(); // เรียกฟังก์ชันสำหรับโหลดฉาก
    }


    void GoToResultScene()
    {
        // ตรวจสอบว่าฉากกำลังโหลดหรือไม่เพื่อหลีกเลี่ยงการโหลดซ้ำ
        if (!isSceneLoading)
        {
            isSceneLoading = true;  // ตั้งสถานะว่ากำลังโหลดซีน
            PhotonNetwork.LoadLevel("ResultScene"); // โหลดฉาก ResultScene
        }
    }



}
