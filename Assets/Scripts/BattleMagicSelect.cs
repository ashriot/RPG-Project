using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour {

    public string magicName;
    public int mpCost;
    public Text nameText;
    public Text costText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Press() {
        var currentTurn = BattleManager.instance.activeBattleCharacters[BattleManager.instance.currentTurnId];

        if (currentTurn.currentMp < mpCost) {
            Debug.Log(currentTurn.characterName + " doesn't have enough MP. (Cost: " + mpCost + ")");
            BattleManager.instance.battleNotification.text.text = "Not Enough MP!";
            BattleManager.instance.battleNotification.Activate();
            BattleManager.instance.magicMenu.SetActive(false);
            return;
        }

        BattleManager.instance.magicMenu.SetActive(false);
        BattleManager.instance.OpenTargetMenu(magicName);

        currentTurn.currentMp -= mpCost;
    }
}
