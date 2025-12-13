using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public static Action<ItemSlot> OnAnySlotClicked;
    public static TMP_Text SharedDescriptionText;

    [Header("Item Data")]
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string ItemId; // Store the item's ID
    public TMP_Text ItemNameText;

    [Header("Item Slot")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    [Header("Item Description Slot")]
    // These are no longer used for the shared description panel, keep only if needed per-slot.
    public Image itemDescriptionImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemQuantityText;

    [Header("References")]
    private Inventory inventory;
    public Image selectedShader; // outline image (assign in inspector)
    public bool thisItemSelected;
    private string itemDescription;

    private void OnEnable()
    {
        // ensure slot starts deselected
        SetSelected(false);
    }

    private void OnDisable()
    {
        SetSelected(false);
    }

    private void Start()
    {
        SetSelected(false);
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, string itemId)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        this.ItemId = itemId;
        isFull = true;

        if (ItemNameText != null) ItemNameText.text = itemName;
        if (quantityText != null) quantityText.text = quantity.ToString();
        if (itemImage != null)
        {
            itemImage.sprite = itemSprite;
            itemImage.enabled = true;
        }
    }

    public void Clear()
    {
        itemName = "";
        quantity = 0;
        itemSprite = null;
        itemDescription = "";
        ItemId = "";
        isFull = false;

        if (ItemNameText != null) ItemNameText.text = "";
        if (quantityText != null) quantityText.text = "";
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
        SetSelected(false);
    }

    public void SetSelected(bool on)
    {
        thisItemSelected = on;

        // Use GameObject active state for the shader so showing/hiding works reliably
        if (selectedShader != null)
            selectedShader.gameObject.SetActive(on);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // update shared description immediately for feedback
        OnLeftClick();

        // notify UI / controller about the click so it can manage selection exclusivity
        OnAnySlotClicked?.Invoke(this);
    }

    private void OnLeftClick()
    {
        if (SharedDescriptionText != null)
            SharedDescriptionText.text = isFull ? itemDescription : "";
    }
}