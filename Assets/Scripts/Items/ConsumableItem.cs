using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Consumable", menuName = "Consumable")]
public class ConsumableItem : Item {

    public int potency;
    public override void Discard() {
        InventoryManager.instance.RemoveItem(id);
    }

    public override void Use(int targetId) {    // or Equip
        return;
    }

    public override string GetStatsString() {
        return null;
    }
}
