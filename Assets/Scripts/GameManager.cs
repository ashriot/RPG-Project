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
        // InventoryManager.instance.AddItemById("bodyLinenShirt", 4);
        // InventoryManager.instance.AddItemById("weapRustyDagger", 4);
        // InventoryManager.instance.AddItemById("feetWornBoots", 4);
        for (var h = 0; h < heroes.Length; h++) {
            heroes[h] = Instantiate(heroes[h]);
            if (heroes[h].handEquipment[0] == null) {
                heroes[h].handEquipment[0] = InventoryManager.instance.GetItemReference("weapUnarmed") as Weapon;
            }
        }

        UpdateHeroesEquipmentBonuses();
        
        for (var h = 0; h < heroes.Length; h++) {
            heroes[h].hp.SetToMax();
            heroes[h].mp.SetToMax();
        }
    }

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

    public void UpdateHeroesEquipmentBonuses(int? heroId = null, EquipmentSlots? slot = null) {
        if (heroId != null) {
            var id = (int)heroId;
            heroes[id].statBonuses = new int[(int)Stats.Count];
            UpdateHeroEquipmentBonus(id, slot);
            UpdateHeroStatBonuses(heroes[id]);
        } else {
            for (var h = 0; h < heroes.Length; h++) {
                heroes[h].statBonuses = new int[(int)Stats.Count];
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
                UpdateHeroStatBonuses(heroes[h]);
            }
        }
    }

    public void RemoveEquipmentBonus(int heroId, EquipmentSlots slot) {
        var slotId = (int)slot;
        heroes[heroId].statBonuses = new int[(int)Stats.Count];
        for (var i = 0; i < (int)Stats.Count; i++) {
            if (slotId < 6) {
                if (heroes[heroId].bodyEquipment[slotId] != null) {
                    heroes[heroId].statBonuses[i] -= heroes[heroId].bodyEquipment[slotId].statBonuses[i];
                } 
            } else { // hands
                heroes[heroId].statBonuses[i] -= heroes[heroId].handEquipment[slotId - 6].statBonuses[i];
            }
        }
        UpdateHeroStatBonuses(heroes[heroId]);
    }

    private void UpdateHeroStatBonuses(Hero hero) {
        hero.hp.bonus += hero.statBonuses[(int)Stats.Hp];
        hero.hp.Increase(hero.hp.bonus);
        hero.mp.bonus += hero.statBonuses[(int)Stats.Mp];
        hero.mp.Increase(hero.mp.bonus);
        hero.attack.bonus += hero.statBonuses[(int)Stats.Attack];
        hero.defense.bonus += hero.statBonuses[(int)Stats.Defense];
        hero.magic.bonus += hero.statBonuses[(int)Stats.Magic];
        hero.speed.bonus += hero.statBonuses[(int)Stats.Speed];
        hero.armor.bonus += hero.statBonuses[(int)Stats.Armor];
        hero.resist.bonus += hero.statBonuses[(int)Stats.Resist];
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
