using UnityEngine;

public class DialogActivator : MonoBehaviour {

    public string[] lines;
    public bool isPerson = true;
    public bool shouldActivateQuest;
    public string questName;
    public bool markComplete;

    private bool canActivate;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (canActivate && Input.GetButtonDown("Fire1") && !DialogManager.instance.dialogBox.activeInHierarchy) {
            DialogManager.instance.ShowDialog(lines, isPerson);
            if (!string.IsNullOrEmpty(questName)) {
                DialogManager.instance.ShouldActivateQuestAtEnd(questName, markComplete);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = false;
        }
    }
}
