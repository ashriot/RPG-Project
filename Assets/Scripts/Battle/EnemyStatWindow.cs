using UnityEngine;
using UnityEngine.UI;

public class EnemyStatWindow : MonoBehaviour {

    public Image portrait;
    public Animator animator;
    public Image hpSlider;
    public Image mpSlider;
    public Image targetBox;
    public int targetId;

    public void Press() {
        GetComponent<EnemyStatWindow>().targetBox.gameObject.SetActive(true);
        // BattleManager.instance.PlayerAttack(actionName);
        BattleManager.instance.SetTargetedEnemyId(targetId);
    }
}
