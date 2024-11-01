using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class Level1TutorialManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI timeText;
    public GameObject playerPrefab;
    public Button LeaveButton; 
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
    public PlayerController playerManager;
    public GameObject Rock1;
    public GameObject Rock2;
    public GameObject Bridge;
    public GameObject WrongBridge;
    public GameObject HorizonBridge;
    public GameObject HorizonWrongBridge;
    
    private bool findHammer = false; 
    private bool findFan = false; 
    private bool findTorch = false; 
    private bool findBottle = false; 
    private bool isSceneLoading = false; 
    private bool isFinish = false; 
    private Vector3 rock1Position;
    private Vector3 rock2Position;
    private int firstBridge;
    private int secondBridge;
    private int thirdBridge;
    private float startTime = 0;
    private int amountTime;
    private GameData gameData = new GameData(0f, 0);

    public Vector3[] firstPosition;
    public Vector3[] secondPositions;
    public Vector3[] thirdPositions;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startTime = Time.time;


        rock1Position = Rock1.transform.position;
        rock2Position = Rock2.transform.position;
        firstBridge = UnityEngine.Random.Range(1, 4);
        secondBridge = UnityEngine.Random.Range(1, 4);
        thirdBridge = UnityEngine.Random.Range(1, 4);


        firstPosition = new Vector3[]
        {
            new Vector3(-27f, -86.99f, 0f),   
            new Vector3(-24.5f, -86.99f, 0f),  
            new Vector3(-22f, -86.99f, 0f)
        };

        secondPositions = new Vector3[]
        {
            new Vector3(-34.04f, -89.24f, 0f),   
            new Vector3(-33.05f, -91.53f, 0f),  
            new Vector3(-34.04f, -94.24001f, 0f)
        };
        thirdPositions = new Vector3[]
        {
            new Vector3(-44.02f, -96.97f, 0f),   
            new Vector3(-41.52f, -96.97f, 0f),  
            new Vector3(-39.02f, -96.97f, 0f)
        };

        UpdatePlayerCount();
        HammerIMG.color = new Color(0f, 0f, 0f, 1f); 
        FanIMG.color = new Color(0f, 0f, 0f, 1f); 
        TorchIMG.color = new Color(0f, 0f, 0f, 1f); 
        BotleIMG.color = new Color(0f, 0f, 0f, 1f); 


        LeaveButton.onClick.AddListener(LeaveRoom);
        MapButton.onClick.AddListener(OpenMap);
        ExitMapButton.onClick.AddListener(CloseMap);
        HintButton.onClick.AddListener(OpenHint);
        ExitHintButton.onClick.AddListener(CloseHint);

        ExitMapButton.gameObject.SetActive(false);
        Map.gameObject.SetActive(false);
        ExitHintButton.gameObject.SetActive(false);
        Hint.gameObject.SetActive(false);

        SetBridge();
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
        string timeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, (timeSpan.Milliseconds / 10));
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTime", RpcTarget.All, Time.time - startTime);
        }
        timeText.text = "Time : " + timeFormatted + "\nReturn : " + gameData.AmountTime;
        UpdatePlayerCount();
        CheckIfAllItemsFound();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
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
    void LeaveRoom()
    {
        Debug.Log(" จะออกแย้ว");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        
        SceneManager.LoadScene("MainMenuScene");
    }
    public void OnReturn(){
        amountTime = gameData.AmountTime;
        amountTime++;
        photonView.RPC("SyncAmount", RpcTarget.All, amountTime);
        
    }
    public void Restart()
    {
        amountTime = gameData.AmountTime;
        amountTime++;
        photonView.RPC("SyncAmount", RpcTarget.All, amountTime);
        Rock1.transform.position = rock1Position;
        Rock2.transform.position = rock2Position;
        SetBridge();
        playerManager.RePosition();
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

    public void SetBridge()
    {
        GameObject[] fakeWaterObjects = GameObject.FindGameObjectsWithTag("FakeWater");
        GameObject[] bridgeWaterObjects = GameObject.FindGameObjectsWithTag("Bridge");
        GameObject[] wrongBridgeWaterObjects = GameObject.FindGameObjectsWithTag("WrongBridge");
        
        foreach (GameObject obj in fakeWaterObjects)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in bridgeWaterObjects)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in wrongBridgeWaterObjects)
        {
            Destroy(obj);
        }

        for (int i = 0; i < 3; i++)
        {
            if (i+1 == firstBridge){
                Instantiate(Bridge, firstPosition[i], Quaternion.identity);
            }
            else{
                Instantiate(WrongBridge, firstPosition[i], Quaternion.identity);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (i+1 == secondBridge){
                Instantiate(HorizonBridge, secondPositions[i],  Quaternion.Euler(0, 0, 90));
            }
            else{
                Instantiate(HorizonWrongBridge, secondPositions[i],  Quaternion.Euler(0, 0, 90));
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (i+1 == thirdBridge){
                Instantiate(Bridge, thirdPositions[i], Quaternion.identity);
            }
            else{
                Instantiate(WrongBridge, thirdPositions[i], Quaternion.identity);
            }
        }
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

            isFinish = true;
            
        }
    }

    // ฟังก์ชันตรวจสอบว่าพบของครบ 4 ชิ้นหรือยัง
    void CheckIfAllItemsFound()
    {
        if (findHammer && findFan && findTorch && findBottle)
        {
            Debug.Log("พบของครบทั้ง 4 ชิ้นแล้ว!");

            // ตรวจสอบว่าเป็น MasterClient หรือไม่
            if (PhotonNetwork.IsMasterClient && isFinish)
            {
                Debug.Log(amountTime);
                photonView.RPC("GoToResultSceneRPC", RpcTarget.All); // เรียกชื่อฟังก์ชัน RPC ที่ถูกต้อง
            }
        }
    }
    [PunRPC]
    void SyncData(float time, int amount)
    {
        gameData.AmountTime = amount;
        gameData.PlayTime = time; 
    }


    [PunRPC]
    void SyncAmount(int amount)
    {
        gameData.AmountTime = amount;
    }
    [PunRPC]
    void SyncTime(float time)
    {
        gameData.PlayTime = time; 
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
            PlayerPrefs.SetFloat("playTime", gameData.PlayTime);
            PlayerPrefs.SetInt("amountTime", gameData.AmountTime); 

            PhotonNetwork.LoadLevel("ResultScene"); // โหลดฉาก ResultScene
        }
    }



}
