using System;
using UnityEngine;

namespace IQwuince.Quests
{
    /// <summary>
    /// Defines rewards given upon quest completion.
    /// Supports XP and multiple item rewards.
    /// </summary>
    [Serializable]
    public class QuestReward
    {
        [Tooltip("Experience points awarded")]
        public int xp;

        [Tooltip("Item rewards given to inventory (uses ItemData ScriptableObjects)")]
        public ItemData[] rewardItems;

        public QuestReward()
        {
            xp = 0;
            rewardItems = new ItemData[0];
        }

        public QuestReward(int xp, ItemData[] items = null)
        {
            this.xp = xp;
            this.rewardItems = items ?? new ItemData[0];
        }

        /// <summary>
        /// Returns true if there are any rewards
        /// </summary>
        public bool HasRewards()
        {
            return xp > 0 || (rewardItems != null && rewardItems.Length > 0);
        }
    }
}
