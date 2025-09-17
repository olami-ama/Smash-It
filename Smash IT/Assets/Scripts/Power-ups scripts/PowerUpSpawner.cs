using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Prefabs (drag them here)")]
    public GameObject[] powerUpPrefabs;

    [Header("Spawn Timing")]
    public float firstDelay = 3f;
    public float spawnInterval = 10f;

    [Header("Spawn Areas")]
    public BoxCollider2D topArea;
    public BoxCollider2D bottomArea;

    [Header("Limit Settings")]
    public int maxPowerUps = 3; // maximum allowed in scene at once

    public MatchSettings matchSettings; // assign in inspector


    private bool spawnOnBottom = true;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstDelay);

        while (true)
        {
            int currentPowerUps = GameObject.FindGameObjectsWithTag("Power up").Length;

            if (currentPowerUps < maxPowerUps)
            {
                SpawnPowerUp();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnPowerUp()
    {
        // Pick random prefab
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        // Decide which area to spawn in
        BoxCollider2D area = spawnOnBottom ? bottomArea : topArea;
        spawnOnBottom = !spawnOnBottom; // alternate each time

        // Pick a random point inside the box area
        Vector3 pos = GetRandomPointInBox(area);

        // Spawn
        Instantiate(prefab, pos, Quaternion.identity);
    }

    private Vector3 GetRandomPointInBox(BoxCollider2D box)
    {
        Bounds bounds = box.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0f);
    }
}
