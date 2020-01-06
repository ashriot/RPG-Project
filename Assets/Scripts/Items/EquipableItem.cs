using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Equipment", menuName = "Equipment")]
public class EquipableItem : Item {

    public EquipmentType equipmentType;
    public int hpBonus;
    public int mpBonus;
    public int attackBonus;
    public int defenseBonus;
    public int magicBonus;
    public int speedBonus;
    public int armorBonus;
    public int resistBonus;
    public int criticalIntensityBonus;

    public override void Discard() {
        InventoryManager.instance.RemoveItem(id);
    }

    public override void Use(int targetId) {    // or Equip
        switch (itemType) {
            case ItemType.Arms:
                break;
            case ItemType.Body:
                break;
            case ItemType.Feet:
                break;
            case ItemType.Finger:
                break;
            case ItemType.Hands:
                break;
            case ItemType.Head:
                break;
            case ItemType.Quest:
                break;
            default:
                Debug.LogError("Item type '" + itemType + "' does not exist!");
                break;
        }
    }
}
