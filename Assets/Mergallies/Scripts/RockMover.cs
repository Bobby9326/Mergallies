using UnityEngine;

public class RockMover : MonoBehaviour
{
    public float fallSpeed = 3f; // ความเร็วในการเคลื่อนที่ของหิน

    private void Update()
    {
        // เคลื่อนที่ลงด้านล่างตลอดเวลา
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าชนกับวัตถุที่มี Tag "Target" หรือไม่
        if (other.CompareTag("Target"))
        {
            Destroy(gameObject); // ทำลายตัวเองเมื่อชนกับ Target
        }
    }
}
