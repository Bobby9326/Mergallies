using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D playerRigidbody;  // ตรวจสอบว่าได้เชื่อมโยงใน Inspector หรือสร้างในโค้ด
    public TextMeshProUGUI test;  // สำหรับแสดงค่าของการเคลื่อนไหว
    public Animator animator;     // สำหรับควบคุม Animator
    
    public float moveSpeed = 5f;
    public Level1TutorialManager gameManager;

    void Start()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>(); // ลองหาค่าจาก GameObject เอง
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>(); // ลองหาค่าจาก GameObject เอง
        }
    }

    void Update()
    {
        // ทุกคนสามารถควบคุม Player Object ได้
        Move();

        // แสดงค่าตำแหน่งของผู้เล่นใน UI
        test.text = "X: " + playerRigidbody.position.x + " , Y: " + playerRigidbody.position.y;
    }

    void Move()
    {
        // การตรวจจับ Input เพื่อควบคุมการเคลื่อนไหวของ Player Object
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveY = Input.GetAxis("Vertical") * moveSpeed;

        // อัปเดตการเคลื่อนไหวบนเครื่องผู้เล่นที่บังคับ
        playerRigidbody.linearVelocity = new Vector2(moveX, moveY);

        // ส่งข้อมูลการเคลื่อนไหวและการอัปเดต Animator ไปยังผู้เล่นคนอื่นผ่าน RPC
        photonView.RPC("SyncMovement", RpcTarget.Others, moveX, moveY, moveX != 0 || moveY != 0);

        // อัปเดตการทำงานของ Animator สำหรับตัวผู้เล่นเอง
        UpdateAnimator(moveX, moveY, moveX != 0 || moveY != 0);
    }

    void UpdateAnimator(float moveX, float moveY, bool isRunning)
    {
        animator.SetBool("IsRunning", isRunning); // กำหนดค่าของ IsRunning
        animator.SetFloat("MoveX", moveX); // กำหนดค่าของ MoveX
    }

    [PunRPC]
    void SyncMovement(float moveX, float moveY, bool isRunning)
    {
        // รับข้อมูลความเร็วจากผู้เล่นคนอื่นและอัปเดตการเคลื่อนไหว
        playerRigidbody.linearVelocity = new Vector2(moveX, moveY);

        // อัปเดตการทำงานของ Animator บนผู้เล่นคนอื่น ๆ ด้วย
        UpdateAnimator(moveX, moveY, isRunning);
    }

    // ฟังก์ชันเมื่อชนกับวัตถุที่มี Collider2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            gameManager.FindHammer();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Torch"))
        {
            gameManager.FindTorch();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Fan"))
        {
            gameManager.FindFan();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Bottle"))
        {
            gameManager.FindBottle();
            Destroy(collision.gameObject);
        }
    }
}
