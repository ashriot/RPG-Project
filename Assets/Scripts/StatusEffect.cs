using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "StatusEffect", menuName = "Status Effect")]
public class StatusEffect : ScriptableObject {

    public new string name;
    public string description;
    public int duration;
    public int potency;
}

public enum StatusEffectTriggers {
    Instant,
    BeginningOfTurn,
    EndOfTurn,
    ReceivingMeleeAttack,
    ReceivingMissileAttack,
    ReceivingMagicAttack,
    AllyAttack
}
