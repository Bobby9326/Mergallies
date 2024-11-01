using UnityEngine;

public class RockController : MonoBehaviour
{
    public GameObject wall;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าชนกับวัตถุที่มี Tag "Switch" หรือไม่
        if (other.CompareTag("Switch"))
        {
            wall.SetActive(false);
            Debug.Log("กำแพงถูกปิดการใช้งานเมื่อหินชนกับสวิตช์");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ตรวจสอบว่าหินออกจากการชนกับสวิตช์
        if (other.CompareTag("Switch"))
        {
            wall.SetActive(true);
            Debug.Log("กำแพงถูกเปิดการใช้งานเมื่อหินออกจากสวิตช์");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // หยุดการเคลื่อนที่ของหินเมื่อหยุดชน
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
