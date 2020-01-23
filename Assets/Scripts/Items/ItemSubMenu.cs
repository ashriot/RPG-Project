using UnityEngine;
using UnityEngine.UI;

public class ItemSubMenu : MonoBehaviour {
    public Text UseText;
    public EquipmentSlots slot;

    public bool waitingToConfirmDrop;

    public void ClickUse() {
        if (GameMenu.instance.clickedItem.GetType() == typeof(EquippableItem) 
            || GameMenu.instance.clickedItem.GetType() == typeof(Weapon)
            || GameMenu.instance.clickedItem.GetType() == typeof(Shield)) {
            AudioManager.instance.PlaySfx("click");

            InventoryManager.instance.RemoveItemById(GameMenu.instance.clickedItem.id);
            var newEquipment = GameMenu.instance.clickedItem as EquippableItem;
            var oldEquipment = ScriptableObject.CreateInstance<EquippableItem>();
            if ((int)slot < 6) {
                if (GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot] != null) {
                    oldEquipment = GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot];
                    GameManager.instance.RemoveEquipmentBonus(GameMenu.instance.currentHeroId, slot);
                    InventoryManager.instance.AddItem(oldEquipment.id, 1);
                }
                GameManager.instance.heroes[GameMenu.instance.currentHeroId].bodyEquipment[(int)slot] = newEquipment;
            } else {
                if (GameMenu.instance.clickedItem.GetType() == typeof(Weapon)) {
                    newEquipment = newEquipment as Weapon;
                } else if (GameMenu.instance.clickedItem.GetType() == typeof(Shield)) {
                    newEquipment = newEquipment as Shield;
                }
                if (GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot-6] != null) {
                    oldEquipment = GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot-6];
                    InventoryManager.instance.AddItem(oldEquipment.id, 1);
                }
                GameManager.instance.heroes[GameMenu.instance.currentHeroId].handEquipment[(int)slot-6] = newEquipment as Hands;
            }
            
            GameManager.instance.UpdateHeroesEquipmentBonuses(GameMenu.instance.currentHeroId, slot);
            GameMenu.instance.Back();
            GameMenu.instance.OpenEquipment();
        }
    }
}
