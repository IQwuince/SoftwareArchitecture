using UnityEngine;

namespace IQwuince.Quests
{
    [CreateAssetMenu(fileName = "New Collect Quest", menuName = "Quests/Collect Items Quest")]
    public class CollectQuestSO : QuestSO
    {
        [Header("Item to Collect")]
        [Tooltip("The ItemData ScriptableObject representing the item to collect")]
        public ItemData itemToCollect;

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
            // First check base validation (but skip target check for collect quests)
            if (string.IsNullOrEmpty(id))
            {
                error = "Quest ID cannot be empty";
                return false;
            }

            if (string.IsNullOrEmpty(title))
            {
                error = "Quest title cannot be empty";
                return false;
            }

            if (targetCount <= 0)
            {
                error = "Target count must be greater than 0";
                return false;
            }

            // Collect quests require ItemData instead of GameObject target
            if (itemToCollect == null)
            {
                error = "Item to collect is required for collect quests";
                return false;
            }

            error = null;
            return true;
        }
    }
}

