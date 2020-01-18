using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : Hands {

    public int baseAttack;
    public int minimumDamage;
    public int maximumDamage;
    public float delay;
    public StatTypes hitType = StatTypes.Attack;
    public StatTypes avoidType = StatTypes.Defense;
    public string weaponTechniqueName;
    public int weaponTechniqueRank;

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
    
    public override string GetStatsString() {
        var result = string.Empty;

        result += "\n" + baseAttack + " ATK" + "\n" + minimumDamage + "-" + maximumDamage + " DMG\n" + (delay*100) + "% Delay";
        if (hpBonus > 0) {
            result += "\nHP+" + hpBonus;
        }
        if (mpBonus > 0) {
            result += "\nMP+" + mpBonus;
        }
        if (attackBonus > 0) {
            result += "\nATK+" + attackBonus;
        }
        if (defenseBonus > 0) {
            result += "\nDEF+" + defenseBonus;
        }
        if (magicBonus > 0) {
            result += "\nMAG+" + magicBonus;
        }
        if (speedBonus > 0) {
            result += "\nSPD+" + speedBonus;
        }
        if (armorBonus > 0) {
            result += "\nARM+" + armorBonus;
        }
        if (resistBonus > 0) {
            result += "\nRES+" + resistBonus;
        }

        return result;
    }
}
