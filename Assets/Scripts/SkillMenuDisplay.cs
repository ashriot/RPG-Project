using UnityEngine;
using UnityEngine.UI;

public class SkillMenuDisplay : MonoBehaviour {
    public new Text name;
    public Text skillPoints, xp;

    public GameObject attributesWindow;

    public void OpenAttributesWindow() {
        GameMenu.instance.PlayOpenSound();
        GameMenu.instance.ShowBackButton();
        attributesWindow.SetActive(true);
    }
}
