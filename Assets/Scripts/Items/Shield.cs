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
}
