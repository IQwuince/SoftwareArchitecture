using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    public int damageAmount;

    // Get damage by touching enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                EventBus.Publish(new PlayerDamagedEvent(damageAmount));
            
        }
    }
}
