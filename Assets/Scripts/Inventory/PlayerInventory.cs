using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    public InventorySlot(ItemData newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
    }

    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private InventorySlot[] inventorySlots;

    // Events
    public System.Action<int> OnInventoryChanged;
    public System.Action<ItemData> OnItemAdded;
    public System.Action<ItemData> OnItemRemoved;

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        inventorySlots = new InventorySlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots[i] = new InventorySlot();
        }
    }

    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null) return false;

        // Tìm slot có cùng item để stack
        if (item.isStackable)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == item)
                {
                    int availableSpace = item.maxStack - inventorySlots[i].quantity;
                    int amountToAdd = Mathf.Min(quantity, availableSpace);

                    inventorySlots[i].quantity += amountToAdd;
                    quantity -= amountToAdd;

                    OnInventoryChanged?.Invoke(i);
                    OnItemAdded?.Invoke(item);

                    if (quantity <= 0) return true;
                }
            }
        }

        // Tìm slot trống
        while (quantity > 0)
        {
            int emptySlot = FindEmptySlot();
            if (emptySlot == -1) return false; // Inventory đầy

            int amountToAdd = item.isStackable ? Mathf.Min(quantity, item.maxStack) : 1;
            inventorySlots[emptySlot] = new InventorySlot(item, amountToAdd);
            quantity -= amountToAdd;

            OnInventoryChanged?.Invoke(emptySlot);
            OnItemAdded?.Invoke(item);
        }

        return true;
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        if (item == null) return false;

        int totalFound = 0;

        // Đếm tổng số item có trong inventory
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == item)
            {
                totalFound += inventorySlots[i].quantity;
            }
        }

        if (totalFound < quantity) return false;

        // Remove items
        int remainingToRemove = quantity;
        for (int i = 0; i < inventorySlots.Length && remainingToRemove > 0; i++)
        {
            if (inventorySlots[i].item == item)
            {
                int amountToRemove = Mathf.Min(remainingToRemove, inventorySlots[i].quantity);
                inventorySlots[i].quantity -= amountToRemove;
                remainingToRemove -= amountToRemove;

                if (inventorySlots[i].quantity <= 0)
                {
                    inventorySlots[i].Clear();
                }

                OnInventoryChanged?.Invoke(i);
                OnItemRemoved?.Invoke(item);
            }
        }

        return true;
    }

    public bool HasItem(ItemData item, int quantity = 1)
    {
        if (item == null) return false;

        int totalFound = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == item)
            {
                totalFound += inventorySlots[i].quantity;
                if (totalFound >= quantity) return true;
            }
        }

        return false;
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;

        int count = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == item)
            {
                count += inventorySlots[i].quantity;
            }
        }

        return count;
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return;
        if (inventorySlots[slotIndex].IsEmpty()) return;

        ItemData item = inventorySlots[slotIndex].item;

        if (item.itemType == ItemType.Consumable)
        {
            // Use the item
            item.UseItem(gameObject);

            // Remove one from inventory
            inventorySlots[slotIndex].quantity--;
            if (inventorySlots[slotIndex].quantity <= 0)
            {
                inventorySlots[slotIndex].Clear();
            }

            OnInventoryChanged?.Invoke(slotIndex);
        }
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsEmpty())
            {
                return i;
            }
        }
        return -1;
    }

    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < inventorySlots.Length)
        {
            return inventorySlots[index];
        }
        return null;
    }

    public int GetInventorySize() => inventorySize;
    public InventorySlot[] GetAllSlots() => inventorySlots;
}
