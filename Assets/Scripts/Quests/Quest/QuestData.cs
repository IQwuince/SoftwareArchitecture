using UnityEngine;

namespace IQwuince.Quests
{
    public abstract class QuestData : ScriptableObject
    {
        public string questName;
        [TextArea] public string description;
        public bool isCompleted;

        public abstract bool CheckCompletion(QuestSystem questSystem);
    }
}
