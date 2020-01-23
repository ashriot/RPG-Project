using UnityEngine;
using UnityEngine.UI;

public class EventNotification : MonoBehaviour {

    public float lifetime = 2f;
    public float moveSpeed = 50f;
    public Text notificationText;
    public Image image;

    private float timer;
    private Color fade;
    private int direction = 1;

    void Start() {
        // TODO: Keep text within camera bounds.
        var halfHeight = Camera.main.orthographicSize;
        var halfWidth = halfHeight * Camera.main.aspect;

        var bottomLeftLimit = PlayerController.instance.groundTilemap.localBounds.min;
        var topRightLimit = PlayerController.instance.groundTilemap.localBounds.max;

        if (transform.position.y > 900f) {
            direction = -1;
        }
        transform.position += new Vector3(0f, direction * 50f, 0f);
    }

    void Update() {
        // Destroy(gameObject, lifetime);
        timer += Time.deltaTime;
        if (timer >= lifetime) {
            fade = notificationText.color;
            fade.a = fade.a /1.1f;
            notificationText.color = fade;

            if(fade.a <=.1) {
                PlayerController.instance.noteCooldown = false;
                Destroy (gameObject);
            }
        }
        transform.position += new Vector3(0f, direction * moveSpeed * Time.deltaTime, 0f);
    }

    public void GenerateLootNote(string itemId) {
        var item = InventoryManager.instance.GetItemReference(itemId);

        image.sprite = item.sprite;
        notificationText.text = item.name;
    }

    public void GenerateNote(string message) {
        image.color = Color.clear;
        notificationText.text = message;
    }
}
