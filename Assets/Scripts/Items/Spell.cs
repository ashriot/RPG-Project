using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Spell", menuName = "Spell")]
public class Spell : Item {

    public int minimumDamage;
    public int maximumDamage;
    public float delay;

    public override void Use(int targetId) {

    }

    public override void Discard() {
        
    }
}
