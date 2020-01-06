using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour {

    public new string name;
    public int level, currentXp, currentHp, maxHp, currentMp, maxMp, attack, defense, magic, speed;
    public float timeTillTurn;
    public bool isPlayer;
    public bool isActive;
    public bool isDead;
    public string[] battleActions;
    public List<StatusEffect> statusEffects;

    public SpriteRenderer spriteRenderer;
    public Sprite portrait;
    public Sprite aliveSprite;
    public Sprite deadSprite;

    public float fadeSpeed = 1f;

    private bool shouldFade;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (shouldFade) {
            spriteRenderer.color = new Color(Mathf.MoveTowards(spriteRenderer.color.r, 1f, fadeSpeed * Time.deltaTime),
                Mathf.MoveTowards(spriteRenderer.color.g, 0f, fadeSpeed * Time.deltaTime),
                Mathf.MoveTowards(spriteRenderer.color.b, 0f, fadeSpeed * Time.deltaTime),
                Mathf.MoveTowards(spriteRenderer.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (spriteRenderer.color.a == 0) {
                gameObject.SetActive(false);
            }
        }
    }

    public void EnemyFade() {
        shouldFade = true;
    }
}
