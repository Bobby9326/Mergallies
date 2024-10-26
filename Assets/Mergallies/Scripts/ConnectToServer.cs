using UnityEngine;

using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        // เริ่มการเชื่อมต่อกับ Photon Server โดยใช้การตั้งค่าใน PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // Callback เมื่อเชื่อมต่อกับ Master Server สำเร็จ
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");
        // เข้าร่วมล็อบบี้หลังจากเชื่อมต่อสำเร็จ
        PhotonNetwork.JoinLobby();
    }

    // Callback เมื่อเข้าล็อบบี้สำเร็จ
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.");
        SceneManager.LoadScene("MainMenuScene");

    }

    // Callback เมื่อการเชื่อมต่อล้มเหลว
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon. Reason: " + cause.ToString());
    }
}