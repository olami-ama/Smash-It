using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Match Settings (ScriptableObject)")]
    public MatchSettings matchSettings; // assign in inspector

    private bool spawnOnBottom = true;

    void Start()
    {
        if (powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("[Spawner] No power-up prefabs assigned!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }
    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;  // stop movement
                                                        // normal paddle controls here
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
        var candidates = new List<GameObject>();

        // If matchSettings is missing, spawn all
        if (matchSettings == null)
        {
            Debug.LogWarning("[Spawner] MatchSettings missing! Spawning all power-ups.");
            candidates.AddRange(powerUpPrefabs);
        }
        else
        {
            foreach (var p in powerUpPrefabs)
            {
                PowerUpPickup pickup = p.GetComponent<PowerUpPickup>();
                if (pickup == null)
                {
                    Debug.LogWarning($"[Spawner] Prefab {p.name} has no PowerUpPickup script!");
                    continue;
                }

                if (matchSettings.allowedPowerUps.Contains(pickup.type))
                {
                    candidates.Add(p);
                }
            }
        }

        if (candidates.Count == 0)
        {
            Debug.Log("[Spawner] No allowed power-ups to spawn (check MatchSettings).");
            return;
        }

        // Pick one randomly
        GameObject prefab = candidates[Random.Range(0, candidates.Count)];

        // Alternate or randomize spawn side
        BoxCollider2D area = spawnOnBottom ? bottomArea : topArea;
        spawnOnBottom = !spawnOnBottom;

        Vector3 pos = GetRandomPointInBox(area);
        Instantiate(prefab, pos, Quaternion.identity);

        Debug.Log($"[Spawner] Spawned {prefab.name} in {(area == bottomArea ? "Bottom" : "Top")} area at {pos}");
    }

    private Vector3 GetRandomPointInBox(BoxCollider2D box)
    {
        if (box == null)
        {
            Debug.LogError("[Spawner] Missing BoxCollider2D area reference!");
            return Vector3.zero;
        }

        Bounds bounds = box.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0f);
    }
}
