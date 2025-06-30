using UnityEngine;

[System.Serializable]
public class EquipmentSlot
{
    public EquipmentType slotType;
    public ItemData equippedItem;

    public EquipmentSlot(EquipmentType type)
    {
        slotType = type;
        equippedItem = null;
    }

    public bool IsEmpty()
    {
        return equippedItem == null;
    }

    public void Clear()
    {
        equippedItem = null;
    }
}

public class PlayerEquipment : MonoBehaviour
{
    [Header("Equipment Slots")]
    [SerializeField] private EquipmentSlot weaponSlot;
    [SerializeField] private EquipmentSlot armorSlot;
    [SerializeField] private EquipmentSlot accessorySlot;

    private PlayerStats playerStats;
    private PlayerInventory playerInventory;

    // Events
    public System.Action<EquipmentType, ItemData> OnItemEquipped;
    public System.Action<EquipmentType, ItemData> OnItemUnequipped;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<PlayerInventory>();

        // Initialize equipment slots
        weaponSlot = new EquipmentSlot(EquipmentType.Weapon);
        armorSlot = new EquipmentSlot(EquipmentType.Armor);
        accessorySlot = new EquipmentSlot(EquipmentType.Accessory);
    }

    public bool EquipItem(ItemData item)
    {
        if (item == null || item.itemType != ItemType.Equipment)
            return false;

        EquipmentSlot targetSlot = GetSlotByType(item.equipmentType);
        if (targetSlot == null) return false;

        // Unequip current item if any
        if (!targetSlot.IsEmpty())
        {
            UnequipItem(item.equipmentType);
        }

        // Remove item from inventory
        if (!playerInventory.RemoveItem(item, 1))
            return false;

        // Equip new item
        targetSlot.equippedItem = item;
        ApplyItemStats(item, true);

        OnItemEquipped?.Invoke(item.equipmentType, item);
        return true;
    }

    public bool UnequipItem(EquipmentType equipmentType)
    {
        EquipmentSlot targetSlot = GetSlotByType(equipmentType);
        if (targetSlot == null || targetSlot.IsEmpty())
            return false;

        ItemData item = targetSlot.equippedItem;

        // Add item back to inventory
        if (!playerInventory.AddItem(item, 1))
            return false; // Inventory full

        // Remove item effects
        ApplyItemStats(item, false);

        // Clear slot
        targetSlot.Clear();

        OnItemUnequipped?.Invoke(equipmentType, item);
        return true;
    }

    private void ApplyItemStats(ItemData item, bool equip)
    {
        if (playerStats == null) return;

        int multiplier = equip ? 1 : -1;

        if (item.healthBonus != 0)
        {
            int newMaxHealth = playerStats.GetMaxHealth() + (item.healthBonus * multiplier);
            playerStats.SetMaxHealth(newMaxHealth);
        }

        if (item.manaBonus != 0)
        {
            int newMaxMana = playerStats.GetMaxMana() + (item.manaBonus * multiplier);
            playerStats.SetMaxMana(newMaxMana);
        }

        // Attack và Defense sẽ được tính toán khi cần thiết thông qua getter methods
    }

    public int GetTotalAttackBonus()
    {
        int total = 0;
        if (!weaponSlot.IsEmpty()) total += weaponSlot.equippedItem.attackBonus;
        if (!armorSlot.IsEmpty()) total += armorSlot.equippedItem.attackBonus;
        if (!accessorySlot.IsEmpty()) total += accessorySlot.equippedItem.attackBonus;
        return total;
    }

    public int GetTotalDefenseBonus()
    {
        int total = 0;
        if (!weaponSlot.IsEmpty()) total += weaponSlot.equippedItem.defenseBonus;
        if (!armorSlot.IsEmpty()) total += armorSlot.equippedItem.defenseBonus;
        if (!accessorySlot.IsEmpty()) total += accessorySlot.equippedItem.defenseBonus;
        return total;
    }

    private EquipmentSlot GetSlotByType(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                return weaponSlot;
            case EquipmentType.Armor:
                return armorSlot;
            case EquipmentType.Accessory:
                return accessorySlot;
            default:
                return null;
        }
    }

    public ItemData GetEquippedItem(EquipmentType type)
    {
        EquipmentSlot slot = GetSlotByType(type);
        return slot?.equippedItem;
    }

    public bool IsSlotEmpty(EquipmentType type)
    {
        EquipmentSlot slot = GetSlotByType(type);
        return slot?.IsEmpty() ?? true;
    }
}
