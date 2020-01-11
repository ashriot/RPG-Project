using UnityEngine;

[System.Serializable]
public class Resource {
    
    public int current;
    public int maximum;
    public int totalBonus;

    public int max { get => maximum + totalBonus; }

    public string display { get => current + "/" + max; }
    public float percent { get => max == 0 ? 1 : (float)current / max; }

    public void Increase(int amount) {
        current = Mathf.Clamp(current + amount, 0, max);
    }

    public void Decerease(int amount) {
        current = Mathf.Clamp(current - amount, 0, max);
    }

    public void SetToMax() {
        current = max;
    }
}
