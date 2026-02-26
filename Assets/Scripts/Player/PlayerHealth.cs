using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerHealth : GenericHealth
{
    public PlayerMovement playerMovement;
    private void Start()
    {
        PlayerHealthUI();
    }
    void OnEnable()
    {
        EventBus<PlayerHealthUpgradeEvent>.OnEvent += PlayerValueUpgrade;
        EventBus<PlayerDamagedEvent>.OnEvent += OnPlayerDamaged;
    }
    void OnDisable()
    {
        EventBus<PlayerHealthUpgradeEvent>.OnEvent -= PlayerValueUpgrade;
        EventBus<PlayerDamagedEvent>.OnEvent -= OnPlayerDamaged;
    }

    void PlayerValueUpgrade(PlayerHealthUpgradeEvent playerHealthUpgradeEvent)
    {
        maxHealth += playerHealthUpgradeEvent.healthValue;
        currentHealth += playerHealthUpgradeEvent.healthValue;
        PlayerHealthUI();
    }
    void OnPlayerDamaged(PlayerDamagedEvent playerDamagedEvent)
    {
        TakeDamage(playerDamagedEvent.damageTaken);
    }

    public override void TakeDamage(int damage)
    {
        //Debug.Log("damage taken");
        base.TakeDamage(damage);
        PlayerHealthUI();
        playerMovement.OnPlayerDamagedMovement();
        DiePlayer();
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        PlayerHealthUI();
    }
    private void DiePlayer()
    {
        if (currentHealth <= minHealth)
        {
            //Debug.Log("Dead");
            GameManager.Instance.DiePlayer();
            PlayerHealthUI();
        }
    }

    
    public void PlayerHealthUI()
    {
        EventBus<PlayerUIValueChangeEvent>.Publish(new PlayerUIValueChangeEvent(currentHealth, minHealth, maxHealth));
    }

    
}

