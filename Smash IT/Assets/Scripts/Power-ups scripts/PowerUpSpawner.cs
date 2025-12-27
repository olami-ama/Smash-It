using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Prefabs")]
    public GameObject[] powerUpPrefabs;

    [Header("Spawn Timing")]
    public float firstDelay = 3f;
    public float spawnInterval = 10f;

    [Header("Spawn Areas")]
    public BoxCollider2D TopArea;

    [Header("Limits")]
    public int maxPowerUps = 4;

    private void Start()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogError("[PowerUpSpawner] No prefabs assigned.");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstDelay);

        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
                yield break;

            int active = GameObject.FindGameObjectsWithTag("Power up").Length;

            if (active < maxPowerUps)
            {
                SpawnPowerUp(); // fixed to use existing method
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnPowerUp()
    {
        if (MatchSettingsData.selectedPowerUps == null ||
            MatchSettingsData.selectedPowerUps.Count == 0)
        {
            Debug.Log("[PowerUpSpawner] No power-ups selected for this match.");
            return;
        }

        List<GameObject> candidates = new List<GameObject>();

        foreach (var p in powerUpPrefabs)
        {
            PowerUpPickup pickup = p.GetComponent<PowerUpPickup>();
            if (pickup == null) continue;

            if (MatchSettingsData.selectedPowerUps.Contains(pickup.powerUpType))
            {
                candidates.Add(p);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.Log("[PowerUpSpawner] No matching power-ups found.");
            return;
        }

        GameObject prefab = candidates[Random.Range(0, candidates.Count)];

        // Spawn on the player side only (bottom area)
        Vector3 pos = GetRandomPoint(TopArea); // use existing GetRandomPoint
        Instantiate(prefab, pos, Quaternion.identity);
    }

    private Vector3 GetRandomPoint(BoxCollider2D box)
    {
        Bounds b = box.bounds;
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y),
            0f
        );
    }
}
