using UnityEngine;
using UnityEngine.UI;

public class EnemyStatWindow : MonoBehaviour {

    public Image portrait;
    public Animator animator;
    public Image hpSlider;
    public Image tempHpSlider;
    public Image mpSlider;
    public Image targetCursor;
    public Image enemyIcon;
    public Image actionIcon;
    public Image targetIcon;
    public int targetId;

    public void Press() {
        GetComponent<EnemyStatWindow>().targetCursor.gameObject.SetActive(true);
        // BattleManager.instance.PlayerAttack(actionName);
        BattleManager.instance.SetTargetedEnemyId(targetId);
    }
}
