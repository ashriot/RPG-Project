using UnityEngine;

[System.Serializable]
public class Stat {
    
    public int baseValue;
    public int totalBonus;

    public int GetTotalValue() {
        return baseValue + totalBonus;
    }

    public void IncreaseBaseValue(int amount) {
        baseValue += amount;
    }

    public void IncreaseBonus(int amount) {
        totalBonus += amount;
    }

    public void DecreaseBonus(int amount) {
        totalBonus -= amount;

        if (totalBonus < 0) {
            Debug.LogError("Bonus went negative!");
        }
    }
}
