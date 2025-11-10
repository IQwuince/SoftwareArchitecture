using UnityEngine;

namespace IQwuince.Quests
{
    /// <summary>
    /// Quest that requires killing a specific number of enemies
    /// </summary>
    [CreateAssetMenu(fileName = "New Kill Quest", menuName = "Quests/Kill Enemies Quest")]
    public class KillQuestSO : QuestSO
    {
        [Header("Kill Quest Configuration")]
        [Tooltip("The enemy prefab to kill")]
        public GameObject targetEnemy;

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

        public override bool Validate(out string error)
        {
            if (!base.Validate(out error))
                return false;

            if (targetEnemy == null)
            {
                error = "Target enemy is required for kill quests";
                return false;
            }

            return true;
        }
    }
}
