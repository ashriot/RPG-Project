using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {

    public Text damageText;

    public float lifetime;
    public float moveSpeed;

    public float placementJitter;
    public float angleJitter;

    private float movementAngle;
    private int direction;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        Destroy(gameObject, lifetime);
        transform.position += new Vector3(moveSpeed * Time.deltaTime * direction, moveSpeed * movementAngle * Time.deltaTime, 0f);
    }

    public void SetDamage(int damageValue, bool isCrit, bool isGraze, bool isMiss, bool isPlayer) {
        if (isPlayer) {
            direction = 1;
        } else {
            direction = -1;
        }

        damageText.text = damageValue.ToString("N0");
        movementAngle = Random.Range(0, angleJitter);
        
        if (isCrit) {
            damageText.color = Color.yellow;
            damageText.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
            damageText.text += "!";
            moveSpeed *= 1.25f;
            movementAngle = 1f;
        } else if (isGraze) {
            damageText.color = Color.gray;
            movementAngle = -.33f;
        } else if (isMiss) {
            damageText.color = Color.gray;
            moveSpeed *= .5f;
            damageText.text = "Miss!";
            movementAngle = -1f;
        }

        var startingOffset = isPlayer ? 30 : -30;
        transform.position += new Vector3(startingOffset, 0, 0f);
    }
}
