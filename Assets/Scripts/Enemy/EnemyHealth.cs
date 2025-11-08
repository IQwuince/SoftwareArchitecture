using TMPro;
using UnityEngine;
using System;

public class EnemyHealth : GenericHealth
{
    public static event Action<GameObject> OnEnemyKilled;

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab; // Assign this in the inspector or via spawner

    private TextMeshPro healthTextEnemy;
    private LevelSystem levelSystem;

    private void Start()
    {
        healthTextEnemy = GetComponentInChildren<TextMeshPro>();
        levelSystem = UnityEngine.Object.FindFirstObjectByType<LevelSystem>();
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
            OnEnemyKilled?.Invoke(enemyPrefab);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void UpdateHealthUI()
    {
        healthTextEnemy.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
