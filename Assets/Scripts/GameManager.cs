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
        for (var h = 0; h < heroes.Length; h++) {
            heroes[h] = Instantiate(heroes[h]);
        }

        UpdateHeroesEquipmentBonuses();
        
        for (var h = 0; h < heroes.Length; h++) {
            heroes[h].hp.SetToMax();
            heroes[h].mp.SetToMax();
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

    public void UpdateHeroesEquipmentBonuses(int? heroId = null, EquipmentSlots? slot = null) {
        if (heroId != null) {
            var id = (int)heroId;
            UpdateHeroEquipmentBonus(id, slot);
            heroes[id].hp.bonus = heroes[id].statBonuses[(int)Stats.Hp];
            heroes[id].hp.Increase(heroes[id].hp.bonus);
            heroes[id].mp.bonus = heroes[id].statBonuses[(int)Stats.Mp];
            heroes[id].mp.Increase(heroes[id].mp.bonus);
            heroes[id].attack.bonus = heroes[id].statBonuses[(int)Stats.Attack];
            heroes[id].defense.bonus = heroes[id].statBonuses[(int)Stats.Defense];
            heroes[id].magic.bonus = heroes[id].statBonuses[(int)Stats.Magic];
            heroes[id].speed.bonus = heroes[id].statBonuses[(int)Stats.Speed];
            heroes[id].armor.bonus = heroes[id].statBonuses[(int)Stats.Armor];
            heroes[id].resist.bonus = heroes[id].statBonuses[(int)Stats.Resist];
        } else {
            for (var h = 0; h < heroes.Length; h++) {
                for (var i = 0; i < (int)Stats.Count; i++) {
                    for (var j = 0; j < 6; j++) {
                        if (heroes[h].bodyEquipment[j] != null) {
                            heroes[h].statBonuses[i] += heroes[h].bodyEquipment[j].statBonuses[i];
                        }
                    }
                    for (var j = 0; j < 2; j++) {
                        if (heroes[h].handEquipment[j] != null) {
                            heroes[h].statBonuses[i] += heroes[h].handEquipment[j].statBonuses[i];
                        }
                    }
                }
                heroes[h].hp.bonus = heroes[h].statBonuses[(int)Stats.Hp];
                heroes[h].hp.Increase(heroes[h].hp.bonus);
                heroes[h].mp.bonus = heroes[h].statBonuses[(int)Stats.Mp];
                heroes[h].mp.Increase(heroes[h].mp.bonus);
                heroes[h].attack.bonus = heroes[h].statBonuses[(int)Stats.Attack];
                heroes[h].defense.bonus = heroes[h].statBonuses[(int)Stats.Defense];
                heroes[h].magic.bonus = heroes[h].statBonuses[(int)Stats.Magic];
                heroes[h].speed.bonus = heroes[h].statBonuses[(int)Stats.Speed];
                heroes[h].armor.bonus = heroes[h].statBonuses[(int)Stats.Armor];
                heroes[h].resist.bonus = heroes[h].statBonuses[(int)Stats.Resist];
            }
        }
    }

    public void RemoveEquipmentBonus(int heroId, EquipmentSlots slot) {
        var slotId = (int)slot;
        for (var i = 0; i < (int)Stats.Count; i++) {
                if (heroes[heroId].bodyEquipment[slotId] != null) {
                    if (slotId < 6) {
                        heroes[heroId].statBonuses[i] -= heroes[heroId].bodyEquipment[slotId].statBonuses[i];
                    } else { // hands
                        heroes[heroId].statBonuses[i] -= heroes[heroId].handEquipment[slotId].statBonuses[i];
                }
            }
        }
    }

    private void UpdateHeroEquipmentBonus(int heroId, EquipmentSlots? slot) {
        if (slot != null) {
            UpdateHeroSlotBonus(heroId, (EquipmentSlots)slot);
        }
        else {
            for (var i = 0; i < (int)EquipmentSlots.Count; i++) {
                UpdateHeroSlotBonus(heroId, (EquipmentSlots)i);
            }
        }
    }

    private void UpdateHeroSlotBonus(int heroId, EquipmentSlots slot) {
        var slotId = (int)slot;
        for (var i = 0; i < (int)Stats.Count; i++) {
            if (slotId < 6) {
                if (heroes[heroId].bodyEquipment[slotId] != null) {
                    heroes[heroId].statBonuses[i] += heroes[heroId].bodyEquipment[slotId].statBonuses[i];
                }
            } else { // hands
                if (heroes[heroId].handEquipment[slotId-6] != null) {
                    heroes[heroId].statBonuses[i] += heroes[heroId].handEquipment[slotId-6].statBonuses[i];
                }
            }
        }
    }

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
