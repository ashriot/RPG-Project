using UnityEngine;

public class Shopkeeper : MonoBehaviour {

    private bool canOpen;

    public string[] itemsForSale = new string[40];

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (canOpen /* && PlayerController.instance.canMove */ && Input.GetButtonDown("Fire1") && !Shop.instance.shopMenu.activeInHierarchy) {
            Shop.instance.itemsForSale = itemsForSale;

            Shop.instance.OpenShop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canOpen = true;
        }    
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canOpen = false;
        }    
    }
}
