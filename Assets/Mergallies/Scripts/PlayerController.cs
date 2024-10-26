using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D playerRigidbody;  // ตรวจสอบว่าได้เชื่อมโยงใน Inspector หรือสร้างในโค้ด
    public TextMeshProUGUI test;  // สำหรับแสดงค่าของการเคลื่อนไหว
    public Animator animator;     // สำหรับควบคุม Animator
    
    public float moveSpeed = 5f;
     // ตัวแปรสำหรับตรวจสอบว่าพบ Hammer หรือไม่

    public Level1TutorialManager gameManager;

    void Start()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>(); // ลองหาค่าจาก GameObject เอง
        }

        if (playerRigidbody == null)
        {
            Debug.LogError("Rigidbody2D ยังไม่ได้รับการกำหนด");
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>(); // ลองหาค่าจาก GameObject เอง
        }

        if (animator == null)
        {
            Debug.LogError("Animator ยังไม่ได้รับการกำหนด");
        }

        
    }

    void Update()
    {
        // ทุกคนควบคุม Player Object เดียวกัน
        Move();

        // แสดงค่าตำแหน่งของผู้เล่นใน UI
        test.text = "X: " + playerRigidbody.position.x + " , Y: " + playerRigidbody.position.y;

        // แสดงว่าพบ Hammer หรือไม่

    }

    void Move()
    {
        // การตรวจจับ Input เพื่อควบคุมการเคลื่อนไหวของ Player Object
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveY = Input.GetAxis("Vertical") * moveSpeed;

        // อัปเดตการเคลื่อนไหวบนเครื่องผู้เล่น
        playerRigidbody.linearVelocity = new Vector2(moveX, moveY);

        // ส่งข้อมูลการเคลื่อนไหวไปยังผู้เล่นคนอื่นผ่าน RPC
        photonView.RPC("SyncMovement", RpcTarget.Others, moveX, moveY);

        // อัปเดตการทำงานของ Animator
        UpdateAnimator(moveX, moveY);
    }

    void UpdateAnimator(float moveX, float moveY)
    {
        // ตรวจสอบว่าผู้เล่นกำลังวิ่งหรือไม่
        bool isRunning = (moveX != 0 || moveY != 0);
        animator.SetBool("IsRunning", isRunning); // กำหนดค่าของ IsRunning

        // กำหนดค่าของ MoveX
        animator.SetFloat("MoveX", moveX);
    }

    // ฟังก์ชันเมื่อชนกับวัตถุที่มี Collider2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ชนกับวัตถุ: " + collision.gameObject.name);  // เพิ่ม Debug เพื่อตรวจสอบการชน

        if (collision.gameObject.CompareTag("Hammer"))
        {
            Debug.Log("พบ Hammer!"); 
            gameManager.FindHammer();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Torch"))
        {
            Debug.Log("พบ Torch!"); 
            gameManager.FindTorch();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Fan"))
        {
            Debug.Log("พบ Fan!"); 
            gameManager.FindFan();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Bottle"))
        {
            Debug.Log("พบ Bottle!"); 
            gameManager.FindBottle();
            Destroy(collision.gameObject);
        }
    }

    // RPC สำหรับทำลายวัตถุ Hammer

    [PunRPC]
    void SyncMovement(float moveX, float moveY)
    {
        // รับข้อมูลความเร็วจากผู้เล่นคนอื่นและอัปเดตการเคลื่อนไหว
        playerRigidbody.linearVelocity = new Vector2(moveX, moveY);

        // อัปเดตการทำงานของ Animator บนผู้เล่นคนอื่น ๆ ด้วย
        UpdateAnimator(moveX, moveY);
    }
}
