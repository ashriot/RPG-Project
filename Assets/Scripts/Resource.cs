using UnityEngine;

[System.Serializable]
public class Resource {
    
    public int current;
    public int baseMax;
    public int totalBonus;

    public int totalMax { get => baseMax + totalBonus; }
    public int missing { get => totalMax - current; }

    public string display { get => current + "/" + totalMax; }
    public float percent { get => totalMax == 0 ? 1 : (float)current / totalMax; }

    public void Increase(int amount) {
        current = Mathf.Clamp(current + amount, 0, totalMax);
    }

    public void Decerease(int amount) {
        current = Mathf.Clamp(current - amount, 0, totalMax);
    }

    public void SetToMax() {
        current = totalMax;
    }
}
