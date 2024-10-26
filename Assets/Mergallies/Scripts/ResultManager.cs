using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Button backToMainMenuButton;  // ปุ่มสำหรับกลับไปยังหน้า Main Menu

    void Start()
    {
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
        SceneManager.LoadScene("MainMenuScene");  // โหลดซีน Main Menu
    }
}
