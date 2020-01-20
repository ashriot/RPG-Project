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

    void Start() {

        for (var i = 0; i < heroes.Length; i++) {
            heroes[i] = Instantiate(heroes[i]);

            // check equipment
            var hpBonus = 0;
            var mpBonus = 0;
            var attackBonus = 0;
            var defenseBonus = 0;
            var magicBonus = 0;
            var speedBonus = 0;
            var armorBonus = 0;
            var resistBonus = 0;

            if (heroes[i].head != null) {
                hpBonus += heroes[i].head.hpBonus;
                mpBonus += heroes[i].head.mpBonus;
                attackBonus += heroes[i].head.attackBonus;
                defenseBonus += heroes[i].head.defenseBonus;
                magicBonus += heroes[i].head.magicBonus;
                speedBonus += heroes[i].head.speedBonus;
                armorBonus += heroes[i].head.armorBonus;
                resistBonus += heroes[i].head.resistBonus;
            }
            if (heroes[i].body != null) {
                hpBonus += heroes[i].body.hpBonus;
                mpBonus += heroes[i].body.mpBonus;
                attackBonus += heroes[i].body.attackBonus;
                defenseBonus += heroes[i].body.defenseBonus;
                magicBonus += heroes[i].body.magicBonus;
                speedBonus += heroes[i].body.speedBonus;
                armorBonus += heroes[i].body.armorBonus;
                resistBonus += heroes[i].body.resistBonus;
            }
            if (heroes[i].arms != null) {
                hpBonus += heroes[i].arms.hpBonus;
                mpBonus += heroes[i].arms.mpBonus;
                attackBonus += heroes[i].arms.attackBonus;
                defenseBonus += heroes[i].arms.defenseBonus;
                magicBonus += heroes[i].arms.magicBonus;
                speedBonus += heroes[i].arms.speedBonus;
                armorBonus += heroes[i].arms.armorBonus;
                resistBonus += heroes[i].arms.resistBonus;
            }
            if (heroes[i].feet != null) {
                hpBonus += heroes[i].feet.hpBonus;
                mpBonus += heroes[i].feet.mpBonus;
                attackBonus += heroes[i].feet.attackBonus;
                defenseBonus += heroes[i].feet.defenseBonus;
                magicBonus += heroes[i].feet.magicBonus;
                speedBonus += heroes[i].feet.speedBonus;
                armorBonus += heroes[i].feet.armorBonus;
                resistBonus += heroes[i].feet.resistBonus;
            }
            if (heroes[i].ringL != null) {
                hpBonus += heroes[i].ringL.hpBonus;
                mpBonus += heroes[i].ringL.mpBonus;
                attackBonus += heroes[i].ringL.attackBonus;
                defenseBonus += heroes[i].ringL.defenseBonus;
                magicBonus += heroes[i].ringL.magicBonus;
                speedBonus += heroes[i].ringL.speedBonus;
                armorBonus += heroes[i].ringL.armorBonus;
                resistBonus += heroes[i].ringL.resistBonus;
            }
            if (heroes[i].ringR != null) {
                hpBonus += heroes[i].ringR.hpBonus;
                mpBonus += heroes[i].ringR.mpBonus;
                attackBonus += heroes[i].ringR.attackBonus;
                defenseBonus += heroes[i].ringR.defenseBonus;
                magicBonus += heroes[i].ringR.magicBonus;
                speedBonus += heroes[i].ringR.speedBonus;
                armorBonus += heroes[i].ringR.armorBonus;
                resistBonus += heroes[i].ringR.resistBonus;
            }
            if (heroes[i].mainHand != null) {
                hpBonus += heroes[i].mainHand.hpBonus;
                mpBonus += heroes[i].mainHand.mpBonus;
                attackBonus += heroes[i].mainHand.attackBonus;
                defenseBonus += heroes[i].mainHand.defenseBonus;
                magicBonus += heroes[i].mainHand.magicBonus;
                speedBonus += heroes[i].mainHand.speedBonus;
                armorBonus += heroes[i].mainHand.armorBonus;
                resistBonus += heroes[i].mainHand.resistBonus;
            }
            if (heroes[i].offHand != null) {
                hpBonus += heroes[i].offHand.hpBonus;
                mpBonus += heroes[i].offHand.mpBonus;
                attackBonus += heroes[i].offHand.attackBonus;
                defenseBonus += heroes[i].offHand.defenseBonus;
                magicBonus += heroes[i].offHand.magicBonus;
                speedBonus += heroes[i].offHand.speedBonus;
                armorBonus += heroes[i].offHand.armorBonus;
                resistBonus += heroes[i].offHand.resistBonus;
            }

            heroes[i].hp.bonus += hpBonus;
            heroes[i].mp.bonus += mpBonus;
            heroes[i].hp.SetToMax();
            heroes[i].mp.SetToMax();
            heroes[i].attack.bonus += attackBonus;
            heroes[i].defense.bonus += defenseBonus;
            heroes[i].magic.bonus += magicBonus;
            heroes[i].speed.bonus += speedBonus;
            heroes[i].armor.bonus += armorBonus;
            heroes[i].resist.bonus += resistBonus;
        }
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

            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentXp", heroes[i].xp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentHp", heroes[i].hp.current);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_maximumHp", heroes[i].hp.baseMax);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentMp", heroes[i].mp.current);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_maximumMp", heroes[i].mp.baseMax);
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

            heroes[i].xp = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentXp");
            heroes[i].hp.current = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentHp");
            heroes[i].hp.baseMax = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_maximumHp");
            heroes[i].mp.current = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentMp");
            heroes[i].mp.baseMax = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_maximumMp");
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
