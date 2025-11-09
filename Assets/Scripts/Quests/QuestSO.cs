using UnityEngine;

namespace IQwuince.Quests
{
    /// <summary>
    /// Base ScriptableObject for quest definitions.
    /// Designers create quest assets from this in the inspector.
    /// </summary>
    public abstract class QuestSO : ScriptableObject
    {
        [Header("Quest Identity")]
        [Tooltip("Unique identifier for this quest")]
        public string id;
        
        [Tooltip("Display name shown in UI")]
        public string title;
        
        [TextArea(3, 5)]
        [Tooltip("Quest description shown to player")]
        public string description;

        [Header("Quest Configuration")]
        [Tooltip("Type of quest (Kill or Collect)")]
        public QuestType questType;
        
        [Tooltip("Reference to target prefab (enemy or item)")]
        public GameObject target;
        
        [Tooltip("How many targets needed to complete")]
        public int targetCount;

        [Header("Rewards")]
        [Tooltip("Rewards given when quest completes")]
        public QuestReward reward;
        
        [Tooltip("Optional icon for UI display")]
        public Sprite icon;

        /// <summary>
        /// Validates the quest data. Called from editor.
        /// </summary>
        public virtual bool Validate(out string error)
        {
            if (string.IsNullOrEmpty(id))
            {
                error = "Quest ID cannot be empty";
                return false;
            }

            if (string.IsNullOrEmpty(title))
            {
                error = "Quest title cannot be empty";
                return false;
            }

            if (target == null)
            {
                error = "Target prefab is required";
                return false;
            }

            if (targetCount <= 0)
            {
                error = "Target count must be greater than 0";
                return false;
            }

            error = null;
            return true;
        }
    }
}
