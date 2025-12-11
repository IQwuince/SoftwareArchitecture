using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public static Action<ItemSlot> OnAnySlotClicked; // broadcast when a slot is clicked

    // Shared TMP_Text for item description (assign from InventoryUI)
    public static TMP_Text SharedDescriptionText;
    public static event Action OnAnySelectionChanged;


    [Header("Item Data")]
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public TMP_Text ItemNameText;

    [Header("Item Slot")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    [Header("Item Description Slot")]
    public Image itemDescriptionImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemQuantityText;

    [Header("References")]
    private Inventory inventory;
    public Image selectedShader;      // outline image (assign in inspector)
    public bool thisItemSelected;

    // Store the description for this slot
    private string itemDescription;

    private void OnEnable()
    {
        OnAnySlotClicked += HandleAnySlotClicked;
    }

    private void OnDisable()
    {
        OnAnySlotClicked -= HandleAnySlotClicked;
    }

    private void Start()
    {
        inventory = UnityEngine.Object.FindFirstObjectByType<Inventory>();
        Clear();
        // make sure outline is off initially
        if (selectedShader != null) selectedShader.gameObject.SetActive(false);
    }

    // Populate the slot visuals with given data
    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        if (ItemNameText != null) ItemNameText.text = itemName;
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            quantityText.enabled = quantity > 1;
        }
        if (itemImage != null)
        {
            itemImage.sprite = itemSprite;
            itemImage.enabled = itemSprite != null;
        }
    }

    // Clear visuals and reset internal state
    public void Clear()
    {
        itemName = null;
        quantity = 0;
        itemSprite = null;
        itemDescription = null;
        isFull = false;

        if (ItemNameText != null) ItemNameText.text = "";
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }

        // ensure selection outline is off when the slot is cleared
        thisItemSelected = false;
        if (selectedShader != null) selectedShader.gameObject.SetActive(false);
    }

    // Toggle selection visual
    public void SetSelected(bool on)
    {
        thisItemSelected = on;
        if (selectedShader != null)
            selectedShader.gameObject.SetActive(on);
    }

    // Handle clicks
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
            if (isFull)
            {
                Debug.Log($"Clicked on slot: {itemName} (Quantity: {quantity})");
                if (SharedDescriptionText != null)
                    SharedDescriptionText.text = itemDescription;
            }
            else
            {
                if (SharedDescriptionText != null)
                    SharedDescriptionText.text = "";
            }
        }
    }

    private void OnLeftClick()
    {
        // notify everyone which slot was clicked; handler will toggle clicked slot and deselect all others
        OnAnySlotClicked?.Invoke(this);
    }

    // Called for every slot when any slot is clicked
    private void HandleAnySlotClicked(ItemSlot clicked)
    {
        if (clicked == this)
        {
            SetSelected(!thisItemSelected);

            if (!thisItemSelected && SharedDescriptionText != null)
                SharedDescriptionText.text = "";
            else if (thisItemSelected && SharedDescriptionText != null)
                SharedDescriptionText.text = itemDescription;

            // Notify UI to refresh
            OnAnySelectionChanged?.Invoke();
        }
        else
        {
            if (thisItemSelected)
                SetSelected(false);
        }
    }
}
