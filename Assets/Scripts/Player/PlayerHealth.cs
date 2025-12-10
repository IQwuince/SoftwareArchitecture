using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerHealth : GenericHealth
{
    [Header("UI Elements")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    public static event System.Action OnPlayerDamaged;


    private void Start()
    {
        PlayerHealthUI();
    }

    private void Update()
    {
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        PlayerHealthUI();
        OnPlayerDamaged?.Invoke(); 
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
        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        //healthSlider.value = (float)currentHealth / maxHealth;
    }
}

