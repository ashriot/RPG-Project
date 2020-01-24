using UnityEngine;

public class BaseWindow : MonoBehaviour {
    public GameObject window;
    public GameObject subMenu;

    public void ToggleWindow(bool value) {
        window.SetActive(value);
        subMenu.SetActive(value);
    }
}
