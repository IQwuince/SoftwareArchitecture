using TMPro;
using UnityEngine;
using System.Text;
using IQwuince.Quests;
using System.Collections.Generic;



public class QuestUIManager : MonoBehaviour
{
    [Tooltip("Assign QuestManager (runtime)")]
    public QuestManager questManager;

    [Tooltip("Single TMP text used for both title and progress display")]
    public TextMeshProUGUI questText;

    [Tooltip("How long (seconds) to show a completed quest after it finishes")]
    public float completedDisplayDuration = 3f;

    private class CompletedEntry
    {
        public QuestType type;
        public string title;
        public float expiryTime;
    }

    // cache keyed by quest type so slot ordering is stable
    private readonly Dictionary<QuestType, CompletedEntry> completedCache = new Dictionary<QuestType, CompletedEntry>();

    // fixed display order: keep Kill first then Collect
    private static readonly QuestType[] displayOrder = new[] { QuestType.Kill, QuestType.Collect };

    private void OnEnable()
    {
        if (questManager != null)
        {
            questManager.OnQuestActivated += OnQuestChanged;
            questManager.OnQuestProgressChanged += OnQuestChanged;
            questManager.OnQuestCompleted += OnQuestCompleted;
        }
    }

    private void OnDisable()
    {
        if (questManager != null)
        {
            questManager.OnQuestActivated -= OnQuestChanged;
            questManager.OnQuestProgressChanged -= OnQuestChanged;
            questManager.OnQuestCompleted -= OnQuestCompleted;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        // Cleanup expired completed entries and update UI if anything removed
        if (completedCache.Count == 0) return;

        float now = Time.time;
        List<QuestType> toRemove = null;
        foreach (var kv in completedCache)
        {
            if (kv.Value.expiryTime <= now)
            {
                if (toRemove == null) toRemove = new List<QuestType>();
                toRemove.Add(kv.Key);
            }
        }

        if (toRemove != null)
        {
            foreach (var k in toRemove) completedCache.Remove(k);
            UpdateUI();
        }
    }

    private void OnQuestChanged(Quest quest)
    {
        UpdateUI();
    }

    private void OnQuestCompleted(Quest quest)
    {
        if (quest?.questData != null)
        {
            var type = quest.questData.questType;
            completedCache[type] = new CompletedEntry
            {
                type = type,
                title = quest.questData.title ?? "Completed Quest",
                expiryTime = Time.time + completedDisplayDuration
            };
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (questText == null)
        {
            Debug.LogWarning("QuestUIManager: questText is not assigned.");
            return;
        }

        if (questManager == null)
        {
            questText.text = "No Quest Manager\nPress O - Activate Kill Quest\nPress P - Activate Collect Quest";
            return;
        }

        StringBuilder sb = new StringBuilder();
        bool hasAnyLine = false;

        // For each slot in a fixed order, display either active quest, cached completed, or nothing
        foreach (var qType in displayOrder)
        {
            Quest active = questManager.GetActiveQuest(qType);

            string line = null;
            if (active != null && active.questData != null)
            {
                string title = active.questData.title ?? "Unnamed Quest";
                if (active.isCompleted)
                {
                    line = $"{title} : quest completed";
                }
                else
                {
                    int current = active.currentProgress;
                    int target = active.questData.targetCount;
                    line = $"{title} : {current}/{target}";
                }
            }
            else if (completedCache.TryGetValue(qType, out var completed))
            {
                line = $"{completed.title} : quest completed";
            }

            if (!string.IsNullOrEmpty(line))
            {
                if (hasAnyLine) sb.AppendLine();
                sb.Append(line);
                hasAnyLine = true;
            }
        }

        if (!hasAnyLine)
        {
            sb.AppendLine("No Active Quests");
            sb.AppendLine("Press O - Activate Kill Quest");
            sb.AppendLine("Press P - Activate Collect Quest");
        }

        questText.text = sb.ToString();
    }
}


