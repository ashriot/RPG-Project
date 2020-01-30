using UnityEngine;
using UnityEngine.UI;

public class StatPanelDisplay : MonoBehaviour {
    public Text nameText, hpText, mpText;
    public Image hpSlider, deflectSlider, barrierSlider, mpSlider, currentTurnCursor, spIndicator, targetIndicator;
    public Animator portraitAnimator;

    public Hero heroRef;
}
