using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

    public static GameMenu instance;

    public string mainMenuSceneName;
    public GameObject gameMenu;
    public GameObject itemCharacterSelect;
    public GameObject[] windows;
    public GameObject[] statusButtons;

    private CharacterStats[] playerStats;

    public Text[] nameTexts, hpTexts, mpTexts, levelTexts, xpTexts;
    public Slider[] xpSliders;
    public Image[] characterImages;
    public GameObject[] characterStats;

    public Text statusName, statusLevel, statusHp, statusMp, statusStrength, statusDefense, statusEquippedWeapon,
        statusWeaponPower, statusEquippedArmor, statusArmorPower, statusXp;
    public Image statusImage;

    public ItemButton[] itemButtons;
    public Item selectedItem;
    public string selectedItemName;
    public Text itemName, itemDescription, useButtonText;
    public Text[] itemCharacterSelectNames;

    public Text goldText;

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire2")) {
            if (gameMenu.activeInHierarchy) {
                gameMenu.GetComponent<MenuFade>().FadeOut();
                CloseMenu();
                AudioManager.instance.PlaySfx("test");
            } else {
                gameMenu.SetActive(true);
                gameMenu.GetComponent<MenuFade>().FadeIn();
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
                AudioManager.instance.PlaySfx("test");
            }
        }
    }

    public void UpdateMainStats() {
        playerStats = GameManager.instance.playerStats;

         for (var i = 0; i < playerStats.Length; i++) {
             if (playerStats[i].gameObject.activeInHierarchy) {
                characterStats[i].SetActive(true);
                nameTexts[i].text = playerStats[i].characterName;
                levelTexts[i].text = playerStats[i].level.ToString();
                hpTexts[i].text = "HP: " + playerStats[i].currentHp + "/" + playerStats[i].maxHp;
                mpTexts[i].text = "MP: " + playerStats[i].currentMp + "/" + playerStats[i].maxMp;
                xpTexts[i].text = playerStats[i].currentXp.ToString("N0") + "/" + playerStats[i].xpToNextLevel[playerStats[i].level].ToString("N0");
                levelTexts[i].text = "Lv: " + playerStats[i].level;
                xpSliders[i].maxValue = playerStats[i].xpToNextLevel[playerStats[i].level];
                xpSliders[i].value = playerStats[i].currentXp;
                characterImages[i].sprite = playerStats[i].image.sprite;
             } else {
                 characterStats[i].SetActive(false);
             }
         }

         goldText.text = GameManager.instance.currentGold.ToString("N0");
    }

    public void ToggleWindow(int windowId) {
        UpdateMainStats();

        for (var i = 0; i < windows.Length; i++) {
            if (i == windowId) {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            } else {
                windows[i].SetActive(false);
            }
        }
        itemCharacterSelect.SetActive(false);
    }

    public void CloseMenu() {
        foreach (var window in windows) {
            window.SetActive(false);
        }

        itemCharacterSelect.SetActive(false);

        GameManager.instance.gameMenuOpen = false;
    }

    public void OpenStatus() {
        UpdateMainStats();
        DisplayCharacterStatus(0);

        for (var i = 0; i < statusButtons.Length; i++) {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].characterName;
        }
    }

    public void DisplayCharacterStatus(int id) {
        statusName.text = playerStats[id].characterName;
        statusLevel.text = playerStats[id].level.ToString();
        statusHp.text = playerStats[id].currentHp + "/" + playerStats[id].maxHp;
        statusMp.text = playerStats[id].currentMp + "/" + playerStats[id].maxMp;
        statusStrength.text = playerStats[id].attack.ToString();
        statusDefense.text = playerStats[id].defense.ToString();

        statusWeaponPower.text = playerStats[id].magic.ToString();
        
        statusArmorPower.text = playerStats[id].speed.ToString();

        statusXp.text = (playerStats[id].xpToNextLevel[playerStats[id].level] - playerStats[id].currentXp).ToString("N0");
        statusImage.sprite = playerStats[id].image.sprite;
    }

    public void ShowItems() {
        GameManager.instance.SortItems();
        for (var i = 0; i < itemButtons.Length; i++ ) {
            itemButtons[i].buttonId = i;

            if (GameManager.instance.inventory[i] != "") {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.inventory[i]).itemSprite;
                itemButtons[i].quantityText.text = GameManager.instance.inventoryQuantity[i].ToString();
            } else {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].quantityText.text = string.Empty;
            }
        }
    }

    public void SelectItem(Item item) {
        selectedItem = item;

        if (selectedItem.itemType == ItemType.item) {
            useButtonText.text = "Use";
        } else if (selectedItem.itemType == ItemType.armor || selectedItem.itemType == ItemType.weapon ) {
            useButtonText.text = "Equip";
        }

        itemName.text = selectedItem.itemName;
        itemDescription.text = selectedItem.description;
    }

    public void DiscardItem() {
        if (selectedItem != null) {
            GameManager.instance.RemoveItem(selectedItem.name);
        }
    }

    public void OpenItemCharacterSelect() {
        itemCharacterSelect.SetActive(true);
        for (var i = 0; i < itemCharacterSelectNames.Length; i++) {
            itemCharacterSelectNames[i].text = GameManager.instance.playerStats[i].characterName;
            itemCharacterSelectNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemCharacterSelect() {
        itemCharacterSelect.SetActive(false);
        
    }

    public void UseItem(int selectedCharacterId) {
        selectedItem.Use(selectedCharacterId);
        CloseItemCharacterSelect();
    }

    public void SaveGame() {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void QuitGame() {
        SceneManager.LoadScene(mainMenuSceneName);
        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
    }
    
    public void PlayButtonSound() {
        AudioManager.instance.PlaySfx("test");
    }
}
