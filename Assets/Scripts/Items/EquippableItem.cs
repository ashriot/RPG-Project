using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Equipment", menuName = "Equipment")]
public class EquippableItem : Item {

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

    public override string GetStatsString() {
        var result = string.Empty;

        if (hpBonus > 0) {
            result += "\nHP+" + hpBonus;
        }
        if (mpBonus > 0) {
            result += "\nMP+" + mpBonus;
        }
        if (attackBonus > 0) {
            result += "\nATK+" + attackBonus;
        }
        if (defenseBonus > 0) {
            result += "\nDEF+" + defenseBonus;
        }
        if (magicBonus > 0) {
            result += "\nMAG+" + magicBonus;
        }
        if (speedBonus > 0) {
            result += "\nSPD+" + speedBonus;
        }
        if (armorBonus > 0) {
            result += "\nARM+" + armorBonus;
        }
        if (resistBonus > 0) {
            result += "\nRES+" + resistBonus;
        }

        return result;
    }
}
