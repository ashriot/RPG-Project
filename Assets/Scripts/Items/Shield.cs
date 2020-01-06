using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Shield", menuName = "Shield")]
public class Shield : Hands {

    public float blockChance;
    public int blockAmount;

    public override string GetAtkOrBlk() {
        return (blockChance * 100) + "%";
    }

    public override string GetDmgOrAmt() {
        return blockAmount.ToString();
    }
}
