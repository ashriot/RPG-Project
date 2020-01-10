public class Hero : Unit {

    public Resource constitution;
    public bool isKOed { get { return (hp.current == 0 && constitution.current > 0); } }
    public new bool isDead { get { return (hp.current == 0 && constitution.current == 0); } }

    public void AddXp(int amount) {
        xp += amount;

        // level up code
    }
}
