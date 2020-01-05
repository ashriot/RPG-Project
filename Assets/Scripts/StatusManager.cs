using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour {

    public static StatusManager instance;

    [Header("Object References")]
    public Canvas canvas;
    public EventNotification note;
    public GameObject statsWindow;
    public GameObject actionMenu;
    public Text tooltip;
    public Text keyCount;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void LootNotification(string itemName) {
        var position = Camera.main.WorldToScreenPoint(PlayerController.instance.transform.position);
        // var message = GetArticle(itemName) + itemName + ".";
        Instantiate(note, position, transform.rotation, canvas.transform).GenerateNote(itemName, itemName);
    }

    private string GetArticle(string text) {
        string[] vowels = {"a", "e", "i", "o", "u" };
        var startingLetter = text.Substring(0, 1);
        foreach(var vowel in vowels) {
            if (vowel == startingLetter) {
                return "an ";
            }
        }
        return "a ";
    }
}
