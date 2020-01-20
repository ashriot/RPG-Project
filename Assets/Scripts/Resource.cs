using UnityEngine;

[System.Serializable]
public class Resource {
    
    public int current;
    public int baseMax;
    public int bonus;

    public int maximum { get => baseMax + bonus; }
    public int missing { get => maximum - current; }

    public string display { get => current + "/" + maximum; }
    public float percent { get => maximum == 0 ? 1 : (float)current / maximum; }

    public void Increase(int amount) {
        current = Mathf.Clamp(current + amount, 0, maximum);
    }

    public void Decerease(int amount) {
        current = Mathf.Clamp(current - amount, 0, maximum);
    }

    public void SetToMax() {
        current = maximum;
    }
}
