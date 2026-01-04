using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float obstacleChance = 0.6f;

    void Start()
    {
        SpawnObstacle();
    }

    void SpawnObstacle()
    {
        if (Random.value > obstacleChance) return;

        float randomX = Random.Range(-2.5f, 2.5f);
        Vector3 spawnPos = transform.position + new Vector3(randomX, 0.5f, 0);

        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, transform);
    }
}
