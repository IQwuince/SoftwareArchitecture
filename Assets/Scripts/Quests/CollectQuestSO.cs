using UnityEngine;

namespace IQwuince.Quests
{
    /// <summary>
    /// Quest that requires collecting a specific number of items from inventory
    /// </summary>
    [CreateAssetMenu(fileName = "New Collect Quest", menuName = "Quests/Collect Items Quest")]
    public class CollectQuestSO : QuestSO
    {
        [Header("Collect Quest Configuration")]
        [Tooltip("The ItemData to collect from inventory")]
        public ItemData targetItem;

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

        public override bool Validate(out string error)
        {
            if (!base.Validate(out error))
                return false;

            if (targetItem == null)
            {
                error = "Target item is required for collect quests";
                return false;
            }

            return true;
        }
    }
}
