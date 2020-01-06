using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : Hands {

    public int baseAttack;
    public int minimumDamage;
    public int maximumDamage;
    public float delay;

    public override string GetAtkOrBlk() {
        return baseAttack.ToString();
    }

    public override string GetDmgOrAmt() {
        return minimumDamage + " - " + maximumDamage;
    }
}
