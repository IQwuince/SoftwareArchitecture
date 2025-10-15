using UnityEngine;
[RequireComponent(typeof(Inventory))]

public class SingletonPlayerInventoryController : MonoBehaviour
{
    public Inventory inventory { get; private set; }
    public static SingletonPlayerInventoryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
        if (inventory != null)
        {
            ItemContainer.onGetItem += inventory.AddItem;
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            ItemContainer.onGetItem -= inventory.AddItem;
        }
    }
}
