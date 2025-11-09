using UnityEngine;

namespace IQwuince.Quests
{
    /// <summary>
    /// Quest that requires collecting a specific number of items
    /// </summary>
    [CreateAssetMenu(fileName = "New Collect Quest", menuName = "Quests/Collect Items Quest")]
    public class CollectQuestSO : QuestSO
    {
        private void OnEnable()
        {
            questType = QuestType.Collect;
        }

        private void Reset()
        {
            questType = QuestType.Collect;
            title = "Collect Items";
            description = "Gather a specified number of items";
        }
    }
}
