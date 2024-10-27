using UnityEngine;

public class StormController : MonoBehaviour
{
    public float moveSpeed = 1f; // ความเร็วในการเคลื่อนที่
    public float moveRange = 10f; // ขอบเขตการเคลื่อนที่ไม่เกิน ±10
    private Vector3 startPosition; // ตำแหน่งเริ่มต้นของวัตถุ
    private Vector3 targetPosition; // ตำแหน่งเป้าหมายที่สุ่มมา

    void Start()
    {
        // บันทึกตำแหน่งเริ่มต้น
        startPosition = transform.position;
        // สุ่มตำแหน่งเป้าหมายในขอบเขต
        GenerateRandomTarget();
    }

    void Update()
    {
        // เคลื่อนที่ไปยังตำแหน่งเป้าหมาย
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // หากถึงตำแหน่งเป้าหมายแล้วให้สุ่มตำแหน่งใหม่
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            GenerateRandomTarget();
        }
    }

    // ฟังก์ชันสุ่มตำแหน่งเป้าหมายใหม่
    void GenerateRandomTarget()
    {
        float randomX = Random.Range(startPosition.x - moveRange, startPosition.x + moveRange);
        float randomY = Random.Range(startPosition.y - moveRange, startPosition.y + moveRange);
        targetPosition = new Vector3(randomX, randomY, startPosition.z); // เก็บตำแหน่งเป้าหมายใหม่
    }
}
