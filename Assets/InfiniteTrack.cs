using System.Collections.Generic;
using UnityEngine;

public class InfiniteTrack : MonoBehaviour
{
    [Header("REFERENCES")]
    public Transform player;
    public List<Transform> roadTiles = new List<Transform>();

    [Header("ROAD")]
    public float tileLength = 500f;
    public float recycleBehindDistance = 10f;

    [Header("TRACK WIDTH")]
    public float trackHalfWidth = 7f;
    public float edgeMargin = 0.8f;

    [Header("OBSTACLES")]
    public GameObject[] obstaclePrefabs;
    public Transform obstaclesParent;
    public float obstacleY = 0.5f;

    [Header("SPAWN CONTROL")]
    public float spawnInterval = 35f;        // engeller arası mesafe
    public float spawnAheadDistance = 120f;  // oyuncunun KAÇ BİRİM önünde doğsun

    private float nextSpawnZ;

    void Start()
    {
        if (player == null || roadTiles.Count < 2)
        {
            Debug.LogError("InfiniteTrack setup eksik!");
            enabled = false;
            return;
        }

        roadTiles.Sort((a, b) => a.position.z.CompareTo(b.position.z));

        // 🔴 EN KRİTİK SATIR
        // İlk engel HER ZAMAN oyuncunun ilerisinde başlar
        nextSpawnZ = player.position.z + spawnAheadDistance;
    }

    void Update()
    {
        RecycleRoads();
        SpawnObstacles();
        CleanupObstacles();
    }

    // ================= ROAD =================
    void RecycleRoads()
    {
        Transform firstRoad = roadTiles[0];
        float behindZ = player.position.z - recycleBehindDistance;

        if (firstRoad.position.z + tileLength < behindZ)
        {
            Transform lastRoad = roadTiles[roadTiles.Count - 1];
            float newZ = lastRoad.position.z + tileLength;

            Vector3 pos = firstRoad.position;
            pos.z = newZ;
            firstRoad.position = pos;

            roadTiles.RemoveAt(0);
            roadTiles.Add(firstRoad);
        }
    }

    // ================= SPAWN =================
    void SpawnObstacles()
    {
        if (obstaclePrefabs.Length == 0 || obstaclesParent == null) return;

        // Oyuncu spawn noktasına yaklaştıkça BİR TANE üret
        if (player.position.z + spawnAheadDistance > nextSpawnZ)
        {
            int count = Random.Range(1, 3); // 1–2 engel, spam yok

            for (int i = 0; i < count; i++)
            {
                float x = GetSmartRandomX();
                float zOffset = Random.Range(-4f, 4f);

                Vector3 pos = new Vector3(
                    x,
                    obstacleY,
                    nextSpawnZ + zOffset
                );

                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity, obstaclesParent);
            }

            // 🔴 BUNU ARTIRDIĞIN SÜRECE ENGEL ASLA BİTMEZ
            nextSpawnZ += spawnInterval;
        }
    }

    // ================= CLEANUP =================
    void CleanupObstacles()
    {
        for (int i = obstaclesParent.childCount - 1; i >= 0; i--)
        {
            Transform obs = obstaclesParent.GetChild(i);
            if (obs.position.z < player.position.z - 60f)
            {
                Destroy(obs.gameObject);
            }
        }
    }

    // ================= X LOGIC =================
    float GetSmartRandomX()
    {
        float r = Random.value;

        float leftEdge = -trackHalfWidth + edgeMargin;
        float rightEdge = trackHalfWidth - edgeMargin;

        if (r < 0.33f)
            return Random.Range(leftEdge, leftEdge + 1.5f);       // sol bariyer
        else if (r < 0.66f)
            return Random.Range(-2f, 2f);                        // orta
        else
            return Random.Range(rightEdge - 1.5f, rightEdge);    // sağ bariyer
    }
}
