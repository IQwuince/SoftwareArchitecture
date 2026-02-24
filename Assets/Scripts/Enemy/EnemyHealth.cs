using TMPro;
using UnityEngine;
using System;

public class EnemyHealth : GenericHealth
{
    public static event Action<GameObject> OnEnemyKilledEvent;

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;
    private EnemyMovement2D enemyMovement;

    [SerializeField] private TextMeshPro healthTextEnemy;
    private EnemyLoot enemyLoot;

    private void Start()
    {
        if (healthTextEnemy != null) healthTextEnemy = GetComponentInChildren<TextMeshPro>();
        enemyLoot = UnityEngine.Object.FindFirstObjectByType<EnemyLoot>();
        enemyMovement = GetComponentInParent<EnemyMovement2D>();
        UpdateHealthUI();
        
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UpdateHealthUI();
        enemyMovement.KnockBackEnemy();
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
            if (enemyLoot != null) enemyLoot.GiveRewards();
            OnEnemyKilledEvent?.Invoke(enemyPrefab);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void UpdateHealthUI()
    {
       if(healthTextEnemy != null) healthTextEnemy.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
