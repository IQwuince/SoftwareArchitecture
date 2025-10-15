using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSortingStrategy : MonoBehaviour
{
    [SerializeField]
    protected string strategyName;
    public string StrategyName => strategyName;
    public abstract Item[] GetSortedItems(List<Item> items);
}
