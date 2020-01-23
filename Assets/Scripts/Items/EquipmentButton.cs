using UnityEngine;
using UnityEngine.UI;

public class EquipmentButton : MonoBehaviour {

    public string id;
    public Image image;
    public Text text;
    public ItemTypes itemType;
    public EquippableItem equippedItem;
    public EquipmentSlots slot;

    public ButtonLongPress buttonLongPress;

    public void Press() {
        GameMenu.instance.ClickEquipment(this);
    }
}
