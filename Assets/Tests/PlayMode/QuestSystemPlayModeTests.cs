/*using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using IQwuince.Quests;


public class QuestSystemPlayModeTests
{
    private GameObject questManagerObj;
    private QuestManager questManager;
    private GameObject questUIObj;
    private QuestUIManager questUIManager;
    private TextMeshProUGUI questText;
    private KillQuestSO testKillQuest;
    private CollectQuestSO testCollectQuest;
    private GameObject testEnemyPrefab;
    private GameObject testItemPrefab;

    [SetUp]
    public void Setup()
    {
        // Create test quest manager
        questManagerObj = new GameObject("QuestManager");
        questManager = questManagerObj.AddComponent<QuestManager>();

        // Create test UI
        questUIObj = new GameObject("QuestUI");
        questText = questUIObj.AddComponent<TextMeshProUGUI>();
        questUIManager = questUIObj.AddComponent<QuestUIManager>();
        questUIManager.questManager = questManager;
        questUIManager.questText = questText;

        // Create test enemy prefab
        testEnemyPrefab = new GameObject("TestEnemy");

        // Create test item prefab
        testItemPrefab = new GameObject("TestItem");

        // Create test kill quest
        testKillQuest = ScriptableObject.CreateInstance<KillQuestSO>();
        testKillQuest.id = "test_kill_001";
        testKillQuest.title = "Test Kill Quest";
        testKillQuest.description = "Kill test enemies";
        testKillQuest.questType = QuestType.Kill;
        testKillQuest.target = testEnemyPrefab;
        testKillQuest.targetCount = 5;
        testKillQuest.reward = new QuestReward(100, 0, 50);

        // Create test collect quest
        testCollectQuest = ScriptableObject.CreateInstance<CollectQuestSO>();
        testCollectQuest.id = "test_collect_001";
        testCollectQuest.title = "Test Collect Quest";
        testCollectQuest.description = "Collect test items";
        testCollectQuest.questType = QuestType.Collect;
        testCollectQuest.target = testItemPrefab;
        testCollectQuest.targetCount = 3;
        testCollectQuest.reward = new QuestReward(50, 101, 25);

        // Assign sample quests
        questManager.sampleKillQuest = testKillQuest;
        questManager.sampleCollectQuest = testCollectQuest;
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up
        Object.Destroy(questManagerObj);
        Object.Destroy(questUIObj);
        Object.Destroy(testEnemyPrefab);
        Object.Destroy(testItemPrefab);
        Object.Destroy(testKillQuest);
        Object.Destroy(testCollectQuest);
    }

    [UnityTest]
    public IEnumerator TestQuestActivation()
    {
        // Test activating a kill quest
        bool activated = questManager.ActivateQuest(testKillQuest);
        Assert.IsTrue(activated, "Kill quest should activate successfully");

        var activeQuest = questManager.GetActiveQuest(QuestType.Kill);
        Assert.IsNotNull(activeQuest, "Active kill quest should not be null");
        Assert.AreEqual(testKillQuest, activeQuest.questData, "Active quest data should match");
        Assert.AreEqual(0, activeQuest.currentProgress, "Initial progress should be 0");
        Assert.IsFalse(activeQuest.isCompleted, "Quest should not be completed initially");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestOneQuestPerTypeEnforcement()
    {
        // Activate first kill quest
        bool firstActivation = questManager.ActivateQuest(testKillQuest);
        Assert.IsTrue(firstActivation, "First kill quest should activate");

        // Create second kill quest
        var secondKillQuest = ScriptableObject.CreateInstance<KillQuestSO>();
        secondKillQuest.id = "test_kill_002";
        secondKillQuest.title = "Second Kill Quest";
        secondKillQuest.questType = QuestType.Kill;
        secondKillQuest.target = testEnemyPrefab;
        secondKillQuest.targetCount = 10;

        // Try to activate second kill quest
        bool secondActivation = questManager.ActivateQuest(secondKillQuest);
        Assert.IsFalse(secondActivation, "Second kill quest should fail to activate");

        // Verify first quest is still active
        var activeQuest = questManager.GetActiveQuest(QuestType.Kill);
        Assert.AreEqual(testKillQuest, activeQuest.questData, "First quest should still be active");

        // Collect quest should still work
        bool collectActivation = questManager.ActivateQuest(testCollectQuest);
        Assert.IsTrue(collectActivation, "Collect quest should activate even with kill quest active");

        Object.Destroy(secondKillQuest);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestKillQuestProgress()
    {
        // Activate quest
        questManager.ActivateQuest(testKillQuest);
        var activeQuest = questManager.GetActiveQuest(QuestType.Kill);

        // Simulate kills
        for (int i = 0; i < 3; i++)
        {
            questManager.UpdateKillProgress(testEnemyPrefab);
            yield return null;
        }

        // Check progress
        Assert.AreEqual(3, activeQuest.currentProgress, "Progress should be 3 after 3 kills");
        Assert.IsFalse(activeQuest.isCompleted, "Quest should not be complete yet");

        // Complete quest
        questManager.UpdateKillProgress(testEnemyPrefab);
        questManager.UpdateKillProgress(testEnemyPrefab);
        yield return null;

        Assert.AreEqual(5, activeQuest.currentProgress, "Progress should be 5");
        Assert.IsTrue(activeQuest.isCompleted, "Quest should be completed");

        // Quest should be removed from active quests
        yield return new WaitForSeconds(0.1f);
        var removedQuest = questManager.GetActiveQuest(QuestType.Kill);
        Assert.IsNull(removedQuest, "Completed quest should be removed from active quests");
    }

    [UnityTest]
    public IEnumerator TestCollectQuestProgress()
    {
        // Activate quest
        questManager.ActivateQuest(testCollectQuest);
        var activeQuest = questManager.GetActiveQuest(QuestType.Collect);

        // Simulate collecting items
        questManager.UpdateCollectProgress(testItemPrefab);
        yield return null;
        Assert.AreEqual(1, activeQuest.currentProgress, "Progress should be 1 after 1 collect");

        questManager.UpdateCollectProgress(testItemPrefab);
        yield return null;
        Assert.AreEqual(2, activeQuest.currentProgress, "Progress should be 2 after 2 collects");
        Assert.IsFalse(activeQuest.isCompleted, "Quest should not be complete yet");

        // Complete quest
        questManager.UpdateCollectProgress(testItemPrefab);
        yield return null;

        Assert.AreEqual(3, activeQuest.currentProgress, "Progress should be 3");
        Assert.IsTrue(activeQuest.isCompleted, "Quest should be completed");

        // Quest should be removed from active quests
        yield return new WaitForSeconds(0.1f);
        var removedQuest = questManager.GetActiveQuest(QuestType.Collect);
        Assert.IsNull(removedQuest, "Completed quest should be removed from active quests");
    }

    [UnityTest]
    public IEnumerator TestQuestUIDisplay()
    {
        yield return null; // Wait one frame for UI to initialize

        // Initially should show "No Active Quests"
        Assert.IsTrue(questText.text.Contains("No Active Quests"),
            "UI should show 'No Active Quests' initially");

        // Activate kill quest
        questManager.ActivateQuest(testKillQuest);
        yield return null;

        // UI should show the quest
        Assert.IsTrue(questText.text.Contains("[Kill]"), "UI should show [Kill] type");
        Assert.IsTrue(questText.text.Contains("Test Kill Quest"), "UI should show quest title");
        Assert.IsTrue(questText.text.Contains("0/5"), "UI should show initial progress 0/5");

        // Make progress
        questManager.UpdateKillProgress(testEnemyPrefab);
        yield return null;

        Assert.IsTrue(questText.text.Contains("1/5"), "UI should show updated progress 1/5");

        // Activate collect quest
        questManager.ActivateQuest(testCollectQuest);
        yield return null;

        // UI should show both quests
        Assert.IsTrue(questText.text.Contains("[Kill]"), "UI should show [Kill] quest");
        Assert.IsTrue(questText.text.Contains("[Collect]"), "UI should show [Collect] quest");
        Assert.IsTrue(questText.text.Contains("Test Collect Quest"), "UI should show collect quest title");
        Assert.IsTrue(questText.text.Contains("0/3"), "UI should show collect quest progress");
    }

    [UnityTest]
    public IEnumerator TestQuestCompletion()
    {
        bool questCompleted = false;
        Quest completedQuest = null;

        // Listen for completion event
        questManager.OnQuestCompleted += (quest) =>
        {
            questCompleted = true;
            completedQuest = quest;
        };

        // Activate and complete quest
        questManager.ActivateQuest(testKillQuest);
        var activeQuest = questManager.GetActiveQuest(QuestType.Kill);

        for (int i = 0; i < 5; i++)
        {
            questManager.UpdateKillProgress(testEnemyPrefab);
            yield return null;
        }

        // Wait for completion event
        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(questCompleted, "Quest completion event should fire");
        Assert.IsNotNull(completedQuest, "Completed quest should not be null");
        Assert.AreEqual(testKillQuest, completedQuest.questData, "Completed quest data should match");
        Assert.AreEqual(100, completedQuest.questData.reward.xp, "Reward XP should be 100");
        Assert.AreEqual(50, completedQuest.questData.reward.currency, "Reward currency should be 50");
    }

    [UnityTest]
    public IEnumerator TestInvalidQuestActivation()
    {
        // Create invalid quest (missing target)
        var invalidQuest = ScriptableObject.CreateInstance<KillQuestSO>();
        invalidQuest.id = "invalid_001";
        invalidQuest.title = "Invalid Quest";
        invalidQuest.questType = QuestType.Kill;
        invalidQuest.target = null; // Missing target
        invalidQuest.targetCount = 5;

        // Try to activate
        bool activated = questManager.ActivateQuest(invalidQuest);
        Assert.IsFalse(activated, "Invalid quest should not activate");

        var activeQuest = questManager.GetActiveQuest(QuestType.Kill);
        Assert.IsNull(activeQuest, "No quest should be active");

        Object.Destroy(invalidQuest);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestQuestProgressEvents()
    {
        int progressChangeCount = 0;

        // Listen for progress events
        questManager.OnQuestProgressChanged += (quest) =>
        {
            progressChangeCount++;
        };

        questManager.ActivateQuest(testKillQuest);

        // Make progress
        questManager.UpdateKillProgress(testEnemyPrefab);
        yield return null;
        Assert.AreEqual(1, progressChangeCount, "Progress event should fire once");

        questManager.UpdateKillProgress(testEnemyPrefab);
        yield return null;
        Assert.AreEqual(2, progressChangeCount, "Progress event should fire twice");
    }
}
*/