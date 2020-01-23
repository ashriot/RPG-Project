using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

    public static GameMenu instance;

    [Header("Object References")]
    public Canvas canvas;
    public EventNotification note;
    public Text goldKeyText;
    public Text silverKeyText;
    public Text goldText;
    public string mainMenuSceneName;
    public Image mainMenuButtonImage;
    public Image mainMenuButtonIcon;
    public Text mainMenuButtonText;
    public Sprite buttonPressedSprite;
    public Sprite buttonReadySprite;
    public Sprite[] suits;
    public GameObject menuButtonsPanel;
    public GameObject mainWindowPanel;
    public Hero selectedHero;
    public GameObject[] windows;
    public Button[] menuButtons;
    public Button backButton;
    public Button removeAll;
    public Button autoEquip;
    public StatPanelDisplay[] heroStatPanels;
    public HeroDisplay miniStatPanelDisplay;
    public HeroDisplay heroDisplayStatus;
    public GameObject statsWindow;
    public SkillWindow skillWindow;
    public GameObject inventoryWindow;
    public EquipmentWindow equipmentWindow;
    public string[] skillNames;

    [Header("Inventory Refs")]
    public ItemButton[] itemButtons;
    public Item clickedItem;
    public EquippableItem clickedEquipment;
    public Image selectedItemCursor;
    public Color red;
    public Color green;
    public Color white;
    public Color gold;

    [Header("Sub Menus")]
    public GameObject tooltipPanel;
    public Text tooltipText;
    public GameObject inventorySubMenu;
    public ItemSubMenu itemSubMenu;
    public GameObject equipmentSubMenu;

    [Header("Data Trackers")]
    public int currentWindowId = 0;
    public int currentHeroId = 0;

    public Hero[] heroes;

    private GameObject subWindow;
    private GameObject subMenu;

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
        HideTooltip();
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
        PlayClickSound();
        ClearPreviews();
        HideBackButton();
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
        PlayClickSound();
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
        if (GameManager.instance.battleActive) {
            if (BattleManager.instance.openWindow != null) {
                PlayClickSound();
                BattleManager.instance.SetMenuButtonToFlee();
            }
            return;
        } else if (!menuButtonsPanel.activeInHierarchy) {
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
        HideAllSubMenus();
        HideSelectedItemCursor();
        ClearPreviews();
        if (currentWindowId != buttonId) {
            currentWindowId = buttonId;
            PlayClickSound();
            OpenWindow();
        }
    }

    public void ShowTooltip(string description) {
        Debug.Log("Tooltip: " + description);
        tooltipPanel.SetActive(true);
        tooltipText.text = description;
    }

    public void HideTooltip() {
        tooltipPanel.SetActive(false);
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
        HideTooltip();
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
        miniStatPanelDisplay.hp.text = hero.hp.maximum.ToString();
        miniStatPanelDisplay.hp.GetComponentInParent<ButtonLongPress>().description = "Hit Points [HP]\nOnce a unit reaches 0 HP, they fall unconscious and lose 1 CON.\nBase: " + hero.hp.baseMax.ToString() + "\nBonus: " + hero.hp.bonus.ToString();
        miniStatPanelDisplay.mp.text = hero.mp.maximum.ToString();
        miniStatPanelDisplay.mp.GetComponentInParent<ButtonLongPress>().description = "Magic Points [MP]\nUsed to cast spells and use magical abilities.\nBase: " + hero.mp.baseMax + "\nBonus: " + hero.mp.bonus;
        miniStatPanelDisplay.attack.text = hero.attack.value.ToString();
        miniStatPanelDisplay.attack.GetComponentInParent<ButtonLongPress>().description = "Attack [ATK]\nCompared to enemy Defense to determine weapon accuracy.\nBase: " + hero.attack.baseValue + "\nBonus: " + hero.attack.bonus;
        miniStatPanelDisplay.defense.text = hero.defense.value.ToString();
        miniStatPanelDisplay.defense.GetComponentInParent<ButtonLongPress>().description = "Defense [DEF]\nReduces enemy's Attack to reduce their weapon accuracy.\nBase: " + hero.defense.baseValue + "\nBonus: " + hero.defense.bonus;
        miniStatPanelDisplay.magic.text = hero.magic.value.ToString();
        miniStatPanelDisplay.magic.GetComponentInParent<ButtonLongPress>().description = "Magic [MAG]\nCompared to enemy's Magic to determine magic accuracy.0\nBase: " + hero.magic.baseValue + "\nBonus: " + hero.magic.bonus;
        miniStatPanelDisplay.speed.text = hero.speed.value.ToString();
        miniStatPanelDisplay.speed.GetComponentInParent<ButtonLongPress>().description = "Speed [SPD]\nDetermines how quickly a unit receives their turns.\nBase: " + hero.speed.baseValue + "\nBonus: " + hero.speed.bonus;
        miniStatPanelDisplay.armor.text = hero.armor.value.ToString();
        miniStatPanelDisplay.armor.GetComponentInParent<ButtonLongPress>().description = "Armor [ARM]\nReduces incoming physical damage.\nBase: " + hero.armor.baseValue + "\nBonus: " + hero.armor.bonus;
        miniStatPanelDisplay.resist.text = hero.resist.value.ToString();
        miniStatPanelDisplay.resist.GetComponentInParent<ButtonLongPress>().description = "Resist [RES]\nReduces incoming magical damage.\nBase: " + hero.resist.baseValue + "\nBonus: " + hero.resist.bonus;

        if (hero.hp.bonus > 0) {
            miniStatPanelDisplay.hp.color = gold;
        } else {
            miniStatPanelDisplay.hp.color = white;
        }
        if (hero.mp.bonus > 0) {
            miniStatPanelDisplay.mp.color = gold;
        } else {
            miniStatPanelDisplay.mp.color = white;
        }
        if (hero.attack.bonus > 0) {
            miniStatPanelDisplay.attack.color = gold;
        } else {
            miniStatPanelDisplay.attack.color = white;
        }
        if (hero.defense.bonus > 0) {
            miniStatPanelDisplay.defense.color = gold;
        } else {
            miniStatPanelDisplay.defense.color = white;
        }
        if (hero.magic.bonus > 0) {
            miniStatPanelDisplay.magic.color = gold;
        } else {
            miniStatPanelDisplay.magic.color = white;
        }
        if (hero.speed.bonus > 0) {
            miniStatPanelDisplay.speed.color = gold;
        } else {
            miniStatPanelDisplay.speed.color = white;
        }
        if (hero.armor.bonus > 0) {
            miniStatPanelDisplay.armor.color = gold;
        } else {
            miniStatPanelDisplay.armor.color = white;
        }
        if (hero.resist.bonus > 0) {
            miniStatPanelDisplay.resist.color = gold;
        } else {
            miniStatPanelDisplay.resist.color = white;
        }
    }

    public void ShowBackButton(GameObject window) {
        subWindow = window;
        subWindow.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void HideBackButton() {
        if (subWindow != null) {
            subWindow.SetActive(false);
            subWindow = null;
        }
        backButton.gameObject.SetActive(false);
    }

    public void HideAllSubMenus() {
        inventorySubMenu.SetActive(false);
        itemSubMenu.gameObject.SetActive(false);
        equipmentSubMenu.SetActive(false);
    }

    private void HideSelectedItemCursor() {
        selectedItemCursor.gameObject.SetActive(false);
    }

    public void OpenInventory(EquipmentButton button = null) {
        clickedEquipment = null;
        HideSelectedItemCursor();
        if (button == null) {
            HideAllSubMenus();
            inventorySubMenu.SetActive(true);
        }
        UpdateStats();
        SetInventory(button);
        UpdateMiniStatPanel();
    }

    public void OpenEquipment() {
        clickedEquipment = null;
        HideAllSubMenus();
        equipmentSubMenu.SetActive(true);
        UpdateStats();
        SetEquipment();
        UpdateMiniStatPanel();
    }

    public void OpenSkills() {
        HideAllSubMenus();
        UpdateStats();
        SetHeroSkills();
        UpdateMiniStatPanel();
    }

    public void OpenStatus() {
        HideAllSubMenus();
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

    private void ClearPreviews() {
        foreach(var preview in miniStatPanelDisplay.statPreviews) {
            preview.gameObject.SetActive(false);
        }
    }

    public void ClickItem(ItemButton button) {
        var fromEquipmentScreen = backButton.gameObject.activeInHierarchy;
        selectedItemCursor.gameObject.SetActive(true);
        selectedItemCursor.transform.position = button.transform.position;
        AudioManager.instance.PlaySfx("click");
        HideAllSubMenus();
        clickedItem = button.item;
        if (clickedItem.GetType() == typeof(EquippableItem)
            || clickedItem.GetType() == typeof(Weapon)
            || clickedItem.GetType() == typeof(Shield)) {
            itemSubMenu.UseText.text = "Equip";
        } else {
            itemSubMenu.UseText.text = "Use";
        }
        itemSubMenu.gameObject.SetActive(true);
        Debug.Log("Item clicked: " + button.item.name);
        if (clickedItem.GetType() == typeof(EquippableItem) // comparing equipment
            || clickedItem.GetType() == typeof(Weapon)
            || clickedItem.GetType() == typeof(Shield)) {
            ClearPreviews();
            var itemPreview = clickedItem as EquippableItem;
            var comparisonItem = clickedEquipment;
            EquippableItem secondaryComparison = null;
            var comparingRings = false;
            if (comparisonItem == null) {
                if (clickedItem.GetType() == typeof(EquippableItem))
                if (clickedItem.itemType == ItemTypes.Arms) {
                    comparisonItem = heroes[currentHeroId].GetBodyEquipment(EquipmentSlots.Arms);
                    if (!fromEquipmentScreen) { itemSubMenu.slot = EquipmentSlots.Arms; }
                }
                if (clickedItem.itemType == ItemTypes.Body) {
                    comparisonItem = heroes[currentHeroId].GetBodyEquipment(EquipmentSlots.Body);
                    if (!fromEquipmentScreen) { itemSubMenu.slot = EquipmentSlots.Body; }
                }
                if (clickedItem.itemType == ItemTypes.Feet) {
                    comparisonItem = heroes[currentHeroId].GetBodyEquipment(EquipmentSlots.Feet);
                    if (!fromEquipmentScreen) { itemSubMenu.slot = EquipmentSlots.Feet; }
                }
                if (clickedItem.itemType == ItemTypes.Head) {
                    comparisonItem = heroes[currentHeroId].GetBodyEquipment(EquipmentSlots.Head);
                    if (!fromEquipmentScreen) { itemSubMenu.slot = EquipmentSlots.Head; }
                }
                if (clickedItem.itemType == ItemTypes.Finger) {
                    comparingRings = true;
                    comparisonItem = heroes[currentHeroId].GetBodyEquipment(EquipmentSlots.RingL);
                    secondaryComparison = heroes[currentHeroId].GetBodyEquipment(EquipmentSlots.RingR);
                    if (!fromEquipmentScreen) { itemSubMenu.slot = EquipmentSlots.RingL; }
                }
            }
            EquippableItem currentItem = comparisonItem == null ? null : comparisonItem;
            var delta = 0;
            var delta2 = 0;
            for (var i = 0; i < (int)Stats.Count; i++) {
                delta = itemPreview.statBonuses[i];
                delta -= currentItem?.statBonuses[i] ?? 0;
                if (comparingRings) {
                    delta2 = itemPreview.statBonuses[i];
                    delta2 -= secondaryComparison?.statBonuses[i] ?? 0;
                }
                if (delta != 0) {
                    miniStatPanelDisplay.statPreviews[i].gameObject.SetActive(true);
                    miniStatPanelDisplay.statPreviews[i].color = delta > 0 ? green : red;
                    miniStatPanelDisplay.statPreviews[i].text = delta > 0 ? "+" + delta : delta.ToString();
                }
                if (delta2 != 0) {
                    if (miniStatPanelDisplay.statPreviews[i].gameObject.activeInHierarchy) {
                        miniStatPanelDisplay.statPreviews[i].color = delta > 0 ? green : red;
                        miniStatPanelDisplay.statPreviews[i].text += "/" + (delta2 > 0 ? "+" + delta2 : delta2.ToString());
                    } else {
                        miniStatPanelDisplay.statPreviews[i].gameObject.SetActive(true);
                        miniStatPanelDisplay.statPreviews[i].color = delta2 > 0 ? green : red;
                        miniStatPanelDisplay.statPreviews[i].text = "-/" + (delta2 > 0 ? "+" + delta2 : delta2.ToString());
                    }
                } else if (comparingRings) {
                    miniStatPanelDisplay.statPreviews[i].text += "/-";
                }
            }
        }
    }

    public void SetInventory(EquipmentButton button = null) {
        var inventory = InventoryManager.instance.inventory;
        if (button != null) {
            var item = button.equippedItem;
            var handFilter = new List<EquipmentTypes>();
            if (button.slot == EquipmentSlots.MainHand) {
                handFilter.Add(EquipmentTypes.HeavyWeapon);
                handFilter.Add(EquipmentTypes.LightWeapon);
                handFilter.Add(EquipmentTypes.MagicWeapon);
            } else if (button.slot == EquipmentSlots.OffHand) {
                handFilter.Add(EquipmentTypes.LightWeapon);
                handFilter.Add(EquipmentTypes.Shield);
            }
            ShowBackButton(inventoryWindow);
            inventory = inventory.Where(i => i.itemType == button.itemType).ToList();
            if (handFilter.Count > 0) {
                var equipment = inventory.Cast<Hands>();
                inventory = equipment.Where(e => handFilter.Contains(e.equipmentType)).Cast<Item>().ToList();
            }
        }

        for (var i = 0; i < itemButtons.Length; i++) {
            if (i >= inventory.Count) {
                itemButtons[i].gameObject.SetActive(false);
            } else {
                itemButtons[i].gameObject.SetActive(true);
                itemButtons[i].buttonId = i;
                itemButtons[i].item = inventory[i];
                itemButtons[i].nameText.text = inventory[i].name;
                itemButtons[i].buttonImage.sprite = inventory[i].sprite;
                itemButtons[i].buttonLongPress.description = inventory[i].name + "\n";
                itemButtons[i].buttonLongPress.description += inventory[i].description + "\n";
                itemButtons[i].buttonLongPress.description += inventory[i].GetStatsString();
                if (inventory[i].itemType == ItemTypes.Consumable) {
                    itemButtons[i].quantityText.text = "x" + inventory[i].quantity;
                } else {
                    itemButtons[i].quantityText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetEquipment() {
        const string none = "      [None]";

        var hero = heroes[currentHeroId];
        equipmentWindow.head = hero.bodyEquipment[(int)EquipmentSlots.Head];
        equipmentWindow.body = hero.bodyEquipment[(int)EquipmentSlots.Body];
        equipmentWindow.arms = hero.bodyEquipment[(int)EquipmentSlots.Arms];
        equipmentWindow.feet = hero.bodyEquipment[(int)EquipmentSlots.Feet];
        equipmentWindow.ringL = hero.bodyEquipment[(int)EquipmentSlots.RingL];
        equipmentWindow.ringR = hero.bodyEquipment[(int)EquipmentSlots.RingR];
        equipmentWindow.mainHand = hero.handEquipment[0];
        equipmentWindow.offHand = hero.handEquipment[1];

        equipmentWindow.equipment = new EquippableItem[] {
            equipmentWindow.head,
            equipmentWindow.body,
            equipmentWindow.arms,
            equipmentWindow.feet,
            equipmentWindow.ringL,
            equipmentWindow.ringR,
            equipmentWindow.mainHand,
            equipmentWindow.offHand
        };

        for (var i = 0; i < equipmentWindow.equipment.Length; i++) {
            if (equipmentWindow.equipment[i] != null) {
                equipmentWindow.buttons[i].image.enabled = true;
                equipmentWindow.buttons[i].image.sprite = equipmentWindow.equipment[i].sprite;
                equipmentWindow.buttons[i].id = equipmentWindow.equipment[i].id;
                equipmentWindow.buttons[i].text.text = equipmentWindow.equipment[i].name;
                equipmentWindow.buttons[i].buttonLongPress.description = equipmentWindow.equipment[i].name + "\n";
                equipmentWindow.buttons[i].buttonLongPress.description += equipmentWindow.equipment[i].description + "\n";
                equipmentWindow.buttons[i].buttonLongPress.description += equipmentWindow.equipment[i].GetStatsString();
                equipmentWindow.buttons[i].equippedItem = equipmentWindow.equipment[i];
            } else {
                equipmentWindow.buttons[i].image.enabled = false;
                equipmentWindow.buttons[i].text.text = none;
                equipmentWindow.buttons[i].buttonLongPress.description = string.Empty;
            }
        }

        if (equipmentWindow.mainHand != null) {
            equipmentWindow.mainHandButton.image.enabled = true;
            equipmentWindow.mainHandButton.image.sprite = equipmentWindow.mainHand.sprite;
            equipmentWindow.mainHandButton.text.text = equipmentWindow.mainHand.name;
            var penalty = 0;
            if (equipmentWindow.offHand != null && equipmentWindow.offHand.equipmentType != EquipmentTypes.Shield) {
                penalty = 6;
            }
            equipmentWindow.mainHandAtkOrBlk.text = equipmentWindow.mainHand.GetAtkOrBlkString(hero.attack.value - penalty);
            equipmentWindow.mainHandDmgOrAmt.text = equipmentWindow.mainHand.GetDmgOrAmtString();
        }
        if (equipmentWindow.offHand != null) {
            equipmentWindow.offHandButton.image.enabled = true;
            equipmentWindow.offHandButton.image.sprite = equipmentWindow.offHand.sprite;
            equipmentWindow.offHandButton.text.text = equipmentWindow.offHand.name;
            if (equipmentWindow.offHand.equipmentType == EquipmentTypes.Shield) {
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

    public void ClickEquipment(EquipmentButton button) {
        PlayClickSound();
        itemSubMenu.slot = button.slot;
        clickedEquipment = button.equippedItem;
        Debug.Log("Item ID: " + button.id);

        windows[1].SetActive(true);
        OpenInventory(button);
    }

    public void SetHeroSkills() {
        var hero = heroes[currentHeroId];
        skillWindow.xp.text = "XP: " + hero.xp + "/" + hero.xpRequiredForNextSkillPoint();
        // skillMenu.skillPoints.text = hero.sp.ToString();
        var skillLines = skillWindow.attributesWindow.GetComponent<SkillDisplay>().skillLines;
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
        } else {
            AudioManager.instance.PlaySfx("error");
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
            hero.hp.baseMax += increase;
        } else if (skillName == "Precision") {
            var hero = heroes[currentHeroId];
            hero.attack.baseValue++;
        } else if (skillName == "Spirit") {
            var hero = heroes[currentHeroId];
            var increase = 5 * rank;
            hero.mp.current += increase;
            hero.mp.baseMax += increase;
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
                heroes[i].CheckForNewSkillPoint();
                heroStatPanels[i].gameObject.SetActive(true);
                heroStatPanels[i].nameText.text = heroes[i].name;
                heroStatPanels[i].portraitAnimator.runtimeAnimatorController = heroes[i].portraitAnimatorController;
                heroStatPanels[i].hpText.text = heroes[i].hp.current.ToString();
                heroStatPanels[i].mpText.text = heroes[i].mp.current.ToString();
                heroStatPanels[i].hpSlider.fillAmount = heroes[i].hp.percent;
                heroStatPanels[i].mpText.text = heroes[i].mp.current.ToString();
                heroStatPanels[i].mpSlider.fillAmount = heroes[i].mp.percent;

                if (heroes[i].sp > 0) {
                    heroStatPanels[i].spIndicator.gameObject.SetActive(true);
                } else {
                    heroStatPanels[i].spIndicator.gameObject.SetActive(false);
                }
                if (GameManager.instance.battleActive) {
                    heroStatPanels[i].targetIndicator.gameObject.SetActive(true);
                    heroStatPanels[i].targetIndicator.sprite = suits[i];
                    heroStatPanels[i].deflectSlider.fillAmount = (float)heroes[i].deflect / heroes[i].hp.maximum;
                } else {
                    heroStatPanels[i].targetIndicator.gameObject.SetActive(false);
                    heroStatPanels[i].deflectSlider.fillAmount = 0;
                }
            } else {
                heroStatPanels[i].gameObject.SetActive(false);
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
                heroStatPanels[i].currentTurnCursor.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void ReadyHeroButtons() {
        for (var i = 0; i < heroStatPanels.Length; i++) {
            if (heroStatPanels[i].gameObject.activeInHierarchy) {
                heroStatPanels[i].currentTurnCursor.gameObject.SetActive(false);
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
        AudioManager.instance.PlaySfx("end_turn");
    }
    
    public void PlayClickSound() {
        AudioManager.instance.PlaySfx("click");
    }
    
    public void PlayCloseSound() {
        AudioManager.instance.PlaySfx("chest");
    }
}
