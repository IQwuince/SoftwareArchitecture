using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Item Pickup Quest")]
public class ItemPickupQuest : QuestData
{
    public string itemName;
    public int requiredAmount;

    public override bool CheckCompletion(QuestSystem questSystem)
    {
        int collected = questSystem.GetItemCount(itemName);
        isCompleted = collected >= requiredAmount;
        return isCompleted;
    }
}
