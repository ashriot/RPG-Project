﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public string mainMenuScene;
    public string loadGameScene;

    // Start is called before the first frame update
    void Start() {
        UIFade.instance.FadeIn();
        AudioManager.instance.PlaySfx("test");

        PlayerController.instance.gameObject.SetActive(false);
        GameMenu.instance.gameObject.SetActive(false);
        BattleManager.instance.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void QuitToMain() {
        Destroy(InventoryManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);

        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLastSave() {
        Destroy(InventoryManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
    
        SceneManager.LoadScene(loadGameScene);
    }
}
