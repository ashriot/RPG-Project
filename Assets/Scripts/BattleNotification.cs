using UnityEngine;
using UnityEngine.UI;

public class BattleNotification : MonoBehaviour {

    public float awakeTime;
    public Text text;

    private float awakeCounter;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (awakeCounter > 0) {
            awakeCounter -= Time.deltaTime;
            if (awakeCounter <= 0) {
                gameObject.SetActive(false);
            }
        }
    }

    public void Activate() {
        gameObject.SetActive(true);
        awakeCounter = awakeTime;
    }
}
