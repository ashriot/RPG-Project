using UnityEngine;

public class BackButton : MonoBehaviour {

    public GameObject windowToClose;

    public void Click() {
        AudioManager.instance.PlaySfx("click");
        windowToClose.SetActive(false);
        gameObject.SetActive(false);
    }
}
