using System;
using System.Collections.Generic;
using UnityEngine;
using IQwuince.Quests;
using UnityEngine.InputSystem;


public class QuestManager : MonoBehaviour
{
    [Header("Sample Quests for Testing")]
    [Tooltip("Sample Kill quest activated with O key")]
    public KillQuestSO sampleKillQuest;

    [Tooltip("Sample Collect quest activated with P key")]
    public CollectQuestSO sampleCollectQuest;

    [Header("Inventory Reference")]
    [Tooltip("Reference to the player's inventory for checking collected items")]
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
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            ActivateQuest(sampleKillQuest);
        }

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
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
    /// Checks the inventory count against the quest requirements using ItemData.
    /// </summary>
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

    /// <summary>
    /// Attempts to find the player inventory in the scene
    /// </summary>
    private Inventory FindPlayerInventory()
    {
        // Try to find via singleton first
        var singleton = FindObjectOfType<SingletonPlayerInventoryController>();
        if (singleton != null && singleton.inventory != null)
        {
            return singleton.inventory;
        }

        // Fallback to finding any Inventory component
        return FindObjectOfType<Inventory>();
    }

    /// <summary>
    /// Public API for game systems to notify item collection by GameObject reference.
    /// Note: This method is deprecated for collect quests. Collect quests now use ItemData 
    /// and automatically track progress via the inventory system.
    /// </summary>
    [System.Obsolete("Collect quests now use ItemData and inventory tracking. This method is kept for backward compatibility but has no effect on collect quests.")]
    public void UpdateCollectProgress(GameObject itemPrefab)
    {
        // This method is kept for backward compatibility but is no longer used
        // Collect quests now automatically track progress via inventory system
        Debug.LogWarning("UpdateCollectProgress(GameObject) is deprecated. Collect quests now use ItemData and inventory tracking.");
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

