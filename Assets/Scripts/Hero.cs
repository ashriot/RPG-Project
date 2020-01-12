using UnityEngine;

public class Hero : Unit {

    public Resource constitution;

    // attribute skills
    public int[] attributeSkillValues;

    public int sp, totalSp;

    public bool isKOed { get { return (hp.current == 0 && constitution.current > 0); } }
    public new bool isDead { get { return (hp.current == 0 && constitution.current == 0); } }

    public HeroType initialValues;

    public int xpRequired() { // INT(5*(E2+1)+((POWER(E2,3)/100)))
        var value = 5 * (totalSp + 1) + ((Mathf.Pow(totalSp, 3) / 100));
        return (int)value;
    }

    public void AddXp(int amount) {
        xp += amount;
        CheckForNewSp();
    }

    public void CheckForNewSp() {
        var req = xpRequired();
        while (xp >= req) {
            xp -= req;
            sp++;
            totalSp++;
            req = xpRequired();
        }
    }
}
