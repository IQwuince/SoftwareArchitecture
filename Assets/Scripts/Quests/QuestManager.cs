using System;
using System.Collections.Generic;
using UnityEngine;

namespace IQwuince.Quests
{
    /// <summary>
    /// Main quest system manager. Handles quest activation, progress tracking, and events.
    /// Enforces one active quest per quest type.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        [Header("Sample Quests for Testing")]
        [Tooltip("Sample Kill quest activated with O key")]
        public KillQuestSO sampleKillQuest;
        
        [Tooltip("Sample Collect quest activated with P key")]
        public CollectQuestSO sampleCollectQuest;

        // Active quests dictionary - one per type
        private Dictionary<QuestType, Quest> activeQuests = new Dictionary<QuestType, Quest>();
        
        // Progress tracking
        private Dictionary<GameObject, int> enemyKillCounts = new Dictionary<GameObject, int>();
        private Dictionary<GameObject, int> itemCollectCounts = new Dictionary<GameObject, int>();

        // Events
        public event Action<Quest> OnQuestActivated;
        public event Action<Quest> OnQuestProgressChanged;
        public event Action<Quest> OnQuestCompleted;

        private void OnEnable()
        {
            // Subscribe to game events
            EnemyHealth.OnEnemyKilled += HandleEnemyKilled;
            Inventory.OnItemPickedUp += HandleItemPickedUp;
        }

        private void OnDisable()
        {
            // Unsubscribe from game events
            EnemyHealth.OnEnemyKilled -= HandleEnemyKilled;
            Inventory.OnItemPickedUp -= HandleItemPickedUp;
        }

        private void Update()
        {
            // Check for sample quest activation keys
            if (Input.GetKeyDown(KeyCode.O) && sampleKillQuest != null)
            {
                ActivateQuest(sampleKillQuest);
            }

            if (Input.GetKeyDown(KeyCode.P) && sampleCollectQuest != null)
            {
                ActivateQuest(sampleCollectQuest);
            }
        }

        /// <summary>
        /// Activate a quest. Returns false if a quest of the same type is already active.
        /// </summary>
        public bool ActivateQuest(QuestSO questData)
        {
            if (questData == null)
            {
                Debug.LogWarning("Cannot activate null quest");
                return false;
            }

            // Check if quest of same type is already active
            if (activeQuests.ContainsKey(questData.questType))
            {
                Debug.LogWarning($"Cannot activate {questData.title}: A {questData.questType} quest is already active");
                return false;
            }

            // Validate quest data
            if (!questData.Validate(out string error))
            {
                Debug.LogError($"Cannot activate invalid quest {questData.title}: {error}");
                return false;
            }

            // Create and activate quest
            Quest quest = new Quest(questData);
            quest.OnProgressChanged += HandleQuestProgressChanged;
            quest.OnQuestCompleted += HandleQuestCompleted;
            
            activeQuests[questData.questType] = quest;

            Debug.Log($"Quest activated: {questData.title}");
            OnQuestActivated?.Invoke(quest);

            return true;
        }

        /// <summary>
        /// Get the active quest of a specific type, or null if none active
        /// </summary>
        public Quest GetActiveQuest(QuestType type)
        {
            return activeQuests.TryGetValue(type, out Quest quest) ? quest : null;
        }

        /// <summary>
        /// Get all active quests
        /// </summary>
        public IEnumerable<Quest> GetActiveQuests()
        {
            return activeQuests.Values;
        }

        /// <summary>
        /// Handle enemy killed event from game systems
        /// </summary>
        private void HandleEnemyKilled(GameObject enemyPrefab)
        {
            if (enemyPrefab == null) return;

            // Track kill count
            if (!enemyKillCounts.ContainsKey(enemyPrefab))
                enemyKillCounts[enemyPrefab] = 0;
            enemyKillCounts[enemyPrefab]++;

            // Check if any Kill quest matches this enemy
            if (activeQuests.TryGetValue(QuestType.Kill, out Quest killQuest))
            {
                if (killQuest.questData.target == enemyPrefab)
                {
                    killQuest.IncrementProgress();
                    Debug.Log($"Quest progress: {killQuest.questData.title} - {killQuest.GetProgressString()}");
                }
            }
        }

        /// <summary>
        /// Handle item picked up event from inventory system.
        /// Note: The inventory system sends item name as string, but we need to match by prefab.
        /// This is a temporary adapter - ideally inventory would send GameObject reference.
        /// </summary>
        private void HandleItemPickedUp(string itemName)
        {
            // This is called from existing Inventory.OnItemPickedUp event which sends string
            // We need to find the matching GameObject by name in active collect quests
            if (activeQuests.TryGetValue(QuestType.Collect, out Quest collectQuest))
            {
                if (collectQuest.questData.target != null && 
                    collectQuest.questData.target.name == itemName)
                {
                    collectQuest.IncrementProgress();
                    Debug.Log($"Quest progress: {collectQuest.questData.title} - {collectQuest.GetProgressString()}");
                }
            }
        }

        /// <summary>
        /// Public API for game systems to notify item collection by GameObject reference
        /// </summary>
        public void UpdateCollectProgress(GameObject itemPrefab)
        {
            if (itemPrefab == null) return;

            // Track collect count
            if (!itemCollectCounts.ContainsKey(itemPrefab))
                itemCollectCounts[itemPrefab] = 0;
            itemCollectCounts[itemPrefab]++;

            // Check if any Collect quest matches this item
            if (activeQuests.TryGetValue(QuestType.Collect, out Quest collectQuest))
            {
                if (collectQuest.questData.target == itemPrefab)
                {
                    collectQuest.IncrementProgress();
                    Debug.Log($"Quest progress: {collectQuest.questData.title} - {collectQuest.GetProgressString()}");
                }
            }
        }

        /// <summary>
        /// Public API for game systems to notify enemy kill by GameObject reference
        /// </summary>
        public void UpdateKillProgress(GameObject enemyPrefab)
        {
            HandleEnemyKilled(enemyPrefab);
        }

        private void HandleQuestProgressChanged(Quest quest)
        {
            OnQuestProgressChanged?.Invoke(quest);
        }

        private void HandleQuestCompleted(Quest quest)
        {
            Debug.Log($"Quest completed: {quest.questData.title}! Rewards: XP={quest.questData.reward.xp}, ItemID={quest.questData.reward.itemId}, Currency={quest.questData.reward.currency}");
            
            // Remove from active quests
            activeQuests.Remove(quest.questData.questType);
            
            OnQuestCompleted?.Invoke(quest);
        }
    }
}
