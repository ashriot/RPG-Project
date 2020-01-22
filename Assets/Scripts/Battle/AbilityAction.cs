using System.Collections.Generic;

[System.Serializable]
public class AbilityAction {
    public StatTypes hitType;
    public StatTypes avoidType;
    public DamageTypes damageType;
    public int hitModifier;
    public int avoidModifier;
    public float potencyModifier = 1f;
    public int minimumPotency;
    public int maximumPotency;
    public bool isWeaponAttack;
    public bool isEffectOnly;
    public TargetTypes targetType;
    public SplashTypes splashType;
    public float splashModifier = 1f;
    public AbilityFx abilityFx;
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public void Execute() {
        // ability execution code here
    }
}

public enum DamageTypes {
    Physical,
    Magical,
    True
}
