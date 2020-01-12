using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionRepo : MonoBehaviour {

    public static BattleActionRepo instance;
    
    public List<BattleAction> referenceBattleActions;
    public List<StatusEffect> referenceStatusEffects;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public BattleAction GetActionByName(string name) {
        var action = referenceBattleActions.Find(a => a.name == name);        
        if (action != null) {
            return action;
        }

        Debug.LogError("Couldn't find battle action '" + name +"'!");
        return null;
    }

    public StatusEffect GetStatusEffectByName(string name) {
        var effect = referenceStatusEffects.Find(a => a.name == name);        
        if (effect != null) {
            return effect;
        }

        Debug.LogError("Couldn't find status effect '" + name +"'!");
        return null;
    }

    public void ApplyStatusEffectToSelf(string name, int duration) {
        var currentTurnId = BattleManager.instance.currentTurnId;
        var statusEffect = GetStatusEffectByName(name);
        statusEffect.duration = duration;
        BattleManager.instance.combatants[currentTurnId].statusEffects.Add(statusEffect);
    }
}
