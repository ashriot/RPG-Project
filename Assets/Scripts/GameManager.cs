using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Hero[] heroes;

    public bool menuButtonsOpen, dialogActive, fadingBetweenAreas, shopActive, battleActive;

    public int currentGoldPieces;
    public int currentGoldKeys;
    public int currentSilverKeys;

    void Awake() {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (menuButtonsOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive) {
            // PlayerController.instance.canMove = false;
        } else {
            // PlayerController.instance.canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            LoadData();
        }
    }

    // public Item GetItemDetails(string itemName) {
    //     for (var i = 0; i < referenceItems.Length; i++) {
    //         if (referenceItems[i].name.ToLower() == itemName.ToLower()) {
    //             return referenceItems[i];
    //         }
    //     }

    //     // item could not be found
    //     Debug.LogError("'" + itemName + "' does not exist!");
    //     return null;
    // }

    public void SaveData() {
        // player location data
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);

        // character data
        for (var i = 0; i < heroes.Length; i++) {
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Active", heroes[i].gameObject.activeInHierarchy ? 1 : 0);

            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Level", heroes[i].level);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentXp", heroes[i].xp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentHp", heroes[i].hp.current);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_MaxHp", heroes[i].hp.max);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentMp", heroes[i].mp.current);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_MaxMp", heroes[i].mp.max);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_BaseAttack", heroes[i].attack.baseValue);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_BaseDefense", heroes[i].defense.baseValue);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_BaseMagic", heroes[i].magic.baseValue);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_BaseSpeed", heroes[i].speed.baseValue);
        }

        // inventory data
        var inventory = InventoryManager.instance.inventory;
        for (var i = 0; i < inventory.Count; i++) {
            PlayerPrefs.SetString("Inventory_" + i, inventory[i].id);
        }
    }

    public void LoadData() {
        // player location data
        var x = PlayerPrefs.GetFloat("Player_Position_x");
        var y = PlayerPrefs.GetFloat("Player_Position_y");
        var z = PlayerPrefs.GetFloat("Player_Position_z");
        PlayerController.instance.transform.position = new Vector3(x, y, z);

        // character data
        for (var i = 0; i < heroes.Length; i++) {
            if (PlayerPrefs.GetInt("Player_" + heroes[i].name + "_Active") == 1) {
                heroes[i].gameObject.SetActive(true);
            } else {
                heroes[i].gameObject.SetActive(false);
            }

            heroes[i].level = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_Level");
            heroes[i].xp = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentXp");
            heroes[i].hp.current = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentHp");
            heroes[i].hp.max = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_MaxHp");
            heroes[i].mp.current = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentMp");
            heroes[i].mp.max = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_MaxMp");
            heroes[i].attack.baseValue = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_BaseAttack");
            heroes[i].defense.baseValue = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_BaseDefense");
            heroes[i].magic.baseValue = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_BaseMagic");
            heroes[i].speed.baseValue = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_BaseSpeed");
        }

        // inventory data
        var inventory = InventoryManager.instance.inventory;
        for (var i = 0; i < inventory.Count; i++) {
            inventory[i].id = PlayerPrefs.GetString("Inventory_" + i);
        }
    }
    
    public void AdjustKeys(int amount) {
        currentGoldKeys += amount;
        GameMenu.instance.UpdateKeys();
    }
}
