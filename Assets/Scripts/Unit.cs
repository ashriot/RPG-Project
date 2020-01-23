using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    public string id;
    public new string name;
    public string className;
    public Animator unitAnimator;
    public RuntimeAnimatorController portraitAnimatorController;
    public Sprite portrait, aliveSprite, deadSprite;

    public bool isActive;
    public bool isKOed { get { return (hp.current == 0 && constitution.current > 0); } }
    public bool isDead { get { return (hp.current == 0 && constitution.current == 0); } }
    
    public string classSkillName;
    public string classSkillDescription;
    public string[] skills;
    public string[] spells;

    public Resource hp;
    public Resource mp;
    public Resource constitution;
    public int deflect, barrier;

    public Stat attack, defense, magic, speed, armor, resist, critPower;
    public int[] statBonuses = new int[(int)Stats.Count];
    
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    // Skills
}
