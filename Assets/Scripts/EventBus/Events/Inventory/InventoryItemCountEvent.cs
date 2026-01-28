using UnityEngine;

public class InventoryItemCountEvent : IGameEvent
{
    public Inventory Inventory { get; }
    public int itemCountT;

    public InventoryItemCountEvent(int itemCountE)
    {
        itemCountT = itemCountE;
    }

}
