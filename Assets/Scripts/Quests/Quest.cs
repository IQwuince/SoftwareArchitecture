using System;

namespace IQwuince.Quests
{
    public class Quest
    {
        public QuestSO questData { get; private set; }
        public int currentProgress { get; private set; }
        public bool isCompleted { get; private set; }

        public event Action<Quest> OnProgressChanged;
        public event Action<Quest> OnQuestCompleted;

        public Quest(QuestSO data)
        {
            questData = data;
            currentProgress = 0;
            isCompleted = false;
        }

        public void IncrementProgress(int amount = 1)
        {
            if (isCompleted) return;

            currentProgress += amount;
            OnProgressChanged?.Invoke(this);

            if (currentProgress >= questData.targetCount)
            {
                Complete();
            }
        }

        private void Complete()
        {
            if (isCompleted) return;

            isCompleted = true;
            OnQuestCompleted?.Invoke(this);
        }

        public string GetProgressString()
        {
            return $"{currentProgress}/{questData.targetCount}";
        }
    }
}

