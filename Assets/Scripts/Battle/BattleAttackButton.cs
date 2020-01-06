using UnityEngine;
using UnityEngine.UI;

public class BattleAttackButton : MonoBehaviour {

    public new string name;
    public string description;
    public Text nameText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Press() {
        var currentTurn = BattleManager.instance.activeBattleCharacters[BattleManager.instance.currentTurnId];

        BattleManager.instance.OpenTargetMenu(name);
    }
}
