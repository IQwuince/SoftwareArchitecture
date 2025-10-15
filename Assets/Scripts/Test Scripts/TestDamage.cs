using UnityEngine;

public class TestDamage : MonoBehaviour
{
    public PlayerHealth PlayerHealth;
    [SerializeField] private int setHealth;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");
            PlayerHealth.TakeDamage(setHealth);
            Debug.Log(PlayerHealth.currentHealth);
        
    }
}
