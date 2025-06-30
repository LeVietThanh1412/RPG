using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    Material,
    Quest,
    Misc
}

public enum EquipmentType
{
    Weapon,
    Armor,
    Accessory
}

[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Items/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public ItemType itemType;

    [Header("Stack Settings")]
    public bool isStackable = true;
    public int maxStack = 99;

    [Header("Value")]
    public int sellPrice = 1;
    public int buyPrice = 2;

    [Header("Equipment Settings")]
    public EquipmentType equipmentType;
    public int attackBonus = 0;
    public int defenseBonus = 0;
    public int healthBonus = 0;
    public int manaBonus = 0;

    [Header("Consumable Settings")]
    public int healthRestore = 0;
    public int manaRestore = 0;

    public virtual void UseItem(GameObject user)
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                UseConsumable(user);
                break;
            case ItemType.Equipment:
                EquipItem(user);
                break;
        }
    }

    private void UseConsumable(GameObject user)
    {
        PlayerStats playerStats = user.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            if (healthRestore > 0)
            {
                playerStats.Heal(healthRestore);
            }

            if (manaRestore > 0)
            {
                playerStats.RestoreMana(manaRestore);
            }
        }
    }

    private void EquipItem(GameObject user)
    {
        PlayerEquipment playerEquipment = user.GetComponent<PlayerEquipment>();
        if (playerEquipment != null)
        {
            playerEquipment.EquipItem(this);
        }
    }
}
