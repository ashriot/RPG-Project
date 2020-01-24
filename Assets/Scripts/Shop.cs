using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    public static Shop instance;

    public GameObject shopMenu;
    public GameObject buyWindow;
    public GameObject sellWindow;

    public Text goldText;
    public string[] itemsForSale;

    public ItemButton[] buyItemButtons;
    public ItemButton[] sellItemButtons;

    public Item selectedItem;
    public Text buyItemName, buyItemDescription, buyItemValue;
    public Text sellItemName, sellItemDescription, sellItemValue;

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.K) && !shopMenu.activeInHierarchy) {
            OpenShop();
        }
    }

    public void OpenShop() {
        shopMenu.SetActive(true);
        OpenBuyWindow();

        GameManager.instance.shopActive = true;

        goldText.text = GameManager.instance.currentGoldPieces.ToString("N0");
    }

    public void CloseShop() {
        shopMenu.SetActive(false);

        GameManager.instance.shopActive = false;
    }

    public void OpenBuyWindow() {
        buyItemButtons[0].Press();

        buyWindow.SetActive(true);
        sellWindow.SetActive(false);

        for (var i = 0; i < buyItemButtons.Length; i++ ) {
            buyItemButtons[i].buttonId = i;

            if (itemsForSale[i] != string.Empty) {
                buyItemButtons[i].buttonImage.gameObject.SetActive(true);
                buyItemButtons[i].buttonImage.sprite = InventoryManager.instance.GetItemReference(itemsForSale[i]).sprite;
                buyItemButtons[i].quantityText.text = string.Empty;
            } else {
                buyItemButtons[i].buttonImage.gameObject.SetActive(false);
                buyItemButtons[i].quantityText.text = string.Empty;
            }
        }
    }

    public void OpenSellWindow() {
        sellItemButtons[0].Press();
        buyWindow.SetActive(false);
        sellWindow.SetActive(true);

        ShowSellItems();
    }

    private void ShowSellItems() {
        for (var i = 0; i < sellItemButtons.Length; i++ ) {
            sellItemButtons[i].buttonId = i;

            if (InventoryManager.instance.inventory[i].id != "") {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true);
                sellItemButtons[i].buttonImage.sprite = InventoryManager.instance.GetItemReference(InventoryManager.instance.inventory[i].id).sprite;
            } else {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false);
                sellItemButtons[i].quantityText.text = string.Empty;
            }
        }
    }

    public void SelectBuyItem(Item item) {
        selectedItem = item;
        buyItemName.text = selectedItem.name;
        buyItemDescription.text = selectedItem.description;
        buyItemValue.text = "Cost: " + selectedItem.goldValue + " gold";
    }

    public void SelectSellItem(Item item) {
        selectedItem = item;
        sellItemName.text = selectedItem.name;
        sellItemDescription.text = selectedItem.description;
        sellItemValue.text = "Value: " + Mathf.FloorToInt(selectedItem.goldValue * 0.5f) + " gold";
    }

    public void BuyItem() {
        if (selectedItem != null) {
            if (GameManager.instance.currentGoldPieces >= selectedItem.goldValue) {
                GameManager.instance.currentGoldPieces -= selectedItem.goldValue;
                InventoryManager.instance.AddItemById(selectedItem.id, 1);
            }

            goldText.text = GameManager.instance.currentGoldPieces.ToString("N0");
        }
    }

    public void SellItem() {
        if (selectedItem != null) {
            GameManager.instance.currentGoldPieces += Mathf.FloorToInt(selectedItem.goldValue * .5f);
            InventoryManager.instance.RemoveItemById(selectedItem.name);
            goldText.text = GameManager.instance.currentGoldPieces.ToString("N0");
            ShowSellItems();
        }
    }
}
