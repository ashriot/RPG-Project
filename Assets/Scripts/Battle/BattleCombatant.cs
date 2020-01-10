using System.Collections.Generic;
using UnityEngine;

public class BattleCombatant : MonoBehaviour {

    public string id;
    public Resource hp, mp;
    public int level, xp, attack, defense, magic, speed, armor, resist;
    public float initiative;
    public bool isPlayer;
    public bool isActive;
    public bool isDead;
    public string[] battleActions;
    public List<StatusEffect> statusEffects;

    public SpriteRenderer spriteRenderer;
    public RuntimeAnimatorController portraitAnimatorController;
    public Sprite portrait;
    public Sprite deadSprite;

    public float fadeSpeed = 1f;

    private bool shouldFade;

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
