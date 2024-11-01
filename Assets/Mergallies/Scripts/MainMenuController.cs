using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button joinInputButton;
    public Button createInputButton;
    public Button joinExitButton;
    public Button createExitButton;
    public Button exitGameButton;
    public TMP_InputField joininputField;
    public TMP_InputField createinputField;
    public Image joinInput;
    public Image createInput;

    private bool isConnectedToLobby = false;
    private bool isConnectedToMaster = false;
    private bool isTryingToCreateRoom = false;
    private bool isSceneLoading = false;

    private void Start()
    {
        Debug.Log(PhotonNetwork.NickName);
        
        if (createRoomButton == null || joinRoomButton == null || joinInputButton == null || createInputButton == null || 
            joininputField == null || createinputField == null || joinInput == null || createInput == null || exitGameButton == null)
        {
            Debug.LogError("Some UI components are not assigned in the Inspector.");
            return;
        }

        joinInput.gameObject.SetActive(false);
        createInput.gameObject.SetActive(false);

        createRoomButton.onClick.AddListener(ShowCreateRoomInput);
        createInputButton.onClick.AddListener(CreateRoomOnClick);
        joinRoomButton.onClick.AddListener(ShowJoinRoomInput);
        joinInputButton.onClick.AddListener(JoinRoomOnClick);
        joinExitButton.onClick.AddListener(UnShowJoinRoomInput);
        createExitButton.onClick.AddListener(UnShowCreateRoomInput);
        exitGameButton.onClick.AddListener(QuitGame);
    }

    private void ShowCreateRoomInput()
    {
        createInput.gameObject.SetActive(true);
    }
    private void UnShowCreateRoomInput()
    {
        createInput.gameObject.SetActive(false);
    }

    private void CreateRoomOnClick()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createinputField.text, roomOptions, null);
    }

    private void ShowJoinRoomInput()
    {
        joinInput.gameObject.SetActive(true);
    }
    private void UnShowJoinRoomInput()
    {
        joinInput.gameObject.SetActive(false);
    }

    private void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(joininputField.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("เข้าห้องสำเร็จ!");

        if (!isSceneLoading)
        {
            isSceneLoading = true;
            PhotonNetwork.LoadLevel("LobbyScene");
        }
        else
        {
            Debug.LogWarning("ซีนกำลังโหลดอยู่แล้ว");
        }
    }

    private void QuitGame()
    {
        Debug.Log("ออกจากเกม");
        Application.Quit();
    }
}
