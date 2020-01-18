using UnityEngine;

public class Hero : Unit {

    public EquippableItem head, body, arms, feet, ringL, ringR;
    public Hands mainHand, offHand;

    // attribute masteries
    public int[] attributeMasteriesValues;

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
}
