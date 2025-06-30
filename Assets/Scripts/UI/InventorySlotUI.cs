using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Components")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Image backgroundImage;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int slotIndex;
    private InventorySlot currentSlot;
    private PlayerInventory playerInventory;

    private void Start()
    {
        // Tìm player inventory
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
        }
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public void UpdateSlotDisplay(InventorySlot slot)
    {
        currentSlot = slot;

        if (slot == null || slot.IsEmpty())
        {
            // Empty slot
            if (itemIcon != null) itemIcon.sprite = null;
            if (itemIcon != null) itemIcon.color = Color.clear;
            if (quantityText != null) quantityText.text = "";
        }
        else
        {
            // Slot has item
            if (itemIcon != null && slot.item.icon != null)
            {
                itemIcon.sprite = slot.item.icon;
                itemIcon.color = Color.white;
            }

            if (quantityText != null)
            {
                if (slot.quantity > 1 || !slot.item.isStackable)
                {
                    quantityText.text = slot.quantity.ToString();
                }
                else
                {
                    quantityText.text = "";
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.IsEmpty()) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Left click - use item
            if (playerInventory != null)
            {
                playerInventory.UseItem(slotIndex);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Right click - show item info or context menu
            ShowItemTooltip();
        }
    }

    private void ShowItemTooltip()
    {
        if (currentSlot != null && !currentSlot.IsEmpty())
        {
            // Create a simple tooltip
            Debug.Log($"Item: {currentSlot.item.itemName}\\nDescription: {currentSlot.item.description}\\nQuantity: {currentSlot.quantity}");

            // Có thể implement tooltip UI ở đây
        }
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }
    }
}
