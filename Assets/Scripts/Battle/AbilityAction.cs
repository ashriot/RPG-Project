using UnityEngine;

[System.Serializable]
public class AbilityAction : ScriptableObject {
    public new string name;
    public float potencyModifier = 1f;
    public int minimumPotency;
    public int maximumPotency;
    public bool usesWeapons;

    public void Execute() {
        // ability execution code here
    }
}