using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Variables item drop")]
    public ItemData[] itemDrops;           
    [Range(0f, 1f)]
    public float dropChance = 1f;         
    [Tooltip("Random  between min and max")]
    public int minQuantity = 1;
    public int maxQuantity = 1;

    [Header("Variables xp drop")]
    public int XPReward;

    private void GiveItems()
    {

        if (itemDrops == null || itemDrops.Length == 0) return;
        if (Random.value > dropChance) return;

        // pick a random non-null entry (tries up to array length to avoid nulls)
        ItemData chosen = null;
        for (int i = 0; i < itemDrops.Length; i++)
        {
            var candidate = itemDrops[Random.Range(0, itemDrops.Length)];
            if (candidate != null)
            {
                chosen = candidate;
                break;
            }
        }
        if (chosen == null) return;

        int qty = Mathf.Clamp(Random.Range(minQuantity, maxQuantity + 1), 1, 999);

        var controller = SingletonPlayerInventoryController.Instance;
        if (controller == null || controller.inventory == null)
        {
            Debug.LogWarning("[EnemyLoot] No player inventory found to give loot to.");
            return;
        }

        for (int i = 0; i < qty; i++)
        {
            controller.inventory.AddItem(chosen.CreateItem());
        }

        Debug.Log($"[EnemyLoot] Gave {qty} x '{chosen.itemName}' to player inventory.");
    }

    private void GiveXP()
    {
        EventBus.Publish(new LevelSystemAddXpEvent(XPReward));
    }

    public void GiveRewards()
    {
        GiveItems();
        GiveXP();
    }
}