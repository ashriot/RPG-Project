using UnityEngine;
using System.Collections.Generic;

[System.Serializable, CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
public class Ability : ScriptableObject {
    public new string name;
    public string description;
    public Sprite sprite;
    public StatTypes hitType;
    public StatTypes avoidType;
    public int mpCost;
    public int tpCost;
    public float potencyModifier = 1f;
    public int minimumPotency;
    public int maximumPotency;
    public bool isWeaponAttack;
    public ChargeTypes chargeType;
    public float chargeDelay = 1f;
    public TargetTypes targetType;
    public SplashTypes splashType;
    public float splashModifier = 1f;
    public List<AbilityAction> actions = new List<AbilityAction>();
    public List<Notes> notes = new List<Notes>();
}

public enum StatTypes {  // the source stat for determining accuracy
    Attack,
    Defense,
    Magic,
    Speed
}

public enum ChargeTypes {
    Instant,    // no delay
    Charge,     // must charge to use
    Recharge    // must recharge after use
}

public enum TargetTypes {
    Self,
    AlliesOnly,
    AnyHero,
    OneEnemy,
    AllEnemies,
    OneFrontRow,
    AllFrontRow,
    OneBackRow,
    AllBackRow
}

public enum SplashTypes {
    None,
    OneAdjacent,
    BothAdjacent,
    EntireRow,
    All
}

public enum Notes {
    Melee,
    Missile,
    Direct,
    Physical,
    Magical,
    Silenceable
}
