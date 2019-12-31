using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour {

    public bool isPlayer;
    public string[] battleActions;
    public string characterName;
    public int currentHp, maxHp, currentMp, maxMp, attack, defense, magic, speed;
    public float timeTillTurn;
    public bool isDead;
    public List<StatusEffect> statusEffects;

    public SpriteRenderer spriteRenderer;
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
