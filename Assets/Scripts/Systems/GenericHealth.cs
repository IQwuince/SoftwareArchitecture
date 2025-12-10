using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GenericHealth : MonoBehaviour
{

    [Header("Health variables")]
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    public virtual void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

}
