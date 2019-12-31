using UnityEngine;

public class QuestManager : MonoBehaviour {

    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Start() {
        instance = this;

        questMarkersComplete = new bool[questMarkerNames.Length];
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            MarkQuestComplete("Testing");
        }

        // if (Input.GetKeyDown(KeyCode.O)) {
        //     SaveQuestData();
        // }

        // if (Input.GetKeyDown(KeyCode.P)) {
        //     LoadQuestData();
        // }
    }

    public int GetQuestId(string questName) {
        for (var i = 0; i < questMarkerNames.Length; i++) {
            if (questMarkerNames[i] == questName) {
                return i;
            }
        }

        Debug.LogError("Quest '" + questName + "' does not exist!");
        return 0;
    }

    public bool CheckIfComplete(string questName) {
        var questId = GetQuestId(questName);
        if (questId != 0) {
            return questMarkersComplete[questId];
        }

        return false;
    }

    public void MarkQuestComplete(string questName) {
        var questId = GetQuestId(questName);
        if (questId != 0) {
            questMarkersComplete[questId] = true;
        } else {
            Debug.LogError("Quest '" + questName + "' does not exist!");
        }
        UpdateLocalQuestObject();
    }

    public void MarkQuestIncomplete(string questName) {
        var questId = GetQuestId(questName);
        if (questId != 0) {
            questMarkersComplete[questId] = false;
        } else {
            Debug.LogError("Quest '" + questName + "' does not exist!");
        }
        UpdateLocalQuestObject();
    }

    public void UpdateLocalQuestObject() {
        var questObjects = FindObjectsOfType<QuestObjectActivator>();

        if (questObjects.Length > 0) {
            foreach (var quest in questObjects) {
                quest.CheckCompletion();
            }
        }
    }

    public void SaveQuestData() {
        for (var i = 0; i < questMarkerNames.Length; i++) {
            PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], questMarkersComplete[i] ? 1 : 0);
        }
    }

    public void LoadQuestData() {
        for (var i = 0; i < questMarkerNames.Length; i++) {
            var valueToSet = 0;
            var questName = "QuestMarker_" + questMarkerNames[i];

            if (PlayerPrefs.HasKey(questName)) {
                valueToSet = PlayerPrefs.GetInt(questName);
            }

            questMarkersComplete[i] = (valueToSet == 1) ? true : false;
        }
    }
}
