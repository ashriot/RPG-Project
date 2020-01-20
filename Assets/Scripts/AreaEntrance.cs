using UnityEngine;

public class AreaEntrance : MonoBehaviour {
    
    public string areaTransitionName;

    // Start is called before the first frame update
    void Start() {
        if (areaTransitionName == PlayerController.instance.areaTransitionName) {
            PlayerController.instance.transform.position = this.transform.position;
        
            UIFade.instance.FadeIn();
            GameManager.instance.fadingBetweenAreas = false;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
