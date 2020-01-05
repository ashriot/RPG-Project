using UnityEngine;

public class Item : MonoBehaviour {
    
    [Header("Basic Details")]
    public ItemType itemType;
    public string itemName;
    public string description;
    public int goldValue;
    public Sprite itemSprite;

    [Header("Item Details")]
    public int potencyValue;
    public bool affectHp, affectMp, boostAttack, boostDefense, boostMagic, boostSpeed;

    [Header("Equipment Details")]
    public int weaponPower;
    public int armorPower;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Use(int id) {
        var selectedCharacter = GameManager.instance.playerStats[id];

        if (itemType == ItemType.item) {
            if (affectHp) {
                selectedCharacter.currentHp += potencyValue;

                if (selectedCharacter.currentHp > selectedCharacter.maxHp) {
                    selectedCharacter.currentHp = selectedCharacter.maxHp;
                }
            }
            
            if (affectMp) {
                selectedCharacter.currentMp += potencyValue;

                if (selectedCharacter.currentMp > selectedCharacter.maxMp) {
                    selectedCharacter.currentMp = selectedCharacter.maxMp;
                }
            }
            
            if (boostAttack) {
                selectedCharacter.attack += potencyValue;
            }
            
            if (boostDefense) {
                selectedCharacter.defense += potencyValue;
            }
        }

/*         if (itemType == ItemType.weapon) {
            if (selectedCharacter.equippedWeapon != string.Empty) {
                GameManager.instance.AddItem(selectedCharacter.equippedWeapon);
            }

            selectedCharacter.equippedWeapon = itemName;
            selectedCharacter.weaponPower = weaponPower;
        }

        if (itemType == ItemType.armor) {
            if (selectedCharacter.equippedArmor != string.Empty) {
                GameManager.instance.AddItem(selectedCharacter.equippedArmor);
            }

            selectedCharacter.equippedArmor = itemName;
            selectedCharacter.armorPower = armorPower;
        } */

        GameManager.instance.RemoveItem(itemName);
    }
}