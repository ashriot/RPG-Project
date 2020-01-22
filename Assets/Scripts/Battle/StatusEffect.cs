
[System.Serializable]
public class StatusEffect {
    public string name { get { return baseStatusEffect.name; } }
    public BaseStatusEffect baseStatusEffect;
    public int stacks;
}
