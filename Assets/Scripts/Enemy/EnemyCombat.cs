using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    private PlayerHealth playerHealth;
    // Get damage by touching enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                Debug.Log("Player hit by enemy, took 10 damage.");
            }

        }
    }
}
