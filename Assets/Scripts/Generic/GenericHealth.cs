using UnityEngine;

public class GenericHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
        DieGeneric();
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void DieGeneric()
    {
        if (currentHealth <= minHealth)
        {
            Debug.Log("Dead");
            GameManager.Instance.Die();
            
        }
    }
}
