using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{

    // Get damage by touching enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                Debug.Log("Player hit by enemy, took 10 damage.");
            }
        }
    }
}
