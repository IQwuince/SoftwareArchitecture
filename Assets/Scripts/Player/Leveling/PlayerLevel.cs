using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Assets.Scripts.EventBus.Events;

public class PlayerLevel : MonoBehaviour
{
    [Header("References")]
    private LevelSystem levelSystem;

    [Header("Level Up Stats")]
    [SerializeField] private AnimationCurve healthPerLevel;
    [SerializeField] private AnimationCurve damagePerLevel;

    [Header("UI")]
    public GameObject UpgradeUI;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI DamageText;
    public TextMeshProUGUI HealthCostText;
    public TextMeshProUGUI DamageCostText;

    [Header("Value levels")]
    [SerializeField] private int healthLevel = 1;
    [SerializeField] private int damageLevel = 1;
    private int lastLevel = 0;

    private void Awake()
    {
        levelSystem = GetComponent<LevelSystem>();
    }

    private void Start()
    {
        if (levelSystem != null) lastLevel = levelSystem.currentLevel;

        if (HealthText != null) HealthText.SetText("Health:100 Level:1");
        if (DamageText != null) DamageText.SetText("Damage:10 Level:1");
        if (HealthCostText != null) HealthCostText.SetText("1 level point");
        if (DamageCostText != null) DamageCostText.SetText("1 level point");
    }

    private void Update()
    {
        if (levelSystem != null && levelSystem.currentLevel > lastLevel)
        {
            int levelsGained = levelSystem.currentLevel - lastLevel;
            for (int i = 0; i < levelsGained; i++)
            {
            }
            lastLevel = levelSystem.currentLevel;
        }

        if (Keyboard.current != null && Keyboard.current.uKey.wasPressedThisFrame)
        {
            UpgradeUI.SetActive(!UpgradeUI.activeSelf);
        }
    }

    public void IncreaseHealth()
    {
        if (levelSystem.levelPoints > 0)
        {
            float healthIncrease = Mathf.RoundToInt(healthPerLevel.Evaluate(levelSystem.currentLevel));
            EventBus<PlayerHealthUpgradeEvent>.Publish(new PlayerHealthUpgradeEvent(Mathf.RoundToInt(healthIncrease)));
            EventBus<UpdatePlayerUIEvent>.Publish(new UpdatePlayerUIEvent(this));
            healthLevel++;
            levelSystem.levelPoints--;
            levelSystem.UpdateUI();

        }


    }
    public void IncreaseDamage()
    {
        if (levelSystem.levelPoints > 0)
        {
            float damageIncrease = Mathf.RoundToInt(damagePerLevel.Evaluate(levelSystem.currentLevel));
            EventBus<PlayerDamageUpgradeEvent>.Publish(new PlayerDamageUpgradeEvent(Mathf.RoundToInt(damageIncrease)));
            damageLevel++;
            levelSystem.levelPoints--;
            levelSystem.UpdateUI();
        }
    }
           


}
