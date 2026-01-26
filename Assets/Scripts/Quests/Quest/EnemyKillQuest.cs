using UnityEngine;

namespace IQwuince.Quests
{
    [CreateAssetMenu(menuName = "Quests/Enemy Kill Quest")]
    public class EnemyKillQuest : QuestData
    {
        public GameObject enemyPrefab;
        public int requiredKills;

        public override bool CheckCompletion(QuestSystem questSystem)
        {
            int killed = questSystem.GetEnemyKillCount(enemyPrefab);
            if (!isCompleted && killed >= requiredKills)
            {
                isCompleted = true;
                Debug.Log($"Quest completed: {questName} ({requiredKills} {enemyPrefab.name} enemies killed)");
            }
            return isCompleted;
        }
    }
}
