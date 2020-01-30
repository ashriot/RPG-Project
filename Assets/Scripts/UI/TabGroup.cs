using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour {

    public List<TabButton> tabButtons = new List<TabButton>();
    public TabButton selectedTab;
    public Sprite tabIdleSprite;
    public Sprite tabHoverSprite;
    public Sprite tabSelectedSprite;

    void Start() {
        if (selectedTab != null) {
            selectedTab.backgroundImage.sprite = tabSelectedSprite;
        }
    }

    public void Subscribe(TabButton button) {
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button) {
        ResetTabs();
        if (selectedTab == null || button != selectedTab) {
            button.backgroundImage.sprite = tabHoverSprite;
        }
    }

    public void OnTabExit(TabButton button) {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button) {
        selectedTab = button;
        ResetTabs();
        button.backgroundImage.sprite = tabSelectedSprite;
    }

    public void ResetTabs() {
        foreach(var button in tabButtons) {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.backgroundImage.sprite = tabIdleSprite;
        }
    }
}
