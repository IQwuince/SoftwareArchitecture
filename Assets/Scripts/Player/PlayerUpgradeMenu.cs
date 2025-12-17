using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerUpgradeMenu : MonoBehaviour
{
    [Header("References")]
    private LevelSystem levelSystem;
    public GameObject menuPanel;

    [Header("Health Upgrade")]
    [SerializeField] private TMP_Text healthLevelText;
    [SerializeField] private TMP_Text healthValueText;
    [SerializeField] private TMP_Text healthCostText;
    [SerializeField] private Button healthUpgradeButton;

    [Header("Damage Upgrade")]
    [SerializeField] private TMP_Text damageLevelText;
    [SerializeField] private TMP_Text damageValueText;
    [SerializeField] private TMP_Text damageCostText;
    [SerializeField] private Button damageUpgradeButton;

    [Header("Upgrade Settings")]
    [SerializeField] private AnimationCurve upgradeCostCurve;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private int healthPerLevel = 13;
    [SerializeField] private int damagePerLevel = 2;

    private int healthLevel = 1;
    private int damageLevel = 1;

    private void Awake()
    {
        levelSystem = UnityEngine.Object.FindFirstObjectByType<LevelSystem>();
    }

    private void Start()
    {
        menuPanel.SetActive(false);
        if (healthUpgradeButton != null) healthUpgradeButton.onClick.AddListener(UpgradeHealth);
        if (damageUpgradeButton != null) damageUpgradeButton.onClick.AddListener(UpgradeDamage);
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
        if (levelSystem != null && levelSystem.levelPoints >= cost )
        {
            levelSystem.levelPoints -= cost;
            healthLevel++;
            UpdateUI();
        }
    }

    void UpgradeDamage()
    {
        int cost = GetUpgradeCost(damageLevel);
        if (levelSystem != null && levelSystem.levelPoints >= cost)
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
        if (healthLevelText != null) healthLevelText.text = $"Level: {healthLevel}";
        if (healthValueText != null) healthValueText.text = $"Health: {healthValue}";
        if (healthCostText != null) healthCostText.text = $"Cost: {GetUpgradeCost(healthLevel)}";

        // Damage
        int damageValue = baseDamage + (damageLevel - 1) * damagePerLevel;
        if (damageLevelText != null) damageLevelText.text = $"Level: {damageLevel}";
        if (damageValueText != null) damageValueText.text = $"Damage: {damageValue}";
        if (damageCostText != null) damageCostText.text = $"Cost: {GetUpgradeCost(damageLevel)}";

        // Enable/disable buttons based on points
        if (healthUpgradeButton != null) healthUpgradeButton.interactable = levelSystem.levelPoints >= GetUpgradeCost(healthLevel);
        if (damageUpgradeButton != null) damageUpgradeButton.interactable = levelSystem.levelPoints >= GetUpgradeCost(damageLevel);
    }

    // Optionally, expose current values for other scripts
    public int GetCurrentHealth() => baseHealth + (healthLevel - 1) * healthPerLevel;
    public int GetCurrentDamage() => baseDamage + (damageLevel - 1) * damagePerLevel;
}
