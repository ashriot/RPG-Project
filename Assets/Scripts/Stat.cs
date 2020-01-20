using UnityEngine;

[System.Serializable]
public class Stat {
    
    public int baseValue;
    public int bonus;
    public int value { get => baseValue + bonus; }

    public void IncreaseBaseValue(int amount) {
        baseValue += amount;
    }

    public void IncreaseBonus(int amount) {
        bonus += amount;
    }

    public void DecreaseBonus(int amount) {
        bonus -= amount;

        if (bonus < 0) {
            Debug.LogError("Bonus went negative!");
        }
    }
}
