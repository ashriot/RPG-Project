using UnityEngine;

public class Hero : Unit {

    public float tpChanceBase;
    public float tpChance;
    
	[LabeledArray(new string[] {"Head", "Body", "Arms", "Feet", "Ring L", "Ring R"})]
    public EquippableItem[] bodyEquipment = new EquippableItem[6];
	[LabeledArray(new string[] {"Main Hand", "Off-hand"})]
    public Hands[] handEquipment = new Hands[2];

    // attribute masteries
    public int[] attributeSkillValues;
    public int[] combatSkillValues;
    public int[] magicSkillValues;
    public int[] fieldSkillValues;

    public int xp, sp, totalSp;

    public int xpRequiredForNextSkillPoint() { // INT(5*(E2+1)+((POWER(E2,3)/100)))
        var value = 5 * (totalSp + 1) + ((Mathf.Pow(totalSp, 3) / 100));
        return (int)value;
    }

    public void AddXp(int amount) {
        xp += amount;
        CheckForNewSkillPoint();
    }

    public void CheckForNewSkillPoint() {
        var req = xpRequiredForNextSkillPoint();
        while (xp >= req) {
            xp -= req;
            sp++;
            totalSp++;
            req = xpRequiredForNextSkillPoint();
        }
    }

    public EquippableItem GetBodyEquipment(EquipmentSlots slot) {
        var slotId = (int)slot;
        return bodyEquipment[slotId];
    }
    
    public EquippableItem GetHandEquipment(EquipmentSlots slot) {
        var slotId = (int)slot - 6;
        return bodyEquipment[slotId];
    }
}
