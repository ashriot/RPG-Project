using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterStatsType : ScriptableObject
{
    public new string name;
    public int level, currentXp, currentHp, maxHp, currentMp, maxMp, attack, defense, magic, speed;
    public float timeTillTurn;
    public bool isPlayer;
    public bool isActive;
    public bool isDead;
    public string[] battleActions;
}
