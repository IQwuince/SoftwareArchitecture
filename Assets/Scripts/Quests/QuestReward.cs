using System;

namespace IQwuince.Quests
{
    [Serializable]
    public struct QuestReward
    {
        public int xp;
        public int itemId;
        public int currency;

        public QuestReward(int xp = 0, int itemId = 0, int currency = 0)
        {
            this.xp = xp;
            this.itemId = itemId;
            this.currency = currency;
        }
    }
}
