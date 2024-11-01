using Photon.Pun;
using System.Collections;
using UnityEngine;

public class FireFloorController : MonoBehaviourPun
{
    public GameObject firePrefab;
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;
    public float minFireDuration = 3f;
    public float maxFireDuration = 5f;
    private Vector3 firePositionOffset = new Vector3(0, 0.1f, 0);
    
    public Sprite defaultSprite;
    public Sprite warningSprite;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
            return;
        }

        if (defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }

        if (PhotonNetwork.IsMasterClient)
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

            float fireDuration = Random.Range(minFireDuration, maxFireDuration);

            photonView.RPC("ChangeToWarningSprite", RpcTarget.All);
            yield return new WaitForSeconds(1f);

            photonView.RPC("SpawnFire", RpcTarget.All, fireDuration);
        }
    }

    [PunRPC]
    void ChangeToWarningSprite()
    {
        if (warningSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = warningSprite;
        }
    }

    [PunRPC]
    void SpawnFire(float fireDuration)
    {
        Vector3 firePosition = transform.position + firePositionOffset;
        GameObject fire = Instantiate(firePrefab, firePosition, Quaternion.identity);
        StartCoroutine(DestroyFireAfterDuration(fire, fireDuration));
    }

    IEnumerator DestroyFireAfterDuration(GameObject fire, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(fire);
        
        // เปลี่ยนกลับเป็น sprite ปกติหลังจากลบไฟ
        photonView.RPC("ResetToDefaultSprite", RpcTarget.All);
    }

    [PunRPC]
    void ResetToDefaultSprite()
    {
        if (defaultSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}