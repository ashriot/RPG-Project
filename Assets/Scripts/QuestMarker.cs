using UnityEngine;

public class QuestMarker : MonoBehaviour {

    public string questName;
    public bool markComplete;
    public bool markOnEnter;
    public bool deactivateOnMark;
    
    private bool canMark;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (canMark && Input.GetButtonDown("Fire1")) {
            canMark = false;
            MarkQuestComplete();
        }
    }

    public void MarkQuestComplete() {
        if (markComplete) {
            QuestManager.instance.MarkQuestComplete(questName);
        } else {
            QuestManager.instance.MarkQuestIncomplete(questName);
        }

        gameObject.SetActive(!deactivateOnMark);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (markOnEnter) {
                MarkQuestComplete();
            } else {
                canMark = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canMark = false;
        }
    }
}
