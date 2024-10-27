using System.Collections;
using UnityEngine;

public class FireFloorController : MonoBehaviour
{
    public GameObject firePrefab;  // ไฟที่เราจะสร้าง
    public float minSpawnTime = 5f; // เวลาสุ่มต่ำสุด
    public float maxSpawnTime = 10f; // เวลาสุ่มสูงสุด
    public float minFireDuration = 3f; // เวลาที่ไฟแสดงต่ำสุด
    public float maxFireDuration = 5f; // เวลาที่ไฟแสดงสูงสุด
    private Vector3 firePositionOffset = new Vector3(0, 0.1f, 0); // กำหนดตำแหน่งให้สูงขึ้น Y = 0.1

    void Start()
    {
        StartCoroutine(SpawnFireRoutine());
    }

    IEnumerator SpawnFireRoutine()
    {
        while (true)
        {
            float spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(spawnTime);

            // สร้างไฟในตำแหน่งที่กำหนด โดยยกให้สูงขึ้น Y = 0.1
            Vector3 firePosition = transform.position + firePositionOffset;
            GameObject fire = Instantiate(firePrefab, firePosition, Quaternion.identity);

            float fireDuration = Random.Range(minFireDuration, maxFireDuration);
            yield return new WaitForSeconds(fireDuration);

            // ทำลายไฟเมื่อหมดเวลา
            Destroy(fire);
        }
    }
}
