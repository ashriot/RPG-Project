using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour {

    public string characterName;
    public int level = 1;
    public int maxLevel = 100;
    public int currentXp;
    public int[] xpToNextLevel;
    public int baseXp = 1000;

    public int currentHp;
    public int maxHp = 100;
    public int currentMp;
    public int maxMp = 10;
    public int[] mpLevelBonus;
    public int attack;
    public int defense;
    public int magic;
    public int speed;
    public Image image;

    // Start is called before the first frame update
    void Start() {
        xpToNextLevel = new int[maxLevel];
        xpToNextLevel[1] = baseXp;

        for (var i = 2; i < xpToNextLevel.Length; i++) {
            xpToNextLevel[i] = Mathf.FloorToInt(xpToNextLevel[i-1] * 1.05f);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void AddXp(int xpToAdd) {
        currentXp += xpToAdd;

        if (currentXp > xpToNextLevel[level]) {
            currentXp -= xpToNextLevel[level];
            level++;

            // stat gains

        }
    }
}
