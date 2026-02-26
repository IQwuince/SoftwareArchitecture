using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform player;

    [Header("Spawn Points (per-entry settings)")]
    [SerializeField] SpawnPointData[] spawnPoints;

    [Header("Spawner Settings")]
    [SerializeField] float spawnInterval;
    [SerializeField] int maxActiveEnemies; 

    readonly List<GameObject> activeEnemies = new List<GameObject>();
    readonly Dictionary<GameObject, int> originIndex = new Dictionary<GameObject, int>();
    readonly HashSet<int> usedSpawnPoints = new HashSet<int>(); // one-time spawn tracking
    float timer;

    private void Start()
    {
        timer = spawnInterval;
        if ((spawnPoints == null || spawnPoints.Length == 0) && transform.childCount > 0)
        {
            var children = new List<SpawnPointData>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(new SpawnPointData { transform = transform.GetChild(i) });
            }
            if (children.Count > 0) spawnPoints = children.ToArray();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TrySpawnOne();
            timer = spawnInterval;
        }

        CleanupActiveList();
    }

    void CleanupActiveList()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                originIndex.Remove(activeEnemies[i]);
                activeEnemies.RemoveAt(i);
            }
        }

        var toRemove = new List<GameObject>();
        foreach (var kv in originIndex)
            if (kv.Key == null) toRemove.Add(kv.Key);
        foreach (var k in toRemove) originIndex.Remove(k);
    }

    void TrySpawnOne()
    {
        if (enemyPrefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (activeEnemies.Count >= maxActiveEnemies) return;

        // build candidate indices and total weight
        var candidates = new List<int>();
        float totalWeight = 0f;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var sp = spawnPoints[i];
            if (sp == null || sp.transform == null) continue;
            if (usedSpawnPoints.Contains(i)) continue; // one-time only
            if (!sp.IsAvailable()) continue;
            if (player != null && Vector2.Distance(player.position, sp.transform.position) < sp.minDistanceFromPlayer) continue;

            candidates.Add(i);
            totalWeight += Mathf.Max(0f, sp.weight);
        }

        if (candidates.Count == 0) return;

        // weighted pick
        float r = Random.Range(0f, totalWeight);
        float acc = 0f;
        int chosenIndex = candidates[Random.Range(0, candidates.Count)];
        foreach (var idx in candidates)
        {
            acc += Mathf.Max(0f, spawnPoints[idx].weight);
            if (r <= acc)
            {
                chosenIndex = idx;
                break;
            }
        }

        var chosen = spawnPoints[chosenIndex];

        // single attempt (no spawnAttempts loop)
        Vector2 offset = new Vector2(
            Random.Range(-chosen.xRange, chosen.xRange),
            Random.Range(-chosen.yRange, chosen.yRange)
        );
        Vector2 pos = (Vector2)chosen.transform.position + offset;

        if (player != null && Vector2.Distance(player.position, pos) < chosen.minDistanceFromPlayer) return;
        if (Physics2D.OverlapCircle(pos, chosen.spawnRadius, chosen.obstacleMask)) return;

        var go = Instantiate(enemyPrefab, pos, chosen.transform.rotation);
        var health = go.GetComponentInChildren<EnemyHealth>(); // or GetComponent<EnemyHealth>() depending on your hierarchy
        if (health != null) health.enemyPrefab = enemyPrefab; // assign prefab ASSET, not spawned instance
        activeEnemies.Add(go);
        originIndex[go] = chosenIndex;

        chosen.MarkUsed(); 
        usedSpawnPoints.Add(chosenIndex); 
    }

    public void NotifyEnemyRemoved(GameObject enemy)
    {
        if (enemy == null) return;
        originIndex.Remove(enemy);
        activeEnemies.Remove(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (var sp in spawnPoints)
        {
            if (sp == null || sp.transform == null) continue;
            Gizmos.DrawWireSphere(sp.transform.position, sp.spawnRadius);
            Vector3 size = new Vector3(Mathf.Max(0.01f, sp.xRange * 2f), Mathf.Max(0.01f, sp.yRange * 2f), 0.01f);
            Gizmos.DrawWireCube(sp.transform.position, size);
        }
    }
}