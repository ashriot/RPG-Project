using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {

    public Text damageText;

    public float lifetime = 1f;
    public float moveSpeed = 1f;

    public float placementJitter = .5f;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        Destroy(gameObject, lifetime);
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime, 0f);
    }

    public void SetDamage(int damageValue) {
        var color = new Color(255f, 255f, 255f, 255f);
        if (damageValue < 0) { // healing
            damageText.text = "+" + (-1 * damageValue) + "HP";
            color = new Color(.2f, 1f, .35f, 1f);
        } else {
            damageText.text = damageValue.ToString("N0");
        }
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0f);
        damageText.color = color;
    }
}
