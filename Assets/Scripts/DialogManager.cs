using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public static DialogManager instance;

    public Text dialogText;
    public Text nameText;
    public GameObject dialogBox;
    public GameObject nameBox;

    public string[] dialogLines;
    public int currentLine;

    private bool justStarted;
    private string questName;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (dialogBox.activeInHierarchy) {
            if (Input.GetButtonUp("Fire1")) {
                if (!justStarted) {
                    currentLine++;
                    if (currentLine >= dialogLines.Length) {
                        dialogBox.SetActive(false);
                        GameManager.instance.dialogActive = false;

                        if (shouldMarkQuest) {
                            shouldMarkQuest = false;
                            if (markQuestComplete) {
                                QuestManager.instance.MarkQuestComplete(questName);
                            } else {
                                QuestManager.instance.MarkQuestIncomplete(questName);
                            }
                        }
                    }
                    else {
                        CheckIfName();
                        dialogText.text = dialogLines[currentLine];
                    }
                }
                else {
                    justStarted = false;
                }
            }
        }
    }

    public void ShowDialog(string[] lines, bool isPerson) {
        dialogLines = lines;
        currentLine = 0;
        
        CheckIfName();
        if (!isPerson) {
            dialogText.alignment = TextAnchor.MiddleCenter;
        } else {
            dialogText.alignment = TextAnchor.UpperLeft;
        }
        
        dialogText.text = dialogLines[currentLine];
        dialogBox.SetActive(true);
        justStarted = true;
        GameManager.instance.dialogActive = true;

        nameBox.SetActive(isPerson);
    }

    public void CheckIfName() {
        if (dialogLines[currentLine].StartsWith("n$")) {
            nameText.text = dialogLines[currentLine].Replace("n$", "");
            currentLine++;
        }
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markComplete) {
        this.questName = questName;
        markQuestComplete = markComplete;

        shouldMarkQuest = true;
    }
}
