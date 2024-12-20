using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D playerRigidbody;
    public TextMeshProUGUI test;
    public Animator animator;

    public float moveSpeed = 5f;
    public float pushForce = 5f;
    public Level1TutorialManager gameManager;

    void Start()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            photonView.RequestOwnership();
        }
        if (photonView.IsMine)
        {
            Move(); // เรียกฟังก์ชันการเคลื่อนไหว
        }

        // แสดงค่าตำแหน่งของผู้เล่นใน UI
        test.text = "X: " + playerRigidbody.position.x + " , Y: " + playerRigidbody.position.y;
    }

    void Move()
    {
        // ตรวจจับ Input เพื่อควบคุมการเคลื่อนไหวของ Player Object
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveY = Input.GetAxis("Vertical") * moveSpeed;

        // ใช้ Rigidbody2D เพื่อเคลื่อนที่ และซิงค์ผ่าน Photon Transform View
        playerRigidbody.linearVelocity = new Vector2(moveX, moveY);

        // ซิงค์ Animator เพื่อให้ทุกคนเห็นอนิเมชั่นที่ถูกต้อง
        photonView.RPC("UpdateAnimator", RpcTarget.All, moveX, moveY != 0 || moveX != 0);
    }

    // ฟังก์ชันที่ใช้ RPC เพื่อซิงค์อนิเมชัน
    [PunRPC]
    void UpdateAnimator(float moveX, bool isRunning)
    {
        if(animator != null)
        {
            animator.SetBool("IsRunning", isRunning);
            animator.SetFloat("MoveX", moveX);
        }
        else
        {
            Debug.LogError("Animator is not assigned");
        }
    }

    public void RePosition()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            gameManager.GoFinish();
        }
        if (other.CompareTag("WrongBridge"))
        {
            transform.position = new Vector3(0, 0, 0);
            gameManager.OnReturn();
        }
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
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // ตั้งตำแหน่งของผู้เล่นไปที่ (0, 0)
            Debug.Log("find : Obstacle");
            transform.position = new Vector3(0, 0, 0);
            gameManager.OnReturn();
        }
        else if (collision.gameObject.CompareTag("Rock"))
        {
            // ดันก้อนหิน
            Rigidbody2D rockRigidbody = collision.rigidbody;
            if (rockRigidbody != null)
            {
                Vector2 pushDirection = collision.GetContact(0).normal * -1; // หาทิศทางการดัน
                rockRigidbody.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}
