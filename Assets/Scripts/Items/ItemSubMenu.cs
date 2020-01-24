using UnityEngine;
using UnityEngine.UI;

public class ItemSubMenu : MonoBehaviour {
    public Text UseText;
    public EquipmentSlots slot;
    public bool fromEquipScreen;

    public bool waitingToConfirmDrop;

    public void ClickUse() {
        if (GameMenu.instance.clickedItem.GetType() == typeof(EquippableItem) 
            || GameMenu.instance.clickedItem.GetType() == typeof(Weapon)
            || GameMenu.instance.clickedItem.GetType() == typeof(Shield)) {
            AudioManager.instance.PlaySfx("click");

            UnequipItem();
            GameManager.instance.UpdateHeroesEquipmentBonuses(GameMenu.instance.currentHeroId, slot);
            InventoryManager.instance.RemoveItemById(GameMenu.instance.clickedItem.id);
            var newEquipment = GameMenu.instance.clickedItem as EquippableItem;
            if ((int)slot < 6) {
                if (GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot] != null) {
                    GameManager.instance.RemoveEquipmentBonus(GameMenu.instance.currentHeroId, slot);
                }
                GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot] = newEquipment;
            } else {
                if (GameMenu.instance.clickedItem.GetType() == typeof(Weapon)) {
                    newEquipment = newEquipment as Weapon;
                } else if (GameMenu.instance.clickedItem.GetType() == typeof(Shield)) {
                    newEquipment = newEquipment as Shield;
                }
                if (GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot-6] != null) {
                }
                GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot-6] = newEquipment as Hands;
            }
            GameManager.instance.UpdateHeroesEquipmentBonuses(GameMenu.instance.currentHeroId, slot);
            GameMenu.instance.Back((fromEquipScreen ? 2 : 1));
            GameMenu.instance.SetEquipment();
        }
        fromEquipScreen = false;
        GameMenu.instance.SetInventory();
    }

    public void ClickRemove() {
        AudioManager.instance.PlaySfx("click");
        UnequipItem();
        if ((int)slot < 6) {
            GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot] = null;
        } else {
            if (slot == EquipmentSlots.MainHand) {
                GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[0] = InventoryManager.instance.GetItemReference("weapUnarmed") as Weapon;
            } else {
                GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot - 6] = null;
            }
        }
        GameManager.instance.UpdateHeroesEquipmentBonuses(GameMenu.instance.currentHeroId, slot);
        GameMenu.instance.Back();
        GameMenu.instance.SetEquipment();
    }

    private void UnequipItem() {
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
                if (oldEquipmentId != "weapUnarmed") {
                    InventoryManager.instance.AddItemById(oldEquipmentId, 1);
                }
            }
        }
    }
}
