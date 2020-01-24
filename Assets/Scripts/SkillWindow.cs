using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour {
    public new Text name;
    public Text skillPoints, xp;

    public GameObject attributesWindow;

    public void OpenAttributesWindow() {
        GameMenu.instance.PlayOpenSound();
        GameMenu.instance.ShowSubWindow(attributesWindow);
        attributesWindow.SetActive(true);
    }
}
