using UnityEngine;

[System.Serializable]
public class Resource {
    
    public int current;
    public int max;

    public string display { get => current + "/" + max; }
    public float percent { get => max == 0 ? 1 : current / max; }

    public Resource(int _current, int _max) {
        current = _current;
        max = _max;
    }

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
