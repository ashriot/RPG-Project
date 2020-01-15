using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager instance;
    
    public List<Item> refArms;
    public List<Item> refBody;
    public List<Item> refConsumables;
    public List<Item> refFeet;
    public List<Item> refHead;
    public List<Item> refRings;
    public List<Item> refShields;
    public List<Item> refSpells;
    public List<Item> refWeapons;
    public List<Item> refOthers;
    public List<Item> inventory;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string id, int qty) {
        var item = FindItemReference(id);
        for (var i = 0; i < qty; i++) {
            inventory.Add(item);
        }
    }

    public void RemoveItem(string id) {
        var item = FetchItemFromInventory(id);
        inventory.Remove(item);
    }

    public Item FetchItemFromInventory(string id) {
        var item = inventory.Find(i => i.id == id);

        if (item == null){
            Debug.LogError("Item '" + id + "' does not exist!");
            return null;
        }

        return item;
    }

    public Item FindItemReference(string id) {
        var prefix = id.Substring(0, 4);
        
        Item item;

        switch (prefix) {
            case "arms":
                item = refArms.Find(i => i.id == id);
                break;
            case "body":
                item = refBody.Find(i => i.id == id);
                break;
            case "cons":
                item = refConsumables.Find(i => i.id == id);
                break;
            case "feet":
                item = refFeet.Find(i => i.id == id);
                break;
            case "head":
                item = refHead.Find(i => i.id == id);
                break;
            case "ring":
                item = refRings.Find(i => i.id == id);
                break;
            case "shld":
                item = refShields.Find(i => i.id == id);
                break;
            case "spel":
                item = refSpells.Find(i => i.id == id);
                break;
            case "weap":
                item = refWeapons.Find(i => i.id == id);
                break;
            case "othr":
                item = refOthers.Find(i => i.id == id);
                break;
            default:
                item = null;
                break;
        }
        

        if (item == null){
            Debug.LogError("Item '" + id + "' does not exist!");
            return null;
        }

        return item;
    }
}
