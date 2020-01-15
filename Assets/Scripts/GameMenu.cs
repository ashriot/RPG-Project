using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

    public static GameMenu instance;

    [Header("Object References")]
    public Canvas canvas;
    public EventNotification note;
    public GameObject statsWindow;
    public Text tooltip;
    public Text goldKeyText;
    public Text silverKeyText;
    public Text goldText;
    public string mainMenuSceneName;
    public Image mainMenuButtonImage;
    public Text mainMenuButtonText;
    public Sprite buttonPressedSprite;
    public Sprite buttonReadySprite;
    public GameObject menuButtonsPanel;
    public GameObject mainWindowPanel;
    public Hero selectedHero;
    public GameObject[] heroStatPanels;
    public GameObject[] windows;
    public Button[] menuButtons;
    public Button backButton;
    public Button removeAll;
    public Button autoEquip;
    public Text[] nameTexts, hpTexts, mpTexts;
    public Image[] hpSliders, mpSliders;
    public Image[] currentTurnOutlines;
    public Image[] spIndicators;
    public HeroDisplay miniStatPanelDisplay;
    public HeroDisplay heroDisplayStatus;
    public SkillMenuDisplay skillMenu;
    public EquipmentWindow equipmentWindow;
    public string[] skillNames;

    [Header("Inventory Refs")]
    public ItemButton[] itemButtons;

    [Header("Data Trackers")]
    public int currentWindowId = 0;
    public int currentHeroId = 0;

    public Hero[] heroes;


    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        UpdateStats();
        UpdateKeys();
        UpdateGold();
        CloseMainMenu();
        CloseAllWindows();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestoreAllHpMp();
        }
    }

    public void RestoreAllHpMp() {
        foreach (var hero in heroes) {
            hero.hp.SetToMax();
            hero.mp.SetToMax();
        }
        UpdateStats();
        UpdateMiniStatPanel();
    }

    public void Back() {
        if (skillMenu.gameObject.activeInHierarchy) {
            if (skillMenu.attributesWindow.activeInHierarchy) {
                AudioManager.instance.PlaySfx("click");
                skillMenu.attributesWindow.SetActive(false);
                backButton.gameObject.SetActive(false);
            }
        }
    }

    public void CloseMainMenu() {
        CloseAllWindows();
        ReadyHeroButtons();
        menuButtonsPanel.SetActive(false);
        mainMenuButtonImage.sprite = buttonReadySprite;
        mainMenuButtonText.text = "Menu";
        GameManager.instance.menuButtonsOpen = false;
        PlayerController.instance.uiOpen = false;
    }

    public void CloseWindow(int windowId) {

    }

    public void ClickHero(int heroId) {
        if (menuButtonsPanel.activeInHierarchy) {
            if (currentHeroId == heroId) return;
        }
        currentHeroId = heroId;
        PlayOpenSound();
        UpdateHeroButtons();
        if (!menuButtonsPanel.activeInHierarchy) {
            menuButtonsPanel.SetActive(true);
            OpenMainMenu();
            return;
        }
        
        UpdateHeroButtons();
        if (currentWindowId == 0) { // status
            OpenStatus();
        } else if (currentWindowId == 1) { // inventory
            OpenInventory();
        } else if (currentWindowId == 2) { // equipment
            OpenEquipment();
        } else if (currentWindowId == 3) { // skills
            OpenSkills();
        }
    }

    public void ClickMainMenuButton() {
        if (!menuButtonsPanel.activeInHierarchy) {
            PlayOpenSound();
            UpdateHeroButtons();
            OpenMainMenu();
        } else {
            PlayCloseSound();
            CloseMainMenu();
            ReadyHeroButtons();
        }
    }

    public void ClickMenuButton(int buttonId) {
        if (currentWindowId != buttonId) {
            currentWindowId = buttonId;
            PlayOpenSound();
            OpenWindow();
        }
    }

    public void LootNotification(string itemId) {
        var position = Camera.main.WorldToScreenPoint(PlayerController.instance.transform.position);
        Instantiate(note, position, transform.rotation, canvas.transform).GenerateLootNote(itemId);
    }

    public void Notification(string message) {
        var position = Camera.main.WorldToScreenPoint(PlayerController.instance.transform.position);
        Instantiate(note, position, transform.rotation, canvas.transform).GenerateNote(message);
    }

    public void OpenMainMenu() {
        menuButtonsPanel.SetActive(true);
        mainMenuButtonText.text = "Close";
        mainMenuButtonImage.sprite = buttonPressedSprite;
        GameManager.instance.menuButtonsOpen = true;
        PlayerController.instance.uiOpen = true;
        mainWindowPanel.SetActive(true);
        OpenWindow();
        UpdateMiniStatPanel();
    }

    public void UpdateMiniStatPanel() {
        var hero = heroes[currentHeroId];
        // miniStatPanelDisplay.name.text = hero.name;
        // miniStatPanelDisplay.xp.text = "XP: " + hero.xp + "/" + hero.level * 100;
        miniStatPanelDisplay.sp.text = hero.sp.ToString();
        miniStatPanelDisplay.hp.text = hero.hp.max.ToString();
        miniStatPanelDisplay.mp.text = hero.mp.max.ToString();
        miniStatPanelDisplay.attack.text = hero.attack.value.ToString();
        miniStatPanelDisplay.defense.text = hero.defense.value.ToString();
        miniStatPanelDisplay.magic.text = hero.magic.value.ToString();
        miniStatPanelDisplay.speed.text = hero.speed.value.ToString();
        miniStatPanelDisplay.armor.text = hero.armor.value.ToString();
        miniStatPanelDisplay.resist.text = hero.resist.value.ToString();

        var green = new Color(.3843f, .7019f, .3294f, 1f);
        var white = new Color(1f, .9450f, .9137f, 1f);
        if (hero.hp.totalBonus > 0) {
            miniStatPanelDisplay.hp.color = green;
        } else {
            miniStatPanelDisplay.hp.color = white;
        }
        if (hero.mp.totalBonus > 0) {
            miniStatPanelDisplay.mp.color = green;
        } else {
            miniStatPanelDisplay.mp.color = white;
        }
        if (hero.attack.totalBonus > 0) {
            miniStatPanelDisplay.attack.color = green;
        } else {
            miniStatPanelDisplay.attack.color = white;
        }
        if (hero.defense.totalBonus > 0) {
            miniStatPanelDisplay.defense.color = green;
        } else {
            miniStatPanelDisplay.defense.color = white;
        }
        if (hero.magic.totalBonus > 0) {
            miniStatPanelDisplay.magic.color = green;
        } else {
            miniStatPanelDisplay.magic.color = white;
        }
        if (hero.speed.totalBonus > 0) {
            miniStatPanelDisplay.speed.color = green;
        } else {
            miniStatPanelDisplay.speed.color = white;
        }
        if (hero.armor.totalBonus > 0) {
            miniStatPanelDisplay.armor.color = green;
        } else {
            miniStatPanelDisplay.armor.color = white;
        }
        if (hero.resist.totalBonus > 0) {
            miniStatPanelDisplay.resist.color = green;
        } else {
            miniStatPanelDisplay.resist.color = white;
        }
    }

    public void OpenInventory() {
        UpdateStats();
        SetInventory();
        UpdateMiniStatPanel();
    }

    public void OpenEquipment() {
        removeAll.gameObject.SetActive(true);
        autoEquip.gameObject.SetActive(true);
        UpdateStats();
        SetHeroEquipment();
        UpdateMiniStatPanel();
    }

    public void OpenSkills() {
        UpdateStats();
        SetHeroSkills();
        UpdateMiniStatPanel();
    }

    public void OpenStatus() {
        UpdateStats();
        SetHeroStatus();
    }

    public void QuitGame() {
        SceneManager.LoadScene(mainMenuSceneName);
        Destroy(InventoryManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
    }

    public void SaveGame() {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void SetHeroStatus() {
        // var hero = heroes[currentHeroId];
        // heroDisplayStatus.name.text = hero.name;
        // heroDisplayStatus.level.text = hero.GetLevelAndClass();
        // heroDisplayStatus.xp.text = "XP: " + hero.xp + "/" + hero.level * 100;
        // heroDisplayStatus.hp.text = hero.hp.display;
        // heroDisplayStatus.mp.text = hero.mp.display;
        // heroDisplayStatus.attack.text = hero.attack.value.ToString();
        // heroDisplayStatus.defense.text = hero.defense.value.ToString();
        // heroDisplayStatus.magic.text = hero.magic.value.ToString();
        // heroDisplayStatus.speed.text = hero.speed.value.ToString();
        // heroDisplayStatus.armor.text = hero.armor.value.ToString();
        // heroDisplayStatus.resist.text = hero.resist.value.ToString();
        // heroDisplayStatus.critPower.text = hero.critPower.value.ToString();

        // if (hero.mainHand != null) {
        //     heroDisplayStatus.MainHandAtkOrBlk.text = hero.mainHand.GetAtkOrBlk();
        //     heroDisplayStatus.MainHandDmgOrAmt.text = hero.mainHand.GetDmgOrAmt();
        // } else {
        //     heroDisplayStatus.MainHandAtkOrBlk.text = "-";
        //     heroDisplayStatus.MainHandDmgOrAmt.text = "-";
        // }
        // if (hero.offHand != null) {
        //     heroDisplayStatus.OffHandAtkOrBlk.text = hero.offHand.GetAtkOrBlk();
        //     heroDisplayStatus.OffHandDmgOrAmt.text = hero.offHand.GetDmgOrAmt();
        // } else {
        //     heroDisplayStatus.OffHandAtkOrBlk.text = "-";
        //     heroDisplayStatus.OffHandDmgOrAmt.text = "-";
        // }
    }

    public void SetInventory() {
        var inventory = InventoryManager.instance.inventory;
        for (var i = 0; i < itemButtons.Length; i++) {
            if (i >= inventory.Count) {
                itemButtons[i].gameObject.SetActive(false);
            } else {
                itemButtons[i].gameObject.SetActive(true);
                itemButtons[i].buttonId = i;
                itemButtons[i].nameText.text = inventory[i].name;
                itemButtons[i].buttonImage.sprite = inventory[i].sprite;
                if (inventory[i].itemType == ItemType.Consumable) {
                    itemButtons[i].quantityText.text = "x" + inventory[i].quantity;
                } else {
                    itemButtons[i].quantityText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetHeroEquipment() {
        const string none = "      [None]";

        var hero = heroes[currentHeroId];
        equipmentWindow.head = hero.head;
        equipmentWindow.body = hero.body;
        equipmentWindow.arms = hero.arms;
        equipmentWindow.feet = hero.feet;
        equipmentWindow.ringL = hero.ringL;
        equipmentWindow.ringR = hero.ringR;
        equipmentWindow.mainHand = hero.mainHand;
        equipmentWindow.offHand = hero.offHand;

        if (equipmentWindow.head != null) {
            equipmentWindow.headButton.image.enabled = true;
            equipmentWindow.headButton.image.sprite = equipmentWindow.head.sprite;
            equipmentWindow.headButton.text.text = equipmentWindow.head.name;
        } else {
            equipmentWindow.headButton.image.enabled = false;
            equipmentWindow.headButton.text.text = none;
        }
        if (equipmentWindow.body != null) {
            equipmentWindow.bodyButton.image.enabled = true;
            equipmentWindow.bodyButton.image.sprite = equipmentWindow.body.sprite;
            equipmentWindow.bodyButton.text.text = equipmentWindow.body.name;
        } else {
            equipmentWindow.bodyButton.image.enabled = false;
            equipmentWindow.bodyButton.text.text = none;
        }
        if (equipmentWindow.arms != null) {
            equipmentWindow.armsButton.image.enabled = true;
            equipmentWindow.armsButton.image.sprite = equipmentWindow.arms.sprite;
            equipmentWindow.armsButton.text.text = equipmentWindow.arms.name;
        } else {
            equipmentWindow.armsButton.image.enabled = false;
            equipmentWindow.armsButton.text.text = none;
        }
        if (equipmentWindow.feet != null) {
            equipmentWindow.feetButton.image.enabled = true;
            equipmentWindow.feetButton.image.sprite = equipmentWindow.feet.sprite;
            equipmentWindow.feetButton.text.text = equipmentWindow.feet.name;
        } else {
            equipmentWindow.feetButton.image.enabled = false;
            equipmentWindow.feetButton.text.text = none;
        }
        if (equipmentWindow.ringL != null) {
            equipmentWindow.ringLButton.image.enabled = true;
            equipmentWindow.ringLButton.image.sprite = equipmentWindow.ringL.sprite;
            equipmentWindow.ringLButton.text.text = equipmentWindow.ringL.name;
        } else {
            equipmentWindow.ringLButton.image.enabled = false;
            equipmentWindow.ringLButton.text.text = none;
        }
        if (equipmentWindow.ringR != null) {
            equipmentWindow.ringRButton.image.enabled = true;
            equipmentWindow.ringRButton.image.sprite = equipmentWindow.ringR.sprite;
            equipmentWindow.ringRButton.text.text = equipmentWindow.ringR.name;
        } else {
            equipmentWindow.ringRButton.image.enabled = false;
            equipmentWindow.ringRButton.text.text = none;
        }
        if (equipmentWindow.mainHand != null) {
            equipmentWindow.mainHandButton.image.enabled = true;
            equipmentWindow.mainHandButton.image.sprite = equipmentWindow.mainHand.sprite;
            equipmentWindow.mainHandButton.text.text = equipmentWindow.mainHand.name;
            var penalty = 0;
            if (equipmentWindow.offHand != null && equipmentWindow.offHand.equipmentType != EquipmentType.Shield) {
                penalty = 6;
            }
            equipmentWindow.mainHandAtkOrBlk.text = equipmentWindow.mainHand.GetAtkOrBlkString(hero.attack.value - penalty);
            equipmentWindow.mainHandDmgOrAmt.text = equipmentWindow.mainHand.GetDmgOrAmtString();
        } else {
            equipmentWindow.mainHandButton.image.enabled = false;
            equipmentWindow.mainHandButton.text.text = none;
            equipmentWindow.mainHandAtkOrBlk.text = "---";
            equipmentWindow.mainHandDmgOrAmt.text = "---";
        }
        if (equipmentWindow.offHand != null) {
            equipmentWindow.offHandButton.image.enabled = true;
            equipmentWindow.offHandButton.image.sprite = equipmentWindow.offHand.sprite;
            equipmentWindow.offHandButton.text.text = equipmentWindow.offHand.name;
            if (equipmentWindow.offHand.equipmentType == EquipmentType.Shield) {
                equipmentWindow.offHandAtkOrBlk.text = equipmentWindow.offHand.GetAtkOrBlkString();
                equipmentWindow.offHandDmgOrAmt.text = equipmentWindow.offHand.GetDmgOrAmtString();
            } else {
                var penalty = 10;
                equipmentWindow.offHandAtkOrBlk.text = equipmentWindow.offHand.GetAtkOrBlkString(hero.attack.value - penalty);
                equipmentWindow.offHandDmgOrAmt.text = equipmentWindow.offHand.GetDmgOrAmtString();
            }
            
        } else {
            equipmentWindow.offHandButton.image.enabled = false;
            equipmentWindow.offHandButton.text.text = none;
            equipmentWindow.offHandAtkOrBlk.text = "---";
            equipmentWindow.offHandDmgOrAmt.text = "---";
        }
    }

    public void SetHeroSkills() {
        var hero = heroes[currentHeroId];
        skillMenu.xp.text = "XP: " + hero.xp + "/" + hero.xpRequired();
        // skillMenu.skillPoints.text = hero.sp.ToString();
        var skillLines = skillMenu.attributesWindow.GetComponent<SkillDisplay>().skillLines;
        for (var i = 0; i < skillLines.Length; i++) {
            for (var j = 0; j < hero.attributeSkillValues.Length; j++) {
                if (skillNames[j] == skillLines[i].name) {
                    // INT(0.5 * (SQRT(8 * SP + 1) - 1))
                    var skillRk = (int)(.5f * (Mathf.Sqrt(8f * hero.attributeSkillValues[j] + 1f) - 1f));
                    // Lv * (Lv +1) / 2
                    var skillThreshold = (int)(skillRk * (skillRk +1) / 2);
                    var skillSp = hero.attributeSkillValues[j] - skillThreshold;
                    var width = skillRk * 6;
                    skillLines[i].skillRank.rectTransform.sizeDelta = new Vector2(width, 5f);
                    width = skillSp * 6;
                    skillLines[i].skillPoints.rectTransform.sizeDelta = new Vector2(width, 5f);
                    skillLines[i].rankText.text = "Rk." + skillRk;
                    break;
                }
            }
        }
    }

    public void IncrementSkill(string skillName) {
        var hero = heroes[currentHeroId];

        if (hero.sp > 0) {
            GameMenu.instance.SetHeroSkills();

            for(var i = 0; i < hero.attributeSkillValues.Length; i++) {
                if (skillNames[i] == skillName) {
                    if (hero.attributeSkillValues[i] == 55) {
                        AudioManager.instance.PlaySfx("error");
                        Debug.Log("Cannot invest anymore points!");
                        return;
                    }
                    hero.sp--;
                    hero.attributeSkillValues[i]++;
                    var skillRank = (.5f * (Mathf.Sqrt(8f * hero.attributeSkillValues[i] + 1f) - 1f));
                    if (skillRank % 1 == 0) {
                        AudioManager.instance.PlaySfx("score");
                        SkillIncreaseEffect(skillName, (int)skillRank);
                    } else {
                        AudioManager.instance.PlaySfx("click");
                    }
                    break;
                }
            }
        }
        UpdateMiniStatPanel();
        UpdateStats();
        SetHeroSkills();
    }

    private void SkillIncreaseEffect(string skillName, int rank) {
        if (skillName == "Acrobatics") {
            var hero = heroes[currentHeroId];
            hero.speed.baseValue++;
        } else if (skillName == "Endurance") {
            var hero = heroes[currentHeroId];
            var increase = 5 * rank;
            hero.hp.current += increase;
            hero.hp.maximum += increase;
        } else if (skillName == "Precision") {
            var hero = heroes[currentHeroId];
            hero.attack.baseValue++;
        } else if (skillName == "Spirit") {
            var hero = heroes[currentHeroId];
            var increase = 5 * rank;
            hero.mp.current += increase;
            hero.mp.maximum += increase;
        } else if (skillName == "Toughness") {
            var hero = heroes[currentHeroId];
            hero.defense.baseValue++;
        } else if (skillName == "Willpower") {
            var hero = heroes[currentHeroId];
            hero.magic.baseValue++;
        }
        // Add more as needed
    }

    public void OpenWindow() {
        for (var i = 0; i < windows.Length; i++) {
            if (i == currentWindowId) {
                if (!windows[i].activeInHierarchy) {
                    if (i == 0) { // status
                        OpenStatus();
                    } else if (i == 1) { // inventory
                        OpenInventory();
                    } else if (i == 2) { // equipment
                        OpenEquipment();
                    } else if (i == 3) { // skills
                        OpenSkills();
                    }
                    menuButtons[i].GetComponent<Image>().sprite = buttonPressedSprite;
                    windows[i].SetActive(true);
                }
            } else {
                menuButtons[i].GetComponent<Image>().sprite = buttonReadySprite;
                windows[i].SetActive(false);
            }
        }
    }

    public void UpdateGold() {
        goldText.text = GameManager.instance.currentGoldPieces.ToString("N0");
    }

    public void UpdateKeys() {
        goldKeyText.text = GameManager.instance.currentGoldKeys.ToString();
        silverKeyText.text = GameManager.instance.currentSilverKeys.ToString();
    }

    public void UpdateStats() {
        heroes = GameManager.instance.heroes;
        for (var i = 0; i < heroes.Length; i++) {
            if (heroes[i].isActive) {
                heroes[i].CheckForNewSp();
                heroStatPanels[i].SetActive(true);
                nameTexts[i].text = heroes[i].name;
                hpTexts[i].text = heroes[i].hp.current.ToString();
                mpTexts[i].text = heroes[i].mp.current.ToString();
                hpSliders[i].fillAmount = heroes[i].hp.percent;
                mpTexts[i].text = heroes[i].mp.current.ToString();
                mpSliders[i].fillAmount = heroes[i].mp.percent;
                if (heroes[i].sp > 0) {
                    spIndicators[i].gameObject.SetActive(true);
                } else {
                    spIndicators[i].gameObject.SetActive(false);
                }
            } else {
                heroStatPanels[i].SetActive(false);
            }
        }
    }

    // Private Methods

    private void CloseAllWindows() {
        mainWindowPanel.SetActive(false);
        for (var i = 0; i < windows.Length; i++) {
            windows[i].SetActive(false);
        }
    }

    private void UpdateHeroButtons() {
        ReadyHeroButtons();
        for (var i = 0; i < heroStatPanels.Length; i++) {
            if (i == currentHeroId) {
                // heroStatPanels[i].GetComponent<Image>().sprite = buttonPressedSprite;
                currentTurnOutlines[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    private void ReadyHeroButtons() {
        for (var i = 0; i < heroStatPanels.Length; i++) {
            if (heroStatPanels[i].activeInHierarchy) {
                currentTurnOutlines[i].gameObject.SetActive(false);
            }
        }
    }

    private void ReadyAllMenuButtons() {
        for (var i = 0; i < menuButtons.Length; i++) {
            menuButtons[i].GetComponent<Image>().sprite = buttonReadySprite;
        }
    }

    private string GetArticle(string text) {
        string[] vowels = {"a", "e", "i", "o", "u" };
        var startingLetter = text.Substring(0, 1);
        foreach(var vowel in vowels) {
            if (vowel == startingLetter) {
                return "an ";
            }
        }
        return "a ";
    }
    
    public void PlayOpenSound() {
        AudioManager.instance.PlaySfx("loot");
    }
    
    public void PlayCloseSound() {
        AudioManager.instance.PlaySfx("chest");
    }
}
