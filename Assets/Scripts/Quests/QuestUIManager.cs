using TMPro;
using UnityEngine;
using System.Text;

namespace IQwuince.Quests
{
    /// <summary>
    /// Manages quest UI display. Shows all active quests and their progress.
    /// Format: "[Type] Title: X/Y"
    /// </summary>
    public class QuestUIManager : MonoBehaviour
    {
        [Tooltip("Reference to QuestManager")]
        public QuestManager questManager;
        
        [Tooltip("TextMeshPro text component for displaying quests")]
        public TextMeshProUGUI questText;

        private void OnEnable()
        {
            if (questManager != null)
            {
                questManager.OnQuestActivated += OnQuestChanged;
                questManager.OnQuestProgressChanged += OnQuestChanged;
                questManager.OnQuestCompleted += OnQuestChanged;
            }
        }

        private void OnDisable()
        {
            if (questManager != null)
            {
                questManager.OnQuestActivated -= OnQuestChanged;
                questManager.OnQuestProgressChanged -= OnQuestChanged;
                questManager.OnQuestCompleted -= OnQuestChanged;
            }
        }

        private void Start()
        {
            UpdateUI();
        }

        private void OnQuestChanged(Quest quest)
        {
            UpdateUI();
        }

        /// <summary>
        /// Update UI to show all active quests
        /// </summary>
        private void UpdateUI()
        {
            if (questText == null)
            {
                Debug.LogWarning("Quest text component not assigned");
                return;
            }

            if (questManager == null)
            {
                questText.text = "No Quest Manager";
                return;
            }

            var activeQuests = questManager.GetActiveQuests();
            
            StringBuilder sb = new StringBuilder();
            bool hasQuests = false;

            foreach (var quest in activeQuests)
            {
                if (hasQuests)
                    sb.AppendLine();

                string typeLabel = quest.questData.questType == QuestType.Kill ? "Kill" : "Collect";
                sb.Append($"[{typeLabel}] {quest.questData.title}: {quest.GetProgressString()}");
                hasQuests = true;
            }

            questText.text = hasQuests ? sb.ToString() : "No Active Quests\n\nPress O - Activate Kill Quest\nPress P - Activate Collect Quest";
        }
    }
}
