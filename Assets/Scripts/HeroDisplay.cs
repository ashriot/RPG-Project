using UnityEngine;
using UnityEngine.UI;

public class HeroDisplay : MonoBehaviour
{
    public Text sp, hp, mp, attack, defense, magic, speed, armor, resist;
    public Text[] statPreviews = new Text[(int)Stats.Count];
}
