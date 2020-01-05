using UnityEngine;

public class Door : MonoBehaviour {

    public Sprite opened;
    public bool isLocked = true;
    public bool isClosed = true;

    public void Open() {
        isClosed = false;
        GetComponent<SpriteRenderer>().sprite = opened;
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx("door");
    }

    public void Unlock() {
        isLocked = false;
        isClosed = false;
        GetComponent<SpriteRenderer>().sprite = opened;
        if (AudioManager.instance != null) {
            AudioManager.instance.PlaySfx("chest");
            AudioManager.instance.Mute("blocked");
        }
    }
}
