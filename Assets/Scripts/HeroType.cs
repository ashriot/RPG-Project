using UnityEngine;

[CreateAssetMenu(fileName = "Hero", menuName = "Hero")]
public class HeroType : ScriptableObject {
    public string id;
    public new string name;
    public string className;
    public string classSkillName;
    public Animator animator;
    public Sprite aliveSprite, deadSprite;
    public bool isPlayer;
    public bool isActive;

    public int xp;
    public int sp, totalSp;

    public Resource hp;
    public Resource mp;
    public Resource constitution;

    public Stat attack, defense, magic, speed, armor, resist, critPower;

    // attribute skills
    public int[] attributeSkillValues;

    public string[] skills;
    public string[] spells;

    public EquippableItem head, body, arms, feet, ringL, ringR;
    public Hands mainHand, offHand;
}
