﻿using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour {

    public Image buttonImage;
    public Text nameText;
    public Text quantityText;
    public int buttonId;
    public ButtonLongPress buttonLongPress;

    public void Press() {
        // if (GameMenu.instance.gameMenu.activeInHierarchy) {
        //     if (InventoryManager.instance.inventory[buttonId] != string.Empty) {
        //         GameMenu.instance.SelectItem(InventoryManager.instance.FindItemReference(InventoryManager.instance.inventory[buttonId]));
        //     }
        // }
        // else if (Shop.instance.shopMenu.activeInHierarchy) {
        //     if (Shop.instance.buyWindow.activeInHierarchy) {
        //         Shop.instance.SelectBuyItem(InventoryManager.instance.FindItemReference(Shop.instance.itemsForSale[buttonId]));
        //     }
        //     else if (Shop.instance.sellWindow.activeInHierarchy) {
        //         Shop.instance.SelectSellItem(InventoryManager.instance.FindItemReference(InventoryManager.instance.inventory[buttonId]));
        //     }
        // } else if (InventoryManager.instance.battleActive) {
        //     if (InventoryManager.instance.inventory[buttonId] != string.Empty) {
        //         BattleManager.instance.SelectItem(InventoryManager.instance.FindItemReference(InventoryManager.instance.inventory[buttonId]));
        //     }
        // }
    }
}
