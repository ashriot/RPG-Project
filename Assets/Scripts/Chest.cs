using UnityEngine;

public class Chest : MonoBehaviour {

    public bool isLocked = false;
    public bool isOpened = false;
    public bool isLooted = false;
    public string[] contents;

    public Sprite opened;
    public Sprite looted;

	public void Unlock() {
        isLocked = false;
        Open();
    }

	public void Open() {
        isOpened = true;
        GetComponent<SpriteRenderer>().sprite = opened;
        PlaySfx("chest");
    }

	public void Loot() {
        isLooted = true;
        GetComponent<SpriteRenderer>().sprite = looted;
        GameMenu.instance.LootNotification(contents[0]);
        InventoryManager.instance.AddItemById(contents[0], 1);
        PlaySfx("loot");
    }

    private void PlaySfx(string name) {
        if (AudioManager.instance != null) {
            AudioManager.instance.Mute("blocked");
            AudioManager.instance.PlaySfx(name);
        }
    }
}
