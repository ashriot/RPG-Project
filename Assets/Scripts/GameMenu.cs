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
    public Sprite buttonPressedSprite;
    public Sprite buttonReadySprite;
    public GameObject menuButtonsPanel;
    public GameObject menuWindow;
    public Hero selectedHero;
    public GameObject[] heroStatPanels;
    public GameObject[] windows;
    public Button[] menuButtons;
    public Text[] nameTexts, hpTexts, mpTexts;
    public Image[] hpSliders, mpSliders;
    public Image[] heroImages;
    public HeroDisplay heroDisplayStatus;

    [Header("Data Trackers")]
    public int currentWindowId = 0;
    public int currentHeroId = 0;

    private Hero[] heroes;


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
        UpdateSideStats();
        UpdateKeys();
        UpdateGold();
        if (menuButtonsPanel.activeInHierarchy) {
            CloseMenu();
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void UpdateSideStats() {
        heroes = GameManager.instance.heroes;
        for (var i = 0; i < heroes.Length; i++) {
            if (heroes[i].isActive) {
                heroStatPanels[i].SetActive(true);
                nameTexts[i].text = heroes[i].name;
                hpTexts[i].text = heroes[i].hp.current.ToString();
                mpTexts[i].text = heroes[i].mp.current.ToString();
                hpSliders[i].fillAmount = heroes[i].hp.GetPercent();
                mpTexts[i].text = heroes[i].mp.current.ToString();
                mpSliders[i].fillAmount = heroes[i].mp.GetPercent();
            } else {
                heroStatPanels[i].SetActive(false);
            }
        }
    }

    public void ToggleWindow(int windowId) {
        currentWindowId = windowId;
        for (var i = 0; i < windows.Length; i++) {
            if (i == windowId) {
                if (!windows[i].activeInHierarchy) {
                    if (i == 0) {
                        OpenStatus();
                    }
                    menuButtons[i].GetComponent<Image>().sprite = buttonPressedSprite;
                    menuWindow.SetActive(true);
                    windows[i].SetActive(true);
                    PlayOpenSound();
                } else {
                    ResetAllHeroButtons();
                    currentWindowId = -1;
                    menuButtons[i].GetComponent<Image>().sprite = buttonReadySprite;
                    menuWindow.SetActive(false);
                    windows[i].SetActive(false);
                }
            } else {
                menuButtons[i].GetComponent<Image>().sprite = buttonReadySprite;
                windows[i].SetActive(false);
            }
        }
        // itemCharacterSelect.SetActive(false);
    }

    public void CloseMenu() {
        ResetAllHeroButtons();
        ResetAllMenuButtons();
        menuButtonsPanel.SetActive(false);
        menuWindow.SetActive(false);
        GameManager.instance.menuButtonsOpen = false;
        foreach (var window in windows) {
            window.SetActive(false);
        }
    }

    public void ClickHeroPanel(int heroId) {
        currentHeroId = heroId;
        if (!menuButtonsPanel.activeInHierarchy) return;
        var hero = heroes[heroId];
        ResetAllHeroButtons();
        for (var i = 0; i < heroStatPanels.Length; i++) {
            if (i == heroId) {
                heroStatPanels[i].GetComponent<Image>().sprite = buttonPressedSprite;
                break;
            }
        }
        if (currentWindowId == 0) { // status
            PlayOpenSound();
            heroDisplayStatus.name.text = hero.name;
            heroDisplayStatus.level.text = hero.GetLevelAndClass();
            heroDisplayStatus.xp.text = "XP: " + hero.xp + "/" + hero.level * 100;
            heroDisplayStatus.hp.text = hero.hp.GetDisplay();
            heroDisplayStatus.mp.text = hero.mp.GetDisplay();
            heroDisplayStatus.attack.text = hero.attack.GetTotalValue().ToString();
            heroDisplayStatus.defense.text = hero.defense.GetTotalValue().ToString();
            heroDisplayStatus.magic.text = hero.magic.GetTotalValue().ToString();
            heroDisplayStatus.speed.text = hero.speed.GetTotalValue().ToString();
            heroDisplayStatus.armor.text = hero.armor.GetTotalValue().ToString();
            heroDisplayStatus.resist.text = hero.resist.GetTotalValue().ToString();
            heroDisplayStatus.critPower.text = hero.critPower.GetTotalValue().ToString();

            if (hero.mainHand != null) {
                heroDisplayStatus.MainHandAtkOrBlk.text = hero.mainHand.GetAtkOrBlk();
                heroDisplayStatus.MainHandDmgOrAmt.text = hero.mainHand.GetDmgOrAmt();
            } else {
                heroDisplayStatus.MainHandAtkOrBlk.text = "-";
                heroDisplayStatus.MainHandDmgOrAmt.text = "-";
            }
            if (hero.offHand != null) {
                heroDisplayStatus.OffHandAtkOrBlk.text = hero.offHand.GetAtkOrBlk();
                heroDisplayStatus.OffHandDmgOrAmt.text = hero.offHand.GetDmgOrAmt();
            } else {
                heroDisplayStatus.OffHandAtkOrBlk.text = "-";
                heroDisplayStatus.OffHandDmgOrAmt.text = "-";
            }
        }
    }

    public void OpenStatus() {
        UpdateSideStats();
        ClickHeroPanel(currentHeroId);
    }

    public void SaveGame() {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void QuitGame() {
        SceneManager.LoadScene(mainMenuSceneName);
        Destroy(InventoryManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
    }

    public void ToggleMainMenu() {
        if (menuButtonsPanel.activeInHierarchy) {
            CloseAllWindows();
            ResetAllHeroButtons();
            PlayCloseSound();
            PlayerController.instance.uiOpen = false;
            menuWindow.SetActive(false);
            menuButtonsPanel.SetActive(false);
            mainMenuButtonImage.sprite = buttonReadySprite;
        } else {
            UpdateSideStats();
            PlayOpenSound();
            PlayerController.instance.uiOpen = true;
            menuWindow.SetActive(true);
            menuButtonsPanel.SetActive(true);
            mainMenuButtonImage.sprite = buttonPressedSprite;
            if (currentWindowId > -1) {
                ToggleWindow(currentWindowId);
            }
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

    public void UpdateGold() {
        goldText.text = GameManager.instance.currentGoldPieces.ToString("N0");
    }

    public void UpdateKeys() {
        goldKeyText.text = GameManager.instance.currentGoldKeys.ToString();
        silverKeyText.text = GameManager.instance.currentSilverKeys.ToString();
    }

    private void CloseAllWindows() {
        for (var i = 0; i < windows.Length; i++) {
            windows[i].SetActive(false);
        }
    }

    private void ResetAllHeroButtons() {
        for (var i = 0; i < heroStatPanels.Length; i++) {
            if (heroStatPanels[i].activeInHierarchy) {
                heroStatPanels[i].GetComponent<Image>().sprite = buttonReadySprite;
            }
        }
    }

    private void ResetAllMenuButtons() {
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
    
    private void PlayOpenSound() {
        AudioManager.instance.PlaySfx("loot");
    }
    
    private void PlayCloseSound() {
        AudioManager.instance.PlaySfx("chest");
    }
}
