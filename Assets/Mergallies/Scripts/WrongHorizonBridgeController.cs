using UnityEngine;

public class WrongHorizonBridgeController : MonoBehaviour
{
    public GameObject replacementPrefab; // Prefab ของ GameObject ที่จะสร้างขึ้นแทน

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่า GameObject ที่ชนเป็น Player หรือไม่
        if (other.CompareTag("Player"))
        {
            // สร้าง replacementPrefab ขึ้นในตำแหน่งและการหมุนเดียวกับวัตถุปัจจุบัน
            Vector3 spawnPosition = transform.position + new Vector3(-0.5f, 0.5f, 0);
            Instantiate(replacementPrefab, spawnPosition, transform.rotation);


            // ทำลายวัตถุนี้
            Destroy(gameObject);
        }
    }
}
