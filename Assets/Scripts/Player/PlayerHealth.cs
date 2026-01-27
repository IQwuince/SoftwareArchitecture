using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerHealth : GenericHealth
{

    private void Start()
    {
        PlayerHealthUI();
    }
    void OnEnable()
    {
        EventBus.Subscribe<PlayerHealthUpgradeEvent>(PlayerValueUpgrade);
    }
    void OnDisable()
    {
        EventBus.UnSubscribe<PlayerHealthUpgradeEvent>(PlayerValueUpgrade);
    }

    void PlayerValueUpgrade(PlayerHealthUpgradeEvent playerHealthUpgradeEvent)
    {
        maxHealth += playerHealthUpgradeEvent.healthValue;
        currentHealth += playerHealthUpgradeEvent.healthValue;
        PlayerHealthUI();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        PlayerHealthUI();
        EventBus.Publish(new PlayerDamagedEvent(this));
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
            Debug.Log("Dead");
            GameManager.Instance.DiePlayer();
            PlayerHealthUI();
        }
    }

    
    public void PlayerHealthUI()
    {
        EventBus.Publish(new PlayerUIValueChangeEvent(currentHealth, minHealth, maxHealth));
    }
}

