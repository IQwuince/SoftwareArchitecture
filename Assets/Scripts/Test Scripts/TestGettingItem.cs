using UnityEngine;

public class TestGettingItem : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        ItemContainer itemContainer = GetComponent<ItemContainer>();

        if (itemContainer != null)
        {
            Item item = itemContainer.GiveItem();
            Debug.Log("Obtained item: " + item.ItemName);
        }
    }
}
