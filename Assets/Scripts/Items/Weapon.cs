using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : Hands {

    public int baseAttack;
    public int minimumDamage;
    public int maximumDamage;
    public float delay;

    public override int GetAtkOrBlkValue() {
        return baseAttack;
    }

    public override int GetDmgOrAmtValue() {
        return (Random.Range(minimumDamage, maximumDamage));
    }

    public override string GetAtkOrBlkString(int bonus = 0) {
        return (baseAttack + bonus) + " ATK v. DEF";
    }

    public override string GetDmgOrAmtString() {
        return minimumDamage + "-" + maximumDamage + " Wpn. Dmg.";
    }
}
