using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    public Button createRoomButton;  // ปุ่มสำหรับสร้างห้อง
    public Button joinRoomButton;    // ปุ่มสำหรับเข้าร่วมห้อง
    private bool isConnectedToLobby = false;  // ตัวแปรเพื่อตรวจสอบว่าต่อกับล็อบบี้สำเร็จแล้วหรือยัง
    private bool isConnectedToMaster = false; // ตัวแปรเพื่อตรวจสอบว่าต่อกับ Master Server สำเร็จแล้วหรือยัง
    private bool isTryingToCreateRoom = false;  // ตัวแปรเพื่อตรวจสอบว่ากำลังพยายามสร้างห้องหรือไม่
    private bool isSceneLoading = false; // ตรวจสอบการโหลดซีน

    private void Start()
    {
        // เชื่อมฟังก์ชันกับปุ่ม
        createRoomButton.onClick.AddListener(() => {
            isTryingToCreateRoom = true;  // กำลังพยายามสร้างห้อง
            TryCreateRoom();
        });
        joinRoomButton.onClick.AddListener(JoinRoom);
    }

    private void Awake()
    {
        // ตรวจสอบสถานะการเชื่อมต่อก่อนที่จะทำการเชื่อมต่อใหม่
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // เริ่มการเชื่อมต่อกับ Photon
            Debug.Log("กำลังเชื่อมต่อกับ Photon...");
        }
        else
        {
            Debug.LogWarning("ไคลเอนต์เชื่อมต่ออยู่แล้ว ไม่จำเป็นต้องเชื่อมต่อใหม่");
        }
    }

    // ฟังก์ชันสำหรับตรวจสอบและพยายามสร้างห้อง
    public void TryCreateRoom()
    {
        if (isConnectedToMaster && isConnectedToLobby)
        {
            CreateRoom();
        }
        else if (!isConnectedToMaster)
        {
            Debug.LogError("ไม่สามารถสร้างห้องได้ ไคลเอนต์ยังไม่ได้เชื่อมต่อกับ Master Server");
        }
        else if (!isConnectedToLobby)
        {
            Debug.LogError("ไม่สามารถสร้างห้องได้ ไคลเอนต์ยังไม่ได้เชื่อมต่อกับล็อบบี้");
        }
    }

    // ฟังก์ชันสำหรับสร้างห้อง
    public void CreateRoom()
    {
        Debug.Log("พยายามสร้างห้อง...");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // จำกัดจำนวนผู้เล่นในห้องให้ได้เพียง 2 คน

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
        {
            PhotonNetwork.CreateRoom(null, roomOptions, null); // สร้างห้องใหม่แบบไม่มีชื่อเฉพาะ
        }
        else
        {
            Debug.LogError("ไคลเอนต์ยังไม่พร้อมที่จะสร้างห้อง กรุณารอสักครู่...");
        }
    }

    // ฟังก์ชันสำหรับเข้าร่วมห้อง
    public void JoinRoom()
    {
        Debug.Log("พยายามเข้าร่วมห้อง...");
        PhotonNetwork.JoinRandomRoom(); // เข้าร่วมห้องแบบสุ่ม
    }

    // ฟังก์ชันที่ถูกเรียกเมื่อเข้าร่วมห้องล้มเหลว
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("การเข้าร่วมห้องล้มเหลว: " + message);
        TryCreateRoom(); // ถ้าไม่มีห้องให้เข้าร่วม ก็สร้างห้องใหม่
    }

    // ฟังก์ชันที่ถูกเรียกเมื่อเข้าร่วมห้องสำเร็จ
    public override void OnJoinedRoom()
    {
        Debug.Log("เข้าห้องสำเร็จ!");

        // ตรวจสอบว่ากำลังโหลดซีนอยู่หรือไม่
        if (!isSceneLoading)
        {
            isSceneLoading = true;  // ตั้งสถานะว่ากำลังโหลดซีน
            PhotonNetwork.LoadLevel("LobbyScene"); // ไปที่หน้า LobbyScene เมื่อเข้าห้องแล้ว
        }
        else
        {
            Debug.LogWarning("ซีนกำลังโหลดอยู่แล้ว");
        }
    }

    // ฟังก์ชันที่ถูกเรียกเมื่อเชื่อมต่อกับ Master Server สำเร็จ
    public override void OnConnectedToMaster()
    {
        Debug.Log("เชื่อมต่อกับ Master Server สำเร็จ");
        isConnectedToMaster = true; // ตอนนี้ไคลเอนต์เชื่อมต่อกับ Master Server สำเร็จแล้ว

        if (!isConnectedToLobby)
        {
            PhotonNetwork.JoinLobby();  // เข้าร่วมล็อบบี้หลังจากเชื่อมต่อสำเร็จ
        }
    }

    // ฟังก์ชันที่ถูกเรียกเมื่อเชื่อมต่อกับล็อบบี้สำเร็จ
    public override void OnJoinedLobby()
    {
        Debug.Log("เข้าสู่ล็อบบี้สำเร็จ");
        isConnectedToLobby = true;  // ตอนนี้ไคลเอนต์เชื่อมต่อกับล็อบบี้สำเร็จแล้ว

        // ถ้ากำลังพยายามสร้างห้องหลังจากเชื่อมต่อสำเร็จ
        if (isTryingToCreateRoom)
        {
            CreateRoom();  // สร้างห้องหลังจากเข้าล็อบบี้สำเร็จ
            isTryingToCreateRoom = false;  // รีเซ็ตสถานะการสร้างห้อง
        }
    }

    // ฟังก์ชันที่ถูกเรียกเมื่อออกจากห้องสำเร็จ
    public override void OnLeftRoom()
    {
        Debug.Log("ออกจากห้องสำเร็จ! กำลังกลับไปที่ Master Server...");
        isSceneLoading = false;  // รีเซ็ตสถานะการโหลดซีน
        PhotonNetwork.Disconnect();  // ตัดการเชื่อมต่อจาก GameServer เพื่อเชื่อมต่อกับ Master Server ใหม่
    }

    // ฟังก์ชันที่ถูกเรียกเมื่อการเชื่อมต่อขาด
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("การเชื่อมต่อขาดจาก Photon: " + cause.ToString());
        isConnectedToMaster = false; // รีเซ็ตสถานะการเชื่อมต่อกับ Master Server
        isConnectedToLobby = false;  // รีเซ็ตสถานะการเชื่อมต่อกับล็อบบี้

        // เชื่อมต่อกลับไปที่ Master Server หลังจากตัดการเชื่อมต่อจาก GameServer
        if (cause == DisconnectCause.DisconnectByClientLogic || cause == DisconnectCause.ClientTimeout)
        {
            PhotonNetwork.ConnectUsingSettings(); // เชื่อมต่อกลับไปยัง Master Server
        }
    }
}
