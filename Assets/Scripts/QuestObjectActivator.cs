using UnityEngine;

public class QuestObjectActivator : MonoBehaviour {

    public GameObject questObject;
    public string questToCheck;
    public bool activeIfComplete;
    private bool initialCheckDone;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (!initialCheckDone) {
            initialCheckDone = true;

            CheckCompletion();
        }
    }

    public void CheckCompletion() {
        if (QuestManager.instance.CheckIfComplete(questToCheck)) {
            questObject.SetActive(activeIfComplete);
        }
    }
}
