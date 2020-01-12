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
        foreach(var hero in heroes) {
            hero.hp.current = hero.initialValues.hp.current;
            hero.hp.maximum = hero.initialValues.hp.maximum;
            hero.hp.totalBonus = hero.initialValues.hp.totalBonus;
            hero.mp.current = hero.initialValues.mp.current;
            hero.mp.maximum = hero.initialValues.mp.maximum;
            hero.mp.totalBonus = hero.initialValues.mp.totalBonus;
            hero.constitution.current = hero.initialValues.constitution.current;
            hero.constitution.maximum = hero.initialValues.constitution.maximum;
            hero.xp = hero.initialValues.xp;
            hero.sp = hero.initialValues.sp;
            hero.totalSp = hero.initialValues.totalSp;
            hero.attack.baseValue = hero.initialValues.attack.baseValue;
            hero.attack.totalBonus = hero.initialValues.attack.totalBonus;
            hero.defense.baseValue = hero.initialValues.defense.baseValue;
            hero.defense.totalBonus = hero.initialValues.defense.totalBonus;
            hero.magic.baseValue = hero.initialValues.magic.baseValue;
            hero.magic.totalBonus = hero.initialValues.magic.totalBonus;
            hero.speed.baseValue = hero.initialValues.speed.baseValue;
            hero.speed.totalBonus = hero.initialValues.speed.totalBonus;
            hero.armor.baseValue = hero.initialValues.armor.baseValue;
            hero.armor.totalBonus = hero.initialValues.armor.totalBonus;
            hero.resist.baseValue = hero.initialValues.resist.baseValue;
            hero.resist.totalBonus = hero.initialValues.resist.totalBonus;
            hero.critPower.baseValue = hero.initialValues.critPower.baseValue;
            hero.critPower.totalBonus = hero.initialValues.critPower.totalBonus;
            hero.skills = hero.initialValues.skills;
            hero.spells = hero.initialValues.spells;

            hero.head = hero.initialValues.head;
            hero.body = hero.initialValues.body;
            hero.arms = hero.initialValues.arms;
            hero.feet = hero.initialValues.feet;
            hero.ringL = hero.initialValues.ringL;
            hero.ringR = hero.initialValues.ringR;
            hero.mainHand = hero.initialValues.mainHand;
            hero.offHand = hero.initialValues.offHand;

            // check equipment
            var hpBonus = 0;
            var mpBonus = 0;
            var attackBonus = 0;
            var defenseBonus = 0;
            var magicBonus = 0;
            var speedBonus = 0;
            var armorBonus = 0;
            var resistBonus = 0;

            if (hero.head != null) {
                hpBonus += hero.head.hpBonus;
                mpBonus += hero.head.mpBonus;
                attackBonus += hero.head.attackBonus;
                defenseBonus += hero.head.defenseBonus;
                magicBonus += hero.head.magicBonus;
                speedBonus += hero.head.speedBonus;
                armorBonus += hero.head.armorBonus;
                resistBonus += hero.head.resistBonus;
            }
            if (hero.body != null) {
                hpBonus += hero.body.hpBonus;
                mpBonus += hero.body.mpBonus;
                attackBonus += hero.body.attackBonus;
                defenseBonus += hero.body.defenseBonus;
                magicBonus += hero.body.magicBonus;
                speedBonus += hero.body.speedBonus;
                armorBonus += hero.body.armorBonus;
                resistBonus += hero.body.resistBonus;
            }
            if (hero.arms != null) {
                hpBonus += hero.arms.hpBonus;
                mpBonus += hero.arms.mpBonus;
                attackBonus += hero.arms.attackBonus;
                defenseBonus += hero.arms.defenseBonus;
                magicBonus += hero.arms.magicBonus;
                speedBonus += hero.arms.speedBonus;
                armorBonus += hero.arms.armorBonus;
                resistBonus += hero.arms.resistBonus;
            }
            if (hero.feet != null) {
                hpBonus += hero.feet.hpBonus;
                mpBonus += hero.feet.mpBonus;
                attackBonus += hero.feet.attackBonus;
                defenseBonus += hero.feet.defenseBonus;
                magicBonus += hero.feet.magicBonus;
                speedBonus += hero.feet.speedBonus;
                armorBonus += hero.feet.armorBonus;
                resistBonus += hero.feet.resistBonus;
            }
            if (hero.ringL != null) {
                hpBonus += hero.ringL.hpBonus;
                mpBonus += hero.ringL.mpBonus;
                attackBonus += hero.ringL.attackBonus;
                defenseBonus += hero.ringL.defenseBonus;
                magicBonus += hero.ringL.magicBonus;
                speedBonus += hero.ringL.speedBonus;
                armorBonus += hero.ringL.armorBonus;
                resistBonus += hero.ringL.resistBonus;
            }
            if (hero.ringR != null) {
                hpBonus += hero.ringR.hpBonus;
                mpBonus += hero.ringR.mpBonus;
                attackBonus += hero.ringR.attackBonus;
                defenseBonus += hero.ringR.defenseBonus;
                magicBonus += hero.ringR.magicBonus;
                speedBonus += hero.ringR.speedBonus;
                armorBonus += hero.ringR.armorBonus;
                resistBonus += hero.ringR.resistBonus;
            }
            if (hero.mainHand != null) {
                hpBonus += hero.mainHand.hpBonus;
                mpBonus += hero.mainHand.mpBonus;
                attackBonus += hero.mainHand.attackBonus;
                defenseBonus += hero.mainHand.defenseBonus;
                magicBonus += hero.mainHand.magicBonus;
                speedBonus += hero.mainHand.speedBonus;
                armorBonus += hero.mainHand.armorBonus;
                resistBonus += hero.mainHand.resistBonus;
            }
            if (hero.offHand != null) {
                hpBonus += hero.offHand.hpBonus;
                mpBonus += hero.offHand.mpBonus;
                attackBonus += hero.offHand.attackBonus;
                defenseBonus += hero.offHand.defenseBonus;
                magicBonus += hero.offHand.magicBonus;
                speedBonus += hero.offHand.speedBonus;
                armorBonus += hero.offHand.armorBonus;
                resistBonus += hero.offHand.resistBonus;
            }

            hero.hp.totalBonus += hpBonus;
            hero.mp.totalBonus += mpBonus;
            hero.hp.SetToMax();
            hero.mp.SetToMax();
            hero.attack.totalBonus += attackBonus;
            hero.defense.totalBonus += defenseBonus;
            hero.magic.totalBonus += magicBonus;
            hero.speed.totalBonus += speedBonus;
            hero.armor.totalBonus += armorBonus;
            hero.resist.totalBonus += resistBonus;

            for (var i = 0; i < hero.initialValues.attributeSkillValues.Length; i++) {
                hero.attributeSkillValues[i] = hero.initialValues.attributeSkillValues[i];
            }
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

            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Level", heroes[i].level);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentXp", heroes[i].xp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentHp", heroes[i].hp.current);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_maximumHp", heroes[i].hp.maximum);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentMp", heroes[i].mp.current);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_maximumMp", heroes[i].mp.maximum);
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
            heroes[i].hp.maximum = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_maximumHp");
            heroes[i].mp.current = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_CurrentMp");
            heroes[i].mp.maximum = PlayerPrefs.GetInt("Player_" + heroes[i].name + "_maximumMp");
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
