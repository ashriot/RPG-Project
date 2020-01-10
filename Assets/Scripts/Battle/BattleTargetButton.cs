using UnityEngine;
using UnityEngine.UI;

public class BattleTargetButton : MonoBehaviour {

    public string actionName;
    public int targetId;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    public void Press() {
        GetComponent<EnemyStatWindow>().targetBox.gameObject.SetActive(true);
        // BattleManager.instance.PlayerAttack(actionName);
        BattleManager.instance.SetTargetedEnemyId(targetId);
    }
}
