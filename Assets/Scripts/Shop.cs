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

        goldText.text = GameManager.instance.currentGold.ToString("N0");
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
                buyItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(itemsForSale[i]).itemSprite;
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
        GameManager.instance.SortItems();
        for (var i = 0; i < sellItemButtons.Length; i++ ) {
            sellItemButtons[i].buttonId = i;

            if (GameManager.instance.inventory[i] != "") {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true);
                sellItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.inventory[i]).itemSprite;
                sellItemButtons[i].quantityText.text = GameManager.instance.inventoryQuantity[i].ToString();
            } else {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false);
                sellItemButtons[i].quantityText.text = string.Empty;
            }
        }
    }

    public void SelectBuyItem(Item item) {
        selectedItem = item;
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.description;
        buyItemValue.text = "Cost: " + selectedItem.goldValue + " gold";
    }

    public void SelectSellItem(Item item) {
        selectedItem = item;
        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.description;
        sellItemValue.text = "Value: " + Mathf.FloorToInt(selectedItem.goldValue * 0.5f) + " gold";
    }

    public void BuyItem() {
        if (selectedItem != null) {
            if (GameManager.instance.currentGold >= selectedItem.goldValue) {
                GameManager.instance.currentGold -= selectedItem.goldValue;
                GameManager.instance.AddItem(selectedItem.itemName);
            }

            goldText.text = GameManager.instance.currentGold.ToString("N0");
        }
    }

    public void SellItem() {
        if (selectedItem != null) {
            GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.goldValue * .5f);
            GameManager.instance.RemoveItem(selectedItem.itemName);
            goldText.text = GameManager.instance.currentGold.ToString("N0");
            ShowSellItems();
        }
    }
}
