using UnityEngine;
using System.Collections.Generic;
using System;
using static UnityEditor.Progress;
using NUnit.Framework.Interfaces;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<ItemData> itemDatas;

    [SerializeReference]
    private List<Item> items;

    public Item[] Items => items.ToArray();

    [SerializeField]
    private ItemSortingStrategy[] itemSortingStrategies;

    [SerializeField]
    private int strategyIndex = 0;

    public static event Action<string> OnItemPickedUp;

    private void Awake()
    {
        GenerateInventory();             // Create items based on itemDatas.
        LoadItemSortingStrategies();     // Find sorting strategies attached as components.
    }

    // Instantiates items based on the item data list.
    private void GenerateInventory()
    {
        items = new List<Item>();
        foreach (ItemData itemData in itemDatas)
        {
            items.Add(itemData.CreateItem()); // Create an item from its data.
        }
    }

    // Adds an item to the inventory.
    public void AddItem(Item item)
    {
        items.Add(item);
        OnItemPickedUp?.Invoke(item.ItemName); // Notify quest system
    }

    // Removes an item from the inventory.
    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    #region "Strategy Pattern Implementation"
    // Loads sorting strategy components from child objects.
    private void LoadItemSortingStrategies()
    {
        itemSortingStrategies = GetComponentsInChildren<ItemSortingStrategy>();
    }

    // Returns the items sorted according to the current strategy.
    public Item[] GetSortedItems()
    {
        // If no sorting strategies exist, return the unsorted list.
        if (itemSortingStrategies.Length == 0)
        {
            return items.ToArray();
        }
        else
        {
            return itemSortingStrategies[strategyIndex].GetSortedItems(items);
        }
    }

    // Sets the current sorting strategy by index.
    public void SetSortingStrategy(int pIndex)
    {
        strategyIndex = pIndex;
    }

    // Cycles to the next sorting strategy (loops back to 0 if at the end).
    public void NextSortingStrategy()
    {
        if (strategyIndex == itemSortingStrategies.Length - 1)
        {
            strategyIndex = 0;
        }
        else
        {
            strategyIndex++;
        }
    }

    // Cycles to the previous sorting strategy (loops to last if at the start).
    public void PreviousSortingStrategy()
    {
        if (strategyIndex == 0)
        {
            strategyIndex = itemSortingStrategies.Length - 1;
        }
        else
        {
            strategyIndex--;
        }
    }

    // Returns the name of the currently selected sorting strategy.
    public string GetCurrentStrategyName()
    {
        return itemSortingStrategies[strategyIndex].StrategyName;
    }
    #endregion
}

