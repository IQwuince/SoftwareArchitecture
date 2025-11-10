using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("References")]
    public LevelSystem levelSystem;
    public PlayerHealth playerHealth;
    public PlayerCombat playerCombat;

    [Header("Level Up Stats")]
    public AnimationCurve healthPerLevel;
    public AnimationCurve damagePerLevel;



    private int lastLevel = 0;

    private void Start()
    {
        if (levelSystem != null)
            lastLevel = levelSystem.currentLevel;
    }

    private void Update()
    {
        if (levelSystem != null && levelSystem.currentLevel > lastLevel)
        {
            int levelsGained = levelSystem.currentLevel - lastLevel;
            for (int i = 0; i < levelsGained; i++)
            {
                IncreaseDamage();
                IncreaseHealth();
            }
            lastLevel = levelSystem.currentLevel;
        }
    }

    void IncreaseHealth()
    {
        float healthIncrease = healthPerLevel.Evaluate(levelSystem.currentLevel);
        playerHealth.maxHealth += Mathf.RoundToInt(healthIncrease);
        playerHealth.currentHealth += Mathf.RoundToInt(healthIncrease);
        playerHealth.PlayerHealthUI();
    }
    void IncreaseDamage()
    {
        float damageIncrease = damagePerLevel.Evaluate(levelSystem.currentLevel);
        playerCombat.damageAmount += Mathf.RoundToInt(damageIncrease);
    }
}
