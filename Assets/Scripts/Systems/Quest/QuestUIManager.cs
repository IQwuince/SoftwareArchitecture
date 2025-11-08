using TMPro;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public QuestSystem questSystem;
    public TextMeshProUGUI questText;

    private void Update()
    {
        if (questSystem == null || questSystem.activeQuests == null || questSystem.activeQuests.Length == 0)
        {
            questText.text = "No active quest";
            return;
        }

        foreach (var quest in questSystem.activeQuests)
        {
            if (quest is EnemyKillQuest killQuest)
            {
                int killed = questSystem.GetEnemyKillCount(killQuest.enemyPrefab);
                questText.text = $"{killQuest.questName}\nKill {killQuest.requiredKills} {killQuest.enemyPrefab.name}: {killed}/{killQuest.requiredKills}";
                return;
            }
            // Add other quest types here as needed
        }
    }
}
