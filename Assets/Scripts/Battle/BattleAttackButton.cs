using UnityEngine;
using UnityEngine.UI;

public class BattleAttackButton : MonoBehaviour {

    public Text nameText;
    public string description;
    public Image image;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Press() {
        // BattleManager.instance.OpenTargetMenu(name);
        BattleManager.instance.HeroAction(nameText.text);
    }

    public void LongPress(){
        Debug.Log("Long press!");
        BattleManager.instance.DisplayTooltip(description);
    }
}
