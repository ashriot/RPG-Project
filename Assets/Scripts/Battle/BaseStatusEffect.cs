using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Effect", menuName = "Base Status Effect")]
public class BaseStatusEffect : ScriptableObject {

    public new string name;
    public StatusEffectExpiries expiry;
    public string description;
    public int potency;
    public StatTypes statModified;
}

public enum StatusEffectExpiries {
    Instant,
    StartOfTurn,
    EndOfTurn,
    ReceivingMeleeAttack,
    ReceivingMissileAttack,
    ReceivingMagicAttack,
    AllyAttack
}
