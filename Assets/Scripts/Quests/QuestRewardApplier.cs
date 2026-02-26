using UnityEngine;

namespace IQwuince.Quests
{
    public class QuestRewardApplier : MonoBehaviour
    {
        [SerializeField] private QuestManager questManager;

        private void OnEnable()
        {
            if (questManager == null)
                questManager = FindFirstObjectByType<QuestManager>();

            if (questManager != null)
                questManager.OnQuestCompleted += HandleQuestCompleted;
        }

        private void OnDisable()
        {
            if (questManager != null)
                questManager.OnQuestCompleted -= HandleQuestCompleted;
        }

        private void HandleQuestCompleted(Quest quest)
        {
            if (quest?.questData == null) return;

            var reward = quest.questData.reward;

            // XP (matches how EnemyLoot does it)
            if (reward.xp > 0)
            {
                EventBus<LevelSystemAddXpEvent>.Publish(new LevelSystemAddXpEvent(reward.xp));
                Debug.Log($"[QuestRewardApplier] Awarded XP: {reward.xp}");
            }

            // TODO: currency + items (need your game APIs)
            if (reward.currency != 0)
                Debug.Log($"[QuestRewardApplier] Currency reward configured: {reward.currency} (not applied yet)");

            if (reward.itemId != 0)
                Debug.Log($"[QuestRewardApplier] Item reward configured: itemId={reward.itemId} (not applied yet)");
        }
    }
}