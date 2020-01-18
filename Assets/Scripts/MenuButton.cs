using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {

    public Image buttonImage;
    public Text nameText;
    public string description;
    public int buttonId;

    public void Press() {
        GameMenu.instance.ClickMenuButton(buttonId);
    }

    public void LongPress(){
        Debug.Log("Long press!");
        GameMenu.instance.ShowTooltip(description);
    }

    public void LongPressRelease() {
        Debug.Log("Long press release!");
        GameMenu.instance.HideTooltip();
    }
}
