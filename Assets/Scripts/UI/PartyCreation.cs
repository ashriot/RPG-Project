using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyCreation : MonoBehaviour {

    public static PartyCreation instance;
    
    [Header("Object Refs")]
    public GameObject beginButton;
    public GameObject backButton;
    public GameObject removeButton;
    public HeroDisplay miniStatPanel;
    public GameObject currentPartyPanel;
    public GameObject basicClassesPanel;
    public GameObject advancedClassesPanel;
    public GameObject chosenClassPanel;
    public GameObject addNewHeroButton;
    public StatPanelDisplay[] statPanels;
    public GameObject tooltipPanel;
    public Text tooltipText;
    public Hero defaultHero;
    public TabButton defaultButton;
    public TabGroup basicClasses;
    public TabButton[] selectedClasses = new TabButton[4];

    [Header("Control Vars")]
    public int currentPartySize;
    public int selectedHeroId;
    public bool readyToBegin { get { return currentPartySize > 0; } }

    public string musicNameToPlay;
    private bool musicStarted = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        foreach(var panel in statPanels) {
            panel.gameObject.SetActive(false);
            panel.currentTurnCursor.gameObject.SetActive(false);
        }
        removeButton.SetActive(false);
        backButton.SetActive(false);
        // advancedClassesPanel.SetActive(false);
        chosenClassPanel.SetActive(false);
        addNewHeroButton.SetActive(true);
        basicClassesPanel.SetActive(false);
        miniStatPanel.gameObject.SetActive(false);
    }
    
    void LateUpdate() {
        if (!musicStarted){
            AudioManager.instance.PlayBgm(musicNameToPlay);
            musicStarted = true;
        }
    }

    public void ClickHero(int heroId) {
        if (selectedHeroId == heroId) { return; }
        PlayClickSound();
        selectedHeroId = heroId;
        UpdatePartyUi();
        SelectHero();
        basicClasses.OnTabSelected(selectedClasses[selectedHeroId]);
        miniStatPanel.gameObject.SetActive(true);
        basicClassesPanel.SetActive(true);
    }

    public void ClickClass(ClassButton button) {
        Debug.Log(button.hero.name);
        PlayClickSound();
        statPanels[selectedHeroId].heroRef = button.hero;
        selectedClasses[selectedHeroId] = button.tabButton;
        UpdatePartyUi();
    }

    public void ClickBack() {
        Debug.Log("Back clicked");
        PlayClickSound();
    }

    public void ClickAddNewHero() {
        PlayClickSound();
        removeButton.SetActive(true);
        statPanels[currentPartySize].gameObject.SetActive(true);
        statPanels[currentPartySize].heroRef = defaultHero;
        statPanels[currentPartySize].heroRef.isActive = true;
        selectedHeroId = currentPartySize;
        currentPartySize++;
        selectedClasses[selectedHeroId] = defaultButton;
        basicClasses.OnTabSelected(defaultButton);
        addNewHeroButton.SetActive(true);
        SelectHero();
        miniStatPanel.gameObject.SetActive(true);
        basicClassesPanel.SetActive(true);
        UpdatePartyUi();

        if (currentPartySize == 4) {
            addNewHeroButton.SetActive(false);
        }
    }

    private void SelectHero() {
        for (var i = 0; i < currentPartySize; i++) {
            if (i == selectedHeroId) {
                statPanels[i].currentTurnCursor.gameObject.SetActive(true);
            } else {
                statPanels[i].currentTurnCursor.gameObject.SetActive(false);
            }
        }

    }

    public void ClickRemove(int heroId) {
        PlayClickSound();
        statPanels[heroId].heroRef.isActive = false;
        statPanels[heroId].gameObject.SetActive(false);
        addNewHeroButton.SetActive(true);
        currentPartySize--;
        if (currentPartySize == 0) {
            removeButton.SetActive(true);
        }
    }
    
    public void PlayClickSound() {
        AudioManager.instance.PlaySfx("click");
    }

    public void ShowTooltip(string description) {
        Debug.Log("Tooltip: " + description);
        tooltipPanel.SetActive(true);
        tooltipText.text = description;
    }

    public void HideTooltip() {
        tooltipPanel.SetActive(false);
    }

    private void UpdatePartyUi() {
        if (readyToBegin) {
            beginButton.SetActive(true);
        } else {
            beginButton.SetActive(false);
        }
        UpdateMiniStatPanel();
        for (var i = 0; i < currentPartySize; i++) {
            var panel = statPanels[i];
            panel.nameText.text = panel.heroRef.name;
            panel.hpText.text = panel.heroRef.hp.baseMax.ToString();
            panel.mpText.text = panel.heroRef.mp.baseMax.ToString();
            panel.portraitAnimator.runtimeAnimatorController = panel.heroRef.portraitAnimatorController;
        }
    }

    public void UpdateMiniStatPanel() {
        var hero = statPanels[selectedHeroId].heroRef;
        miniStatPanel.sp.text = hero.sp.ToString();
        miniStatPanel.sp.GetComponent<ButtonLongPress>().description = "Skill Points [SP]\nUse these to improve your skills.\nXP: " + hero.xp + "/" + hero.xpRequiredForNextSkillPoint();
        miniStatPanel.hp.text = hero.hp.maximum.ToString();
        // miniStatPanel.hp.GetComponentInParent<ButtonLongPress>().description = "Hit Points [HP]\nOnce a unit reaches 0 HP, they fall unconscious and lose 1 CON.\nBase: " + hero.hp.baseMax + "\nBonus: " + hero.hp.bonus;
        miniStatPanel.mp.text = hero.mp.maximum.ToString();
        // miniStatPanel.mp.GetComponentInParent<ButtonLongPress>().description = "Magic Points [MP]\nUsed to cast spells and use magical abilities.\nBase: " + hero.mp.baseMax + "\nBonus: " + hero.mp.bonus;
        miniStatPanel.attack.text = hero.attack.value.ToString();
        // miniStatPanel.attack.GetComponentInParent<ButtonLongPress>().description = "Attack [ATK]\nCompared to enemy Defense to determine weapon accuracy.\nBase: " + hero.attack.baseValue + "\nBonus: " + hero.attack.bonus;
        miniStatPanel.defense.text = hero.defense.value.ToString();
        // miniStatPanel.defense.GetComponentInParent<ButtonLongPress>().description = "Defense [DEF]\nReduces enemy's Attack to reduce their weapon accuracy.\nBase: " + hero.defense.baseValue + "\nBonus: " + hero.defense.bonus;
        miniStatPanel.magic.text = hero.magic.value.ToString();
        // miniStatPanel.magic.GetComponentInParent<ButtonLongPress>().description = "Magic [MAG]\nCompared to enemy's Magic to determine magic accuracy.0\nBase: " + hero.magic.baseValue + "\nBonus: " + hero.magic.bonus;
        miniStatPanel.speed.text = hero.speed.value.ToString();
        // miniStatPanel.speed.GetComponentInParent<ButtonLongPress>().description = "Speed [SPD]\nDetermines how quickly a unit receives their turns.\nBase: " + hero.speed.baseValue + "\nBonus: " + hero.speed.bonus;
        miniStatPanel.armor.text = hero.armor.value.ToString();
        // miniStatPanel.armor.GetComponentInParent<ButtonLongPress>().description = "Armor [ARM]\nReduces incoming physical damage.\nBase: " + hero.armor.baseValue + "\nBonus: " + hero.armor.bonus;
        miniStatPanel.resist.text = hero.resist.value.ToString();
        // miniStatPanel.resist.GetComponentInParent<ButtonLongPress>().description = "Resist [RES]\nReduces incoming magical damage.\nBase: " + hero.resist.baseValue + "\nBonus: " + hero.resist.bonus;
    }
}
