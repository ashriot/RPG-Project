using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour {

    public string magicName;
    public string description;
    public int mpCost;
    public Image image;
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
            Debug.Log(currentTurn.name + " doesn't have enough MP. (Cost: " + mpCost + ")");
            BattleManager.instance.battleNotification.text.text = "Not Enough MP!";
            BattleManager.instance.battleNotification.Activate();
            return;
        }

        BattleManager.instance.OpenTargetMenu(magicName);
    }
}
