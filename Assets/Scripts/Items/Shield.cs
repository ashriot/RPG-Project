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
