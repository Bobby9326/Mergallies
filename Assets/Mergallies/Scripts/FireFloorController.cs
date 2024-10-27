using Photon.Pun;
using System.Collections;
using UnityEngine;

public class FireFloorController : MonoBehaviourPun
{
    public GameObject firePrefab;  // ไฟที่เราจะสร้าง
    public float minSpawnTime = 5f; // เวลาสุ่มต่ำสุด
    public float maxSpawnTime = 10f; // เวลาสุ่มสูงสุด
    public float minFireDuration = 3f; // เวลาที่ไฟแสดงต่ำสุด
    public float maxFireDuration = 5f; // เวลาที่ไฟแสดงสูงสุด
    private Vector3 firePositionOffset = new Vector3(0, 0.1f, 0); // กำหนดตำแหน่งให้สูงขึ้น Y = 0.1

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) // ให้เฉพาะ MasterClient เป็นผู้ควบคุมการสร้างไฟ
        {
            StartCoroutine(SpawnFireRoutine());
        }
    }

    IEnumerator SpawnFireRoutine()
    {
        while (true)
        {
            float spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(spawnTime);

            // สุ่มระยะเวลาที่ไฟจะคงอยู่
            float fireDuration = Random.Range(minFireDuration, maxFireDuration);

            // เรียกใช้ RPC เพื่อให้ทุกคนสร้างไฟพร้อมกัน
            photonView.RPC("SpawnFire", RpcTarget.All, fireDuration);
        }
    }

    [PunRPC]
    void SpawnFire(float fireDuration)
    {
        // สร้างไฟในตำแหน่งที่กำหนด โดยยกให้สูงขึ้น Y = 0.1
        Vector3 firePosition = transform.position + firePositionOffset;
        GameObject fire = Instantiate(firePrefab, firePosition, Quaternion.identity);

        // เริ่ม Coroutine เพื่อทำลายไฟหลังจากหมดเวลา
        StartCoroutine(DestroyFireAfterDuration(fire, fireDuration));
    }

    IEnumerator DestroyFireAfterDuration(GameObject fire, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(fire); // ทำลายไฟเมื่อหมดเวลา
    }
}
