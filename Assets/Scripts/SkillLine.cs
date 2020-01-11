using UnityEngine;
using UnityEngine.UI;

public class SkillLine : MonoBehaviour {
    public Text nameText, rankText;
    public Image skillRank, skillPoints;
    public new string name;
    public int spInvested;


    public void Press() {
        GameMenu.instance.IncrementSkill(name);
    }
}
