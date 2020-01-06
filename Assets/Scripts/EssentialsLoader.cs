using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour {

    public GameObject UIScreen;
    public GameObject player;
    public GameObject gameManager;
    public GameObject audioManager;
    public GameObject battleManager;

    // Start is called before the first frame update
    void Start() {
        if (UIFade.instance == null) {
            UIFade.instance = Instantiate(UIScreen).GetComponent<UIFade>();
        }

        if (PlayerController.instance == null) {
            PlayerController.instance = Instantiate(player, new Vector3(0, 0, 0), transform.rotation).GetComponent<PlayerController>();
        }

        if (InventoryManager.instance == null) {
            Instantiate(gameManager);
        }

        if (AudioManager.instance == null) {
            Instantiate(audioManager);
        }

        if (BattleManager.instance == null) {
            Instantiate(battleManager);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
