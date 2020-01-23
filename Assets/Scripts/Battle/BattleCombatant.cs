using System.Collections.Generic;
using UnityEngine;

public class BattleCombatant : MonoBehaviour {

    public string id;
    public Resource hp, mp;
    public int deflect;
    public int barrier;
    public int level, xp, attack, defense, magic, speed, armor, resist, minDamage, maxDamage, tp;
    public float tpChanceBase;
    public float tpChance;
    public int ticks;
    public float delay = 1f;
    public bool isPlayer;
    public bool isActive;
    public bool isDead { get { return hp.current <= 0; } }
    public bool isCharging;
    public string chargedAbilityName;
    public int targetId;
    public bool isTaunting = false;
    public bool isHiding = false;
    public string classSkillName, classSkillDescription;
    public string[] abilities;
    public string[] spells;
    public List<StatusEffect> statusEffects;
    public Hands mainHand, offHand;

    public bool isDualWielding {
        get {
            if (mainHand != null && offHand != null && offHand.equipmentType == EquipmentTypes.LightWeapon) {
                return true;
            }
            else {
                return false;
            }
        }
    }

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
