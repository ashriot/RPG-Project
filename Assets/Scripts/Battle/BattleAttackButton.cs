using UnityEngine;
using UnityEngine.UI;

public class BattleAttackButton : MonoBehaviour {

    public Text nameText;
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
}
