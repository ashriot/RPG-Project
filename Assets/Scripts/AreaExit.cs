﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour {

    public string areaToLoad;
    public string areaTransitionName;
    public AreaEntrance entrance;
    
    public float waitToLoad = .8f;

    private bool shouldLoadAfterFade;

    // Start is called before the first frame update
    void Start() {
        entrance.areaTransitionName = this.areaTransitionName;
    }

    // Update is called once per frame
    void Update() {
        if (shouldLoadAfterFade) {
            waitToLoad -= Time.deltaTime;
            
            if (waitToLoad <= 0) {
                shouldLoadAfterFade = false;
                SceneManager.LoadScene(areaToLoad);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            //SceneManager.LoadScene(areaToLoad);
            shouldLoadAfterFade = true;
            GameManager.instance.fadingBetweenAreas = true;

            UIFade.instance.Expand();

            PlayerController.instance.areaTransitionName = areaTransitionName;
        }
    }
}
