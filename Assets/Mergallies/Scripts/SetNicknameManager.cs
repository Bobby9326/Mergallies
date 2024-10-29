using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SetNicknameManager : MonoBehaviour
{
    public Button setButton;  
    public TMP_InputField nameField;
    void Start()
    {
        setButton.onClick.AddListener(SetName);
    }

    // Update is called once per frame
    private void SetName()
    {
        PhotonNetwork.NickName = nameField.text;
        PhotonNetwork.LoadLevel("MainMenuScene");
    }
}
