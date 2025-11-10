using UnityEngine;
using IQwuince.Quests;


    [CreateAssetMenu(fileName = "New Kill Quest", menuName = "Quests/Kill Enemies Quest")]
    public class KillQuestSO : QuestSO
    {
        private void OnEnable()
        {
            questType = QuestType.Kill;
        }

        private void Reset()
        {
            questType = QuestType.Kill;
            title = "Kill Enemies";
            description = "Defeat a specified number of enemies";
        }
    }

