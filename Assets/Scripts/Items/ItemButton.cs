using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour {

    public Image buttonImage;
    public Text nameText;
    public Text quantityText;
    public int buttonId;
    public Item item;
    public ButtonLongPress buttonLongPress;

    public void Press() {
        GameMenu.instance.ClickItem(this);
    }
}
