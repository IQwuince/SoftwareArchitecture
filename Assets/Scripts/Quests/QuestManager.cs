using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IQwuince.Quests
{
    public class QuestManager : MonoBehaviour
    {
        [Header("Quests")]
        [Tooltip("Sample Kill quest activated with O key")]
        public KillQuestSO testKillQuest;

        [Tooltip("Sample Collect quest activated with P key")]
        public CollectQuestSO testCollectQuest;

        [Header("Inventory Reference")]
        [Tooltip("Reference to the player's inventory")]
        public Inventory playerInventory;

        // Active quests dictionary - one per type
        private Dictionary<QuestType, Quest> activeQuests = new Dictionary<QuestType, Quest>();

        // Progress tracking
        private Dictionary<GameObject, int> enemyKillCounts = new Dictionary<GameObject, int>();

        // Events
        public event Action<Quest> OnQuestActivated;
        public event Action<Quest> OnQuestProgressChanged;
        public event Action<Quest> OnQuestCompleted;

        private void OnEnable()
        {
            // Subscribe to game events
            EnemyHealth.OnEnemyKilledEvent += HandleEnemyKilled;
            Inventory.OnItemPickedUp += HandleItemPickedUp;
        }

        private void OnDisable()
        {
            // Unsubscribe from game events
            EnemyHealth.OnEnemyKilledEvent -= HandleEnemyKilled;
            Inventory.OnItemPickedUp -= HandleItemPickedUp;
        }

        private void Update()
        {
            // Check for sample quest activation keys
            if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
            {

                ActivateQuest(testKillQuest);
            }

            if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
            {
                ActivateQuest(testCollectQuest);
            }
        }

        public bool ActivateQuest(QuestSO questData)
        {

            if (questData == null)
            {
                Debug.LogWarning("Cannot activate quest");
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

        public Quest GetActiveQuest(QuestType type)
        {
            return activeQuests.TryGetValue(type, out Quest quest) ? quest : null;
        }

        public IEnumerable<Quest> GetActiveQuests()
        {
            return activeQuests.Values;
        }

        private void HandleEnemyKilled(GameObject enemyPrefab)
        {
           
            if (enemyPrefab == null)  return;

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
        private void HandleItemPickedUp(string itemName)
        {
            // Check if there's an active collect quest
            if (!activeQuests.TryGetValue(QuestType.Collect, out Quest collectQuest))
                return;

            // Get the collect quest data
            CollectQuestSO collectQuestData = collectQuest.questData as CollectQuestSO;
            if (collectQuestData == null || collectQuestData.itemToCollect == null)
                return;

            // If the player inventory is not assigned, try to find it
            if (playerInventory == null)
            {
                playerInventory = FindPlayerInventory();
                if (playerInventory == null)
                {
                    Debug.LogWarning("QuestManager: Player inventory not found. Cannot track collect quest progress.");
                    return;
                }
            }

            // Get the current count of the required item in the inventory
            int currentCount = playerInventory.GetItemCount(collectQuestData.itemToCollect);

            // Update progress to match inventory count
            if (currentCount > collectQuest.currentProgress)
            {
                int progressToAdd = currentCount - collectQuest.currentProgress;
                collectQuest.IncrementProgress(progressToAdd);
                Debug.Log($"Quest progress: {collectQuest.questData.title} - {collectQuest.GetProgressString()}");
            }
        }

        private Inventory FindPlayerInventory()
        {
            // Try to find via singleton first
            var singleton = UnityEngine.Object.FindFirstObjectByType<SingletonPlayerInventoryController>();
            if (singleton != null && singleton.inventory != null)
            {
                return singleton.inventory;
            }

            // Fallback to finding any Inventory component
            return UnityEngine.Object.FindFirstObjectByType<Inventory>();
        }

        [System.Obsolete("Collect quests now use ItemData and inventory tracking. This method is kept for backward compatibility but has no effect on collect quests.")]
        public void UpdateCollectProgress(GameObject itemPrefab)
        {
            Debug.LogWarning("UpdateCollectProgress(GameObject) is deprecated. Collect quests now use ItemData and inventory tracking.");
        }
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


