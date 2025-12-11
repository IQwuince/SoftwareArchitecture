using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Unique id for each item")]
    public string id;

    [Header("Core properties")]
    public string itemName;
    public int attack;
    public int defense;
    public string itemDescription;

    [Header("Visuals")]
    public Sprite itemIcon;
    public GameObject itemModel;

    public Item CreateItem()
    {
        return new Item(this);
    }
}

[Serializable]
public class Item
{
    [Header("Unique id for each item")]
    [SerializeField]
    private string id;
    public string Id => id;//This setup allows access to the private field 'id'
                           //while also allows it to be shown in the inspector

    [Header("Core properties")]
    [SerializeField]
    private string itemName;
    public string ItemName => itemName;
    [SerializeField]
    private int attack;
    public int Attack => attack;
    [SerializeField]
    private int defense;
    public int Defense => defense;
    private string itemDescription;
    public string ItemDescription => itemDescription;

    [Header("Visuals")]
    public Sprite itemIcon;
    public GameObject itemModel;

    public Item(ItemData itemData)
    {
        id = itemData.id;
        itemName = itemData.itemName;
        attack = itemData.attack;
        defense = itemData.defense;
        itemDescription = itemData.itemDescription;

        itemIcon = itemData.itemIcon;
        itemModel = itemData.itemModel;
    }
}

