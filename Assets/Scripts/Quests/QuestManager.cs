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

        [Header("Integration")]
        [Tooltip("Reference to player inventory for item tracking and rewards")]
        public Inventory playerInventory;

        [Tooltip("Reference to player level system for XP rewards")]
        public LevelSystem playerLevelSystem;

        // Active quests dictionary - one per type
        private Dictionary<QuestType, Quest> activeQuests = new Dictionary<QuestType, Quest>();
        
        // Progress tracking
        private Dictionary<GameObject, int> enemyKillCounts = new Dictionary<GameObject, int>();
        private Dictionary<string, int> itemCollectCounts = new Dictionary<string, int>();

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
                if (killQuest.questData is KillQuestSO killQuestData)
                {
                    if (killQuestData.targetEnemy == enemyPrefab)
                    {
                        killQuest.IncrementProgress();
                        Debug.Log($"Quest progress: {killQuest.questData.title} - {killQuest.GetProgressString()}");
                    }
                }
            }
        }

        /// <summary>
        /// Handle item picked up event from inventory system.
        /// Checks inventory for item counts to track collect quest progress.
        /// </summary>
        private void HandleItemPickedUp(string itemName)
        {
            if (playerInventory == null)
            {
                Debug.LogWarning("QuestManager: playerInventory is not assigned. Cannot track item collection.");
                return;
            }

            // Check if any Collect quest is active
            if (activeQuests.TryGetValue(QuestType.Collect, out Quest collectQuest))
            {
                if (collectQuest.questData is CollectQuestSO collectQuestData)
                {
                    // Check if this item matches the quest target by ID
                    if (collectQuestData.targetItem != null)
                    {
                        // Count items in inventory that match the target item's ID
                        int itemCount = CountItemsInInventory(collectQuestData.targetItem.id);
                        
                        // Update quest progress if count increased
                        if (itemCount > collectQuest.currentProgress)
                        {
                            collectQuest.SetProgress(itemCount);
                            Debug.Log($"Quest progress: {collectQuest.questData.title} - {collectQuest.GetProgressString()}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Count items in inventory by item ID
        /// </summary>
        private int CountItemsInInventory(string itemId)
        {
            if (playerInventory == null) return 0;

            int count = 0;
            foreach (Item item in playerInventory.Items)
            {
                if (item.Id == itemId)
                {
                    count++;
                }
            }
            return count;
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
            Debug.Log($"Quest completed: {quest.questData.title}!");
            
            // Award rewards
            AwardRewards(quest.questData.reward);
            
            // Remove from active quests
            activeQuests.Remove(quest.questData.questType);
            
            OnQuestCompleted?.Invoke(quest);
        }

        /// <summary>
        /// Award quest rewards to the player
        /// </summary>
        private void AwardRewards(QuestReward reward)
        {
            if (reward == null || !reward.HasRewards())
            {
                Debug.Log("No rewards to award.");
                return;
            }

            // Award XP
            if (reward.xp > 0)
            {
                if (playerLevelSystem != null)
                {
                    playerLevelSystem.AddExperience(reward.xp);
                    Debug.Log($"Awarded {reward.xp} XP!");
                }
                else
                {
                    Debug.LogWarning($"QuestManager: Cannot award {reward.xp} XP - playerLevelSystem not assigned");
                }
            }

            // Award items
            if (reward.rewardItems != null && reward.rewardItems.Length > 0)
            {
                if (playerInventory != null)
                {
                    foreach (ItemData itemData in reward.rewardItems)
                    {
                        if (itemData != null)
                        {
                            Item rewardItem = itemData.CreateItem();
                            playerInventory.AddItem(rewardItem);
                            Debug.Log($"Awarded item: {itemData.itemName}");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("QuestManager: Cannot award items - playerInventory not assigned");
                }
            }
        }
    }
}
