using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnPointData
{
    public Transform transform;
    [Header("Per-point random offset")]
    public float xRange = 0.5f;
    public float yRange = 0.5f;

    [Header("Validation")]
    public float spawnRadius = 0.5f;           // overlap check radius
    public float minDistanceFromPlayer = 3f;   // don't spawn too close
    public LayerMask obstacleMask;             // layers that block spawn

    [Header("Usage")]
    public int maxActiveAtPoint = 3;           // 0 = unlimited
    public float cooldown = 0f;                // seconds until reuse
    public float weight = 1f;                  // spawn selection weight

    [HideInInspector] public float lastUsedTime = -Mathf.Infinity;

    public bool IsAvailable() => Time.time >= lastUsedTime + cooldown;
    public void MarkUsed() => lastUsedTime = Time.time;
}