using Photon.Pun;
using UnityEngine;

public class StormController : MonoBehaviourPun
{
    public float moveSpeed = 1f; // ความเร็วในการเคลื่อนที่
    public float moveRange = 10f; // ขอบเขตการเคลื่อนที่ไม่เกิน ±10
    private Vector3 startPosition; // ตำแหน่งเริ่มต้นของวัตถุ
    private Vector3 targetPosition; // ตำแหน่งเป้าหมายที่สุ่มมา

    void Start()
    {
        // บันทึกตำแหน่งเริ่มต้น
        startPosition = transform.position;
        
        if (PhotonNetwork.IsMasterClient)
        {
            // เฉพาะ MasterClient จะสุ่มตำแหน่งใหม่
            GenerateRandomTarget();
        }
    }

    void Update()
    {
        // เคลื่อนที่ไปยังตำแหน่งเป้าหมาย
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // หากถึงตำแหน่งเป้าหมายแล้วให้สุ่มตำแหน่งใหม่
        if (PhotonNetwork.IsMasterClient && Vector3.Distance(transform.position, targetPosition) < 0.1f)
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

        // ส่งตำแหน่งใหม่ให้กับผู้เล่นคนอื่น
        photonView.RPC("SetTargetPosition", RpcTarget.All, targetPosition);
    }

    // RPC สำหรับตั้งค่าตำแหน่งเป้าหมายใหม่
    [PunRPC]
    void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
}
