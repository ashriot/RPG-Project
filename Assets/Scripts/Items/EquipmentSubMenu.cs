using System.Linq;
using UnityEngine;

public class EquipmentSubMenu : MonoBehaviour
{
    public void ClickAutoEquip() {
        AudioManager.instance.PlaySfx("click");

        var hero = GameManager.instance.heroes[GameMenu.instance.currentHeroId];
        for(var i = 0; i < (int)EquipmentSlots.Count - 2; i++) {
            if (hero.bodyEquipment[i] == null) {
                Debug.Log((EquipmentSlots)i + " is empty!");
                // if (InventoryManager.instance.inventory.Where(i => i.itemType == ItemTypes.Arms))
                // GameManager.instance.UpdateHeroesEquipmentBonuses(GameMenu.instance.currentHeroId, slot);
            }
        }
        for (var i = 0; i < 2; i++) {
            if (hero.handEquipment[i] == null || hero.handEquipment[i].name == "Unarmed") {
                Debug.Log((EquipmentSlots)(i+6) + " is empty!");
                // GameManager.instance.UpdateHeroesEquipmentBonuses(GameMenu.instance.currentHeroId, slot);
            }
        }
        // GameMenu.instance.Back(2);
        // GameMenu.instance.SetEquipment();
    }

    public void ClickRemoveAll() {
        AudioManager.instance.PlaySfx("click");
        var hero = GameManager.instance.heroes[GameMenu.instance.currentHeroId];
        for (var i = 0; i < (int)EquipmentSlots.Count - 2; i++) {
            UnequipItem((EquipmentSlots)i);
        }
        GameMenu.instance.Back();
        GameMenu.instance.SetEquipment();
    }

    private void UnequipItem(EquipmentSlots slot) {
        var oldEquipmentId = string.Empty;
        if ((int)slot < 6) {
            if (GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot] != null) {
                oldEquipmentId = GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot].id;
                GameManager.instance.RemoveEquipmentBonus(GameMenu.instance.currentHeroId, slot);
                InventoryManager.instance.AddItemById(oldEquipmentId, 1);
            }
        }
        else {
            if (GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot - 6] != null) {
                oldEquipmentId = GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot - 6].id;
                GameManager.instance.RemoveEquipmentBonus(GameMenu.instance.currentHeroId, slot);
                InventoryManager.instance.AddItemById(oldEquipmentId, 1);
            }
        }
    }
}
