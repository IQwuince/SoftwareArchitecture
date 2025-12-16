using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerUpgradeMenu : MonoBehaviour
{
    [Header("References")]
    public LevelSystem levelSystem;
    public GameObject menuPanel;

    [Header("Health Upgrade")]
    public TMP_Text healthLevelText;
    public TMP_Text healthValueText;
    public TMP_Text healthCostText;
    public Button healthUpgradeButton;

    [Header("Damage Upgrade")]
    public TMP_Text damageLevelText;
    public TMP_Text damageValueText;
    public TMP_Text damageCostText;
    public Button damageUpgradeButton;

    [Header("Upgrade Settings")]
    public AnimationCurve upgradeCostCurve;
    public int baseHealth = 100;
    public int baseDamage = 10;
    public int healthPerLevel = 13;
    public int damagePerLevel = 2;

    private int healthLevel = 1;
    private int damageLevel = 1;

    private void Start()
    {
        menuPanel.SetActive(false);
        healthUpgradeButton.onClick.AddListener(UpgradeHealth);
        damageUpgradeButton.onClick.AddListener(UpgradeDamage);
        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.uKey.wasPressedThisFrame)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            UpdateUI();
        }
    }

    void UpgradeHealth()
    {
        int cost = GetUpgradeCost(healthLevel);
        if (levelSystem.levelPoints >= cost)
        {
            levelSystem.levelPoints -= cost;
            healthLevel++;
            UpdateUI();
        }
    }

    void UpgradeDamage()
    {
        int cost = GetUpgradeCost(damageLevel);
        if (levelSystem.levelPoints >= cost)
        {
            levelSystem.levelPoints -= cost;
            damageLevel++;
            UpdateUI();
        }
    }

    int GetUpgradeCost(int level)
    {
        // Level 1 upgrade costs 1, Level 5 could cost 8, etc.
        return Mathf.Max(1, Mathf.RoundToInt(upgradeCostCurve.Evaluate(level)));
    }

    void UpdateUI()
    {
        // Health
        int healthValue = baseHealth + (healthLevel - 1) * healthPerLevel;
        healthLevelText.text = $"Level: {healthLevel}";
        healthValueText.text = $"Health: {healthValue}";
        healthCostText.text = $"Cost: {GetUpgradeCost(healthLevel)}";

        // Damage
        int damageValue = baseDamage + (damageLevel - 1) * damagePerLevel;
        damageLevelText.text = $"Level: {damageLevel}";
        damageValueText.text = $"Damage: {damageValue}";
        damageCostText.text = $"Cost: {GetUpgradeCost(damageLevel)}";

        // Enable/disable buttons based on points
        healthUpgradeButton.interactable = levelSystem.levelPoints >= GetUpgradeCost(healthLevel);
        damageUpgradeButton.interactable = levelSystem.levelPoints >= GetUpgradeCost(damageLevel);
    }

    // Optionally, expose current values for other scripts
    public int GetCurrentHealth() => baseHealth + (healthLevel - 1) * healthPerLevel;
    public int GetCurrentDamage() => baseDamage + (damageLevel - 1) * damagePerLevel;
}
