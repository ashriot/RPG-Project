using UnityEngine;

[System.Serializable]
public abstract class Item : ScriptableObject {
    
    [Header("Basic Details")]
    public string id;
    public ItemType itemType;
    public new string name;
    public string description;
    public int goldValue;
    public Sprite sprite;

    public abstract void Use(int targetId);
    public abstract void Discard();
        //var selectedCharacter = GameManager.instance.playerStats[id];

        // if (itemType == ItemType.item) {
        //     if (affectHp) {
        //         selectedCharacter.currentHp += potencyValue;

        //         if (selectedCharacter.currentHp > selectedCharacter.maxHp) {
        //             selectedCharacter.currentHp = selectedCharacter.maxHp;
        //         }
        //     }
            
        //     if (affectMp) {
        //         selectedCharacter.currentMp += potencyValue;

        //         if (selectedCharacter.currentMp > selectedCharacter.maxMp) {
        //             selectedCharacter.currentMp = selectedCharacter.maxMp;
        //         }
        //     }
            
        //     if (boostAttack) {
        //         selectedCharacter.attack += potencyValue;
        //     }
            
        //     if (boostDefense) {
        //         selectedCharacter.defense += potencyValue;
        //     }
        // }

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

        //GameManager.instance.RemoveItem(itemName);
}