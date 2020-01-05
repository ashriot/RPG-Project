using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public CharacterStats[] playerStats;
    public BattleCharacter[] heroes;
    public bool gameMenuOpen, dialogActive, fadingBetweenAreas, shopActive, battleActive;


    // AB: combine these
    public string[] inventory;
    public int[] inventoryQuantity;
    public Item[] referenceItems;

    public int currentGold;

    // Start is called before the first frame update
    void Start() {
        instance = this;

        DontDestroyOnLoad(gameObject);

        SortItems();
    }

    // Update is called once per frame
    void Update() {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive) {
            // PlayerController.instance.canMove = false;
        } else {
            // PlayerController.instance.canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.J)) {
            AddItem("Iron Armor");
            AddItem("Pile of Poo");

            RemoveItem("Health Potion");
            RemoveItem("Poopy Potion");
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            LoadData();
        }
    }

    public Item GetItemDetails(string itemName) {
        for (var i = 0; i < referenceItems.Length; i++) {
            if (referenceItems[i].itemName.ToLower() == itemName.ToLower()) {
                return referenceItems[i];
            }
        }

        // item could not be found
        Debug.LogError("'" + itemName + "' does not exist!");
        return null;
    }

    public void SortItems() {
        bool needsSorting = true;

        while (needsSorting) {
            needsSorting = false;
            for (var i = 0; i < inventory.Length - 1; i++) {
                if (inventory[i] == string.Empty) {
                    inventory[i] = inventory[i+1];
                    inventory[i+1] = string.Empty;

                    inventoryQuantity[i] = inventoryQuantity[i+1];
                    inventoryQuantity[i+1] = 0;

                    // check if inventory needs additional sorting
                    if (inventory[i] != string.Empty) {
                        needsSorting = true;
                    }
                }
            }
        }
    }

    public void AddItem(string itemName) {
        int newPosition = 0;
        bool foundPosition = false;

        for (var i = 0; i < inventory.Length; i++) {
            if (inventory[i] == string.Empty || inventory[i] == itemName) {
                newPosition = i;
                foundPosition = true;
                break;
            }
        }

        if (foundPosition) {
            bool itemExists = false;
            for (var i = 0; i < referenceItems.Length; i++) {
                if (referenceItems[i].itemName == itemName) {
                    itemExists = true;
                    break;
                }
            }
            if (itemExists) {
                inventory[newPosition] = itemName;
                inventoryQuantity[newPosition]++;
            } else {
                Debug.LogError("Item '" + itemName + "' doesn't exist! Check your shit!");
            }
        }
        GameMenu.instance.ShowItems();
    }

    public void RemoveItem(string itemName) {
        bool foundItem = false;
        int itemPosition = 0;

        for (var i = 0; i < inventory.Length; i++) {
            if (inventory[i] == itemName) {
                foundItem = true;
                itemPosition = i;
                break;
            }
        }

        if (foundItem) {
            inventoryQuantity[itemPosition]--;

            if (inventoryQuantity[itemPosition] <= 0) {
                inventory[itemPosition] = string.Empty;
            }
            GameMenu.instance.ShowItems();
        } else {
                Debug.LogError("Item '" + itemName + "' doesn't exist and couldn't be removed!");
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

            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Level", heroes[i].level);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentXp", heroes[i].currentXp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentHp", heroes[i].currentHp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_MaxHp", heroes[i].maxHp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_CurrentMp", heroes[i].currentMp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_MaxMp", heroes[i].maxMp);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Attack", heroes[i].attack);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Defense", heroes[i].defense);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Magic", heroes[i].magic);
            PlayerPrefs.SetInt("Player_" + heroes[i].name + "_Speed", heroes[i].speed);
        }

        // inventory data
        for (var i = 0; i < inventory.Length; i++) {
            PlayerPrefs.SetString("Inventory_" + i, inventory[i]);
            PlayerPrefs.SetInt("InventoryQuantity_" + i, inventoryQuantity[i]);
        }

    }

    public void LoadData() {
        // player location data
        var x = PlayerPrefs.GetFloat("Player_Position_x");
        var y = PlayerPrefs.GetFloat("Player_Position_y");
        var z = PlayerPrefs.GetFloat("Player_Position_z");
        PlayerController.instance.transform.position = new Vector3(x, y, z);

        // character data
        for (var i = 0; i < playerStats.Length; i++) {
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Active") == 1) {
                playerStats[i].gameObject.SetActive(true);
            } else {
                playerStats[i].gameObject.SetActive(false);
            }

            playerStats[i].level = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Level");
            playerStats[i].currentXp = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_CurrentXp");
            playerStats[i].currentHp = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_CurrentHp");
            playerStats[i].maxHp = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_MaxHp");
            playerStats[i].currentMp = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_CurrentMp");
            playerStats[i].maxMp = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_MaxMp");
            playerStats[i].attack = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Attack");
            playerStats[i].defense = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Defense");
            playerStats[i].magic = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Magic");
            playerStats[i].speed = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Speed");
        }

        for (var i = 0; i < inventory.Length; i++) {
            inventory[i] = PlayerPrefs.GetString("Inventory_" + i);
            inventoryQuantity[i] = PlayerPrefs.GetInt("InventoryQuantity_" + i);
        }
    }
}
