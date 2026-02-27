using UnityEngine;

public class TestGettingItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        
        if (trigger.CompareTag("Player"))
        {
            Debug.Log("pick up");

            ItemContainer itemContainer = GetComponent<ItemContainer>();

            if (itemContainer != null)
            {
                Item item = itemContainer.GiveItem();
                Debug.Log("Obtained item: " + item.ItemName);
                GameObject.Destroy(gameObject);
            }
        }
    }
}
