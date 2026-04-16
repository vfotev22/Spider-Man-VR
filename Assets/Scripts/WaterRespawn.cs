using UnityEngine;

public class WaterRespawn : MonoBehaviour
{
    [Header("Respawn")]
    public string spawnPointTag = "Spawn Point";
    public string waterLayerName = "Water";

    [Header("References")]
    public Rigidbody playerRb;

    private Transform spawnPoint;
    private int waterLayer;

    void Start()
    {
        waterLayer = LayerMask.NameToLayer(waterLayerName);

        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnPointTag);
        if (spawnObj != null)
        {
            spawnPoint = spawnObj.transform;
        }
        else
        {
            Debug.LogError("[RESPAWN] No object found with tag: " + spawnPointTag);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            Respawn();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == waterLayer)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        if (spawnPoint == null)
            return;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Debug.Log("[RESPAWN] Player respawned at spawn point");
    }
}