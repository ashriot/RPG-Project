
public abstract class Hands : EquippableItem
{
    public abstract int GetAtkOrBlkValue();
    public abstract string GetAtkOrBlkString(int bonus = 0);
    public abstract int GetDmgOrAmtValue();
    public abstract string GetDmgOrAmtString();
}
