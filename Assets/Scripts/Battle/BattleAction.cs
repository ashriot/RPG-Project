using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Action", menuName = "Battle Action")]
public class BattleAction : ScriptableObject {

    public new string name;
    public Sprite sprite;
    public string description;
    public BattleActionType actionType;
    public int attackPower;
    public int minDamage;
    public int maxDamage;
    public int mpCost;
    public bool isCharge;
    public float delay;
    public AttackEffect visualFx;
    public List<StatusEffect> statusEffects;
}
