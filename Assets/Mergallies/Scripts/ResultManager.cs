using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System;

public class ResultManager : MonoBehaviourPunCallbacks
{
    public Button backToMainMenuButton;  // ปุ่มสำหรับกลับไปยังหน้า Main Menu
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI AmountText;

    void Start()
    {
        int savedPlayCount = PlayerPrefs.GetInt("amountTime");
        float savedPlayTime = PlayerPrefs.GetFloat("playTime");
        TimeSpan timeSpan = TimeSpan.FromSeconds(savedPlayTime);
        string timeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, (timeSpan.Milliseconds / 10));
        string player1Name = PhotonNetwork.PlayerList[0].NickName;
        string player2Name = PhotonNetwork.PlayerList[1].NickName;
        NameText.text = player1Name + " & " + player2Name;
        TimeText.text = timeFormatted;
        AmountText.text = ""+savedPlayCount;


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
