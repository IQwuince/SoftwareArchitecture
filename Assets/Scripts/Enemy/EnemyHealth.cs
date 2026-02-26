using TMPro;
using UnityEngine;

public class EnemyHealth : GenericHealth
{
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
            Debug.Log($"[EnemyHealth] EnemyKilledEvent: enemyPrefab={(enemyPrefab ? enemyPrefab.name : "NULL")} instance={gameObject.name}");
            EventBus<EnemyKilledEvent>.Publish(new EnemyKilledEvent(enemyPrefab));
            if (enemyLoot != null) enemyLoot.GiveRewards();
            GameObject.Destroy(this.gameObject);
        }
    }

    private void UpdateHealthUI()
    {
       if(healthTextEnemy != null) healthTextEnemy.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
