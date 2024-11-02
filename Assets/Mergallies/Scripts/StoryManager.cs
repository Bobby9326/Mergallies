using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviourPunCallbacks
{
    public Button nextButton;
    public Button skipButton;
    public TextMeshProUGUI skipText;
    public Image storyIMG1;
    public Image storyIMG2;
    public Image storyIMG3;


    private int currentImageIndex = 1;
    private int skipCount = 0;
    private bool[] skipStates = new bool[2]; // เก็บสถานะของผู้เล่น 2 คนว่าได้กด Skip หรือไม่
    private bool isLoadingScene = false; // ตัวแปรเพื่อป้องกันการโหลดซ้ำ

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        nextButton.onClick.AddListener(OnNextButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    void Update()
    {
        // อัปเดตภาพที่แสดงผลตามค่า currentImageIndex
        UpdateStoryImages();

        // อัปเดตสถานะ skipText ตาม skipCount
        UpdateSkipText();
    }

    void UpdateStoryImages()
    {
        // ซ่อนรูปภาพทั้งหมดก่อน
        storyIMG1.gameObject.SetActive(false);
        storyIMG2.gameObject.SetActive(false);
        storyIMG3.gameObject.SetActive(false);


        // แสดงรูปภาพตาม currentImageIndex
        if (currentImageIndex == 1)
        {
            storyIMG1.gameObject.SetActive(true);
        }
        else if (currentImageIndex == 2)
        {
            storyIMG2.gameObject.SetActive(true);
        }
        else if (currentImageIndex == 3)
        {
            storyIMG3.gameObject.SetActive(true);
        }

    }

    void UpdateSkipText()
    {
        if (skipCount == 1)
        {
            skipText.text = "(1/2)";
        }
        else if (skipCount == 2 && !isLoadingScene) // เพิ่มตรวจสอบสถานะการโหลด
        {
            skipText.text = "(2/2)";
            isLoadingScene = true; // ตั้งสถานะการโหลดเพื่อป้องกันการโหลดซ้ำ
            PhotonNetwork.LoadLevel("Level1TutorialScene"); // ไปที่ Scene Game เมื่อผู้เล่นทั้งสองคนกด Skip
        }
        else
        {
            skipText.text = "";
        }
    }

    public void OnNextButtonClicked()
    {
        if (!isLoadingScene) // ป้องกันการโหลดซ้ำ
        {
            photonView.RPC("NextImage", RpcTarget.All); // ใช้ RPC เพื่อให้การกด Next ส่งผลกับทุกคน
        }
    }

    [PunRPC]
    void NextImage()
    {
        Debug.Log("Before: " + currentImageIndex);
        currentImageIndex++;
        Debug.Log("After: " + currentImageIndex);
        if (currentImageIndex > 3 && !isLoadingScene)
        {
            isLoadingScene = true; // ป้องกันการโหลดซ้ำ
            PhotonNetwork.LoadLevel("Level1TutorialScene"); // ไปที่ Scene Game เมื่อกด Next ถึงรูปสุดท้าย
        }
    }

    public void OnSkipButtonClicked()
    {
        photonView.RPC("HandleSkip", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber); // ส่งหมายเลขผู้เล่นที่กด Skip ผ่าน RPC
    }

    [PunRPC]
    void HandleSkip(int playerID)
    {
        // ตรวจสอบว่าผู้เล่นได้กด skip ไปแล้วหรือยัง
        if (!skipStates[playerID - 1] && !isLoadingScene) 
        {
            skipStates[playerID - 1] = true; // บันทึกสถานะการกด Skip ของผู้เล่น
            skipCount++;
        }
    }
}
