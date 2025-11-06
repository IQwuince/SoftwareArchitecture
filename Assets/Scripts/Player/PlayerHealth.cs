using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerHealth : GenericHealth
{
    [Header("UI Elements")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    private void Start()
    {
        HealthUI();
    }

    private void Update()
    {
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        HealthUI();
        DiePlayer();
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        HealthUI();
    }
    private void DiePlayer()
    {
        if (currentHealth <= minHealth)
        {
            Debug.Log("Dead");
            GameManager.Instance.DiePlayer();

        }
    }

    private void HealthUI()
    {
        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        //healthSlider.value = (float)currentHealth / maxHealth;
    }
}

