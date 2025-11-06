using TMPro;
using UnityEngine;

public class EnemyHealth : GenericHealth
{
    private TextMeshPro healthTextEnemy;
    private LevelSystem levelSystem;

    private void Start()
    {
        healthTextEnemy = GetComponentInChildren<TextMeshPro>();
        levelSystem = Object.FindFirstObjectByType<LevelSystem>();

        UpdateHealthUI();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UpdateHealthUI();
        DieEnemy();
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        UpdateHealthUI();
    }

    private void DieEnemy()
    {
        if (currentHealth <= minHealth)
        {
            levelSystem.AddExperience(150);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void UpdateHealthUI()
    {
        healthTextEnemy.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
