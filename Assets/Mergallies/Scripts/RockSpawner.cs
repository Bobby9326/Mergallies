using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab; // พรีแฟบของก้อนหิน
    public float spawnInterval = 2f; // ช่วงเวลาในการสปาวน์หิน (วินาที)
    public float fallSpeed = 3f; // ความเร็วในการเคลื่อนที่ของก้อนหิน

    private void Start()
    {
        // เริ่มต้นการสปาวน์หิน
        InvokeRepeating("SpawnRock", 0f, spawnInterval);
    }

    private void SpawnRock()
    {
        // ใช้ตำแหน่งของ RockSpawner ในการสร้างหิน
        Vector3 spawnPosition = transform.position;

        // สร้างหินจากพรีแฟบและกำหนดตำแหน่ง
        GameObject rock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
        
        // เพิ่มคอมโพเนนต์ RockMover เพื่อให้หินเคลื่อนที่ลงล่าง
        RockMover rockMover = rock.AddComponent<RockMover>();
        rockMover.fallSpeed = fallSpeed;
    }
}
