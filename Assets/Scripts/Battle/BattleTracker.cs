using UnityEngine;

[System.Serializable]
public class BattleTracker : ScriptableObject {
    public bool isPlayer;

    public SpriteRenderer spriteRenderer;

    public int ticks, deflection, barrier;
    public bool isTaunting, isHiding, isCharging;

    public string chargedActionName;

    public int chargedActionTargetId;
}
