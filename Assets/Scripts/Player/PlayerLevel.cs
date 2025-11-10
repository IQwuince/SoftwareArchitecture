using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public LevelSystem levelSystem;
    public PlayerHealth playerHealth;
    public PlayerCombat playerCombat;

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
                IncreaseDamage(10);
                IncreaseHealth(20);
            }
            lastLevel = levelSystem.currentLevel;
        }
    }

    void IncreaseHealth(int amount)
    {
        playerHealth.maxHealth += amount;
        playerHealth.currentHealth += amount;
        playerHealth.PlayerHealthUI();
    }
    void IncreaseDamage(int amount)
    {
        playerCombat.damageAmount += amount;
    }
}
