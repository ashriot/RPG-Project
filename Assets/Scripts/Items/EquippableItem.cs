using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Equipment", menuName = "Equipment")]
public class EquippableItem : Item {

    public EquipmentTypes equipmentType;
	
	[LabeledArray(typeof(Stats))]
    public int[] statBonuses = new int[(int)Stats.Count];

    public override void Discard() {
        InventoryManager.instance.RemoveItemById(id);
    }

    public override void Use(int targetId) {    // or Equip
        //
    }

    public override string GetStatsString() {
        var result = string.Empty;

        if (statBonuses[(int)Stats.Hp] > 0) {
            result += "\nHP+" + statBonuses[(int)Stats.Hp];
        }
        if (statBonuses[(int)Stats.Mp] > 0) {
            result += "\nMP+" + statBonuses[(int)Stats.Mp];
        }
        if (statBonuses[(int)Stats.Attack] > 0) {
            result += "\nATK+" + statBonuses[(int)Stats.Attack];
        }
        if (statBonuses[(int)Stats.Defense] > 0) {
            result += "\nDEF+" + statBonuses[(int)Stats.Defense];
        }
        if (statBonuses[(int)Stats.Magic] > 0) {
            result += "\nMAG+" + statBonuses[(int)Stats.Magic];
        }
        if (statBonuses[(int)Stats.Speed] > 0) {
            result += "\nSPD+" + statBonuses[(int)Stats.Speed];
        }
        if (statBonuses[(int)Stats.Armor] > 0) {
            result += "\nARM+" + statBonuses[(int)Stats.Armor];
        }
        if (statBonuses[(int)Stats.Resist] > 0) {
            result += "\nRES+" + statBonuses[(int)Stats.Resist];
        }

        return result;
    }
}
