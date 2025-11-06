using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public EnemyCombat enemyCombat;
    public EnemyHealth enemyHealth;
    public Transform player;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;

    private void FixedUpdate()
    {
        if (enemyHealth.currentHealth > 0 && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.fixedDeltaTime;
        }

        OnDrawGizmos();
    }

    // Visual Debugging
    private void OnDrawGizmos()
    {
        if (player == null) return;

        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.position;
        float distance = Vector3.Distance(enemyPos, playerPos);

        // Calculate the point at the end of the detection range
        Vector3 direction = (playerPos - enemyPos).normalized;
        Vector3 rangeEnd = enemyPos + direction * Mathf.Min(distance, detectionRange);

        // Draw green line for in-range part
        Gizmos.color = Color.green;
        Gizmos.DrawLine(enemyPos, rangeEnd);

        // Draw red line for out-of-range part
        if (distance > detectionRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rangeEnd, playerPos);
        }
    }

}
