using UnityEngine;

public class Description : MonoBehaviour {

    public string description;

    public void LongPress(){
        // Debug.Log("Long press!");
        GameMenu.instance.ShowTooltip(description);
    }

    public void LongPressRelease() {
        // Debug.Log("Long press release!");
        GameMenu.instance.HideTooltip();
    }
}
