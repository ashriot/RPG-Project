using UnityEngine;
using UnityEngine.UI;

public class BattleActionDisplay : MonoBehaviour {

    public Text text;

    public float lifetime = 1f;
    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        Destroy(gameObject, lifetime);
    }

    public void SetText(string name) {
        text.text = name;
    }
}
