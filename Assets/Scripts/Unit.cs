using System.ComponentModel;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    public string id;
    public new string name;
    public string className;
    public Animator animator;
    public Sprite aliveSprite, deadSprite;
    public bool isActive;
    public bool isDead { get { return (hp.current == 0); } }

    public int level;
    public int xp;

    public Resource hp;
    public Resource mp;

    public Stat attack, defense, magic, speed, armor, resist, critPower;

    public EquippableItem head, body, arms, feet, ringL, ringR;
    public Hands mainHand, offHand;

    public string GetLevelAndClass() {
        return "Lv." + level + " " + className;
    }

    // Skills
}
