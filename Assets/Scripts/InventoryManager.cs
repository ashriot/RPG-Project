using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager instance;
    
    public List<Item> referenceItems;
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

    public void AddItem(string id) {
        var item = FetchItemFromInventory(id);
        inventory.Add(item);
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
        var item = referenceItems.Find(i => i.id == id);

        if (item == null){
            Debug.LogError("Item '" + id + "' does not exist!");
            return null;
        }

        return item;
    }
}
