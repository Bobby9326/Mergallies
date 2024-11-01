using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ResultManager : MonoBehaviourPunCallbacks
{
    public Button backToMainMenuButton;  // ปุ่มสำหรับกลับไปยังหน้า Main Menu

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        // ตรวจสอบว่าได้เชื่อมโยงปุ่มใน Inspector หรือไม่
        if (backToMainMenuButton != null)
        {
            // เพิ่ม listener สำหรับปุ่มเมื่อมีการกด
            backToMainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        else
        {
            Debug.LogError("BackToMainMenuButton ยังไม่ได้เชื่อมโยงใน Inspector");
        }
    }

    // ฟังก์ชันสำหรับกลับไปหน้า Main Menu
    void GoToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenuScene"); // กลับไปที่หน้า Main Menu
    }

}
