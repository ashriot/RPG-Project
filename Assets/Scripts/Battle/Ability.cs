using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
public class Ability : ScriptableObject {
    public AbilityTypes abilityType;
    public new string name;
    public string description;
    public Sprite sprite;
    public int mpCost;
    public int tpCost;
    public ChargeTypes chargeType;
    public float delay = 1f;
    public List<AbilityAction> actions = new List<AbilityAction>();
    public List<Notes> notes = new List<Notes>();
}

public enum AbilityTypes {
    Melee,
    Ranged,
    AoE,
    SingleMagic,
    ProjectileMagic,
    Debuff,
    Buff,
    Heal,
    Count
}

public enum StatTypes {  // the source stat for determining accuracy
    Hp,
    Mp,
    Tp,
    Con,
    Attack,
    Defense,
    Magic,
    Speed,
    Armor,
    Resist,
    CritPow,
    Deflect,
    Barrier,
    Count // max number of statTypes
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
