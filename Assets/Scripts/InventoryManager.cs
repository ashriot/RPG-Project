using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager instance;
    
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
        var item = Instantiate(GetItemReference(id));
        for (var i = 0; i < qty; i++) {
            if (item.itemType == ItemTypes.Consumable) {
                var prevItem = FetchItemFromInventory(id);
                if (prevItem != null) {
                    prevItem.quantity++;
                } else {
                    inventory.Add(item);
                }
            } else {
                inventory.Add(item);
            }
        }
    }

    public void RemoveItemById(string id) {
        var item = FetchItemFromInventory(id);
        inventory.Remove(item);
    }

    public Item FetchItemFromInventory(string id) {
        var item = inventory.Find(i => i.id == id);

        if (item == null){
            Debug.Log("Item '" + id + "' is not in inventory!");
            return null;
        }

        return item;
    }

    public void SortInventory() {
        inventory.OrderByDescending(i => (int)(i.itemType))
            .ToList();
    }

    public Item GetItemReference(string id) {
        var folder = id.Substring(0, 4);
        var itemName = id.Substring(4, id.Length-4);
        var item = Instantiate(Resources.Load<Item>("Items/" + folder + "/" + itemName));

        if (item == null){
            Debug.LogError("Item '" + id + "' does not exist!");
            return null;
        }

        return item;
    }
}
