using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Shield", menuName = "Shield")]
public class Shield : Hands {

    public float blockChance;
    public int blockAmount;

    public override int GetAtkOrBlkValue() {
        return (int)(blockChance * 100);
    }

    public override int GetDmgOrAmtValue() {
        return blockAmount;
    }

    public override string GetAtkOrBlkString(int bonus = 0) {
        return (blockChance * 100) + "% Block Chance";
    }

    public override string GetDmgOrAmtString() {
        return "Blocks " + blockAmount.ToString() + " dmg.";
    }
    
    public override string GetStatsString() {
        var result = string.Empty;

        result += (blockChance*100) + "% chance to block " + blockAmount + " DMG";

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
