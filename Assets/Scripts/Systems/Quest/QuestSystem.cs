using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public QuestData[] activeQuests;

    private Dictionary<GameObject, int> enemyKillCounts = new();
    private Dictionary<string, int> itemPickupCounts = new();

    private void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += RegisterEnemyKill;
        Inventory.OnItemPickedUp += RegisterItemPickup;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyKilled -= RegisterEnemyKill;
        Inventory.OnItemPickedUp -= RegisterItemPickup;
    }

    public void RegisterEnemyKill(GameObject enemyPrefab)
    {
        if (!enemyKillCounts.ContainsKey(enemyPrefab))
            enemyKillCounts[enemyPrefab] = 0;
        enemyKillCounts[enemyPrefab]++;

        // Log progress for each relevant quest
        foreach (var quest in activeQuests)
        {
            if (quest is EnemyKillQuest killQuest && killQuest.enemyPrefab == enemyPrefab)
            {
                int killed = enemyKillCounts[enemyPrefab];
                Debug.Log($"{killed}/{killQuest.requiredKills} {enemyPrefab.name} enemies killed for quest: {killQuest.questName}");
            }
        }
    }

    public int GetEnemyKillCount(GameObject enemyPrefab)
    {
        return enemyKillCounts.TryGetValue(enemyPrefab, out int count) ? count : 0;
    }

    public void RegisterItemPickup(string itemName)
    {
        if (!itemPickupCounts.ContainsKey(itemName))
            itemPickupCounts[itemName] = 0;
        itemPickupCounts[itemName]++;
    }

    public int GetItemCount(string itemName)
    {
        return itemPickupCounts.TryGetValue(itemName, out int count) ? count : 0;
    }

    private void Update()
    {
        foreach (var quest in activeQuests)
        {
            quest.CheckCompletion(this);
        }
    }
}
