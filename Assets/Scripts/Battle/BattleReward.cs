using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour {

    public static BattleReward instance;

    public GameObject rewardScreen;

    public Text XpText;
    public Text ItemsText;

    public string[] itemRewards;
    public int xpReward;

    public bool markQuestComplete;
    public string questNameToComplete;

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OpenWindow(int xpEarned, string[] itemsReceived) {
        xpReward = xpEarned;
        itemRewards = itemsReceived;

        XpText.text = "Each party member received " + xpEarned.ToString("N0") + " XP!";
        ItemsText.text = string.Empty;

        for (var i = 0; i < itemsReceived.Length; i++) {
            ItemsText.text += itemsReceived[i] + "\n\n";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseWindow() {
        for (var i = 0; i < GameManager.instance.heroes.Length; i++) {
            if (GameManager.instance.heroes[i].gameObject.activeInHierarchy) {
                GameManager.instance.heroes[i].AddXp(xpReward);
            }
        }

        for (var i = 0; i < itemRewards.Length; i++) {
            InventoryManager.instance.AddItem(itemRewards[i]);
        }

        rewardScreen.SetActive(false);
        GameManager.instance.battleActive = false;

        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questNameToComplete);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(BattleReward))]
public class BattleRewardEditor : Editor {
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        var manager = (BattleReward)target;

        if(GUILayout.Button("Generate rewards.")) {
            manager.OpenWindow(75, new string[] { "Iron Sword", "Iron Armor" });
        }
    }
}

#endif