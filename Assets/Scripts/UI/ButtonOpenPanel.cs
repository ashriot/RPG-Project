using UnityEngine;

public class ButtonOpenPanel : MonoBehaviour {

    public BackButton backButton;
    public GameObject panel;

    public void Click() {
        AudioManager.instance.PlaySfx("click");
        panel.SetActive(true);
        backButton.windowToClose = panel;
        backButton.gameObject.SetActive(true);
    }
}
