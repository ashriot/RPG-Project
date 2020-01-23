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

        if (statBonuses[(int)Stats.Hp] > 0) {
            result += "\nHP+" + statBonuses[(int)Stats.Hp];
        }
        if (statBonuses[(int)Stats.Mp] > 0) {
            result += "\nMP+" + statBonuses[(int)Stats.Mp];
        }
        if (statBonuses[(int)Stats.Attack] > 0) {
            result += "\nATK+" + statBonuses[(int)Stats.Attack];
        }
        if (statBonuses[(int)Stats.Defense] > 0) {
            result += "\nDEF+" + statBonuses[(int)Stats.Defense];
        }
        if (statBonuses[(int)Stats.Magic] > 0) {
            result += "\nMAG+" + statBonuses[(int)Stats.Magic];
        }
        if (statBonuses[(int)Stats.Speed] > 0) {
            result += "\nSPD+" + statBonuses[(int)Stats.Speed];
        }
        if (statBonuses[(int)Stats.Armor] > 0) {
            result += "\nARM+" + statBonuses[(int)Stats.Armor];
        }
        if (statBonuses[(int)Stats.Resist] > 0) {
            result += "\nRES+" + statBonuses[(int)Stats.Resist];
        }

        return result;
    }
}
