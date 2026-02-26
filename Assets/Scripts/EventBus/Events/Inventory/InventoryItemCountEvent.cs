using UnityEngine;

public class InventoryItemCountEvent : Event
{
    public Inventory Inventory { get; }
    public int itemCountT;

    public InventoryItemCountEvent(int itemCountE)
    {
        itemCountT = itemCountE;
    }

}
