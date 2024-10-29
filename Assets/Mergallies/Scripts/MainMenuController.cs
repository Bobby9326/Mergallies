using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // เพิ่มการใช้งาน IEnumerator และ Coroutine

public class MainMenuController : MonoBehaviourPunCallbacks
{
    public Button createRoomButton;  // ปุ่มสำหรับสร้างห้อง
    public Button joinRoomButton;    // ปุ่มสำหรับเข้าร่วมห้อง
    public Button joinInputButton; 
    public Button createInputButton; 
    public TMP_InputField joininputField;  // ใช้ TMP_InputField
    public TMP_InputField createinputField; // ใช้ TMP_InputField
    public Image joinInput;
    public Image createInput;

    private bool isConnectedToLobby = false;  // ตรวจสอบการเชื่อมต่อกับล็อบบี้
    private bool isConnectedToMaster = false; // ตรวจสอบการเชื่อมต่อกับ Master Server
    private bool isTryingToCreateRoom = false;  // ตรวจสอบว่ากำลังพยายามสร้างห้องหรือไม่
    private bool isSceneLoading = false; // ตรวจสอบการโหลดซีน

    private void Start()
    {
        Debug.Log(PhotonNetwork.NickName);
        // ตรวจสอบการเชื่อมโยงวัตถุเพื่อป้องกัน NullReferenceException
        if (createRoomButton == null || joinRoomButton == null || joinInputButton == null || createInputButton == null || 
            joininputField == null || createinputField == null || joinInput == null || createInput == null)
        {
            Debug.LogError("Some UI components are not assigned in the Inspector.");
            return;
        }

        // ซ่อน InputField ทั้งสองเมื่อเริ่มต้น
        joinInput.gameObject.SetActive(false);
        createInput.gameObject.SetActive(false);

        // เชื่อมฟังก์ชันกับปุ่ม
        createRoomButton.onClick.AddListener(ShowCreateRoomInput);
        createInputButton.onClick.AddListener(CreateRoomOnClick);
        joinRoomButton.onClick.AddListener(ShowJoinRoomInput);
        joinInputButton.onClick.AddListener(JoinRoomOnClick);
    }

    private void ShowCreateRoomInput()
    {
        createInput.gameObject.SetActive(true);  // แสดง Input สำหรับการสร้างห้อง
    }

    private void CreateRoomOnClick()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createinputField.text, roomOptions, null);

    }

    private void ShowJoinRoomInput()
    {
        joinInput.gameObject.SetActive(true);  // แสดง Input สำหรับการเข้าร่วมห้อง
    }

    private void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(joininputField.text);
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

}
