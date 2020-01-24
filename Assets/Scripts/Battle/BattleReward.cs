using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour {

    public static BattleReward instance;

    public Text[] xpTexts;
    public int[] xpRewards;
    public Color originalColor;

    public bool markQuestComplete;
    public string questNameToComplete;

    private Vector3 originalPos;
    private bool moving;

    void Awake() {
        instance = this;
        originalPos = transform.position;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.R)) {
            OpenWindow(100);
        }
    }

    public void OpenWindow(int xpEarned) {
        if (moving) {
            return;
        }
        moving = true;
        foreach(var text in xpTexts) {
            text.color = originalColor;
        }

        Debug.Log("Start REWARDS");
        for (var i = 0; i < xpRewards.Length; i++) {
            xpRewards[i] = xpEarned;
        }

        foreach (var xpText in xpTexts) {
            xpText.text = "+" + xpEarned + " XP";
        }

        for (var i = 0; i < GameManager.instance.heroes.Length; i++) {
            if (GameManager.instance.heroes[i].gameObject.activeInHierarchy) {
                GameManager.instance.heroes[i].AddXp(xpRewards[i]);
            }
        }

        PlayerController.instance.inBattle = false;
        GameManager.instance.battleActive = false;

        GameMenu.instance.UpdateStats();
        StartCoroutine(CloseWindow(.5f));

        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questNameToComplete);
        }
    }

    public IEnumerator CloseWindow(float duration) {
        var endPos = transform.localPosition + new Vector3(55f, 0f, 0f);
        float counter = 0;
        while (counter < duration) {
            counter += Time.deltaTime;
            Vector3 currentPos = transform.localPosition;
            float time = Vector3.Distance(currentPos, endPos) / (duration - counter) * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(currentPos, endPos, time);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(FadeOut(2f));
    }

    public IEnumerator FadeOut(float _duration) {
        var waitTime = _duration/2;
        while (waitTime > 0) {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        float elapsedTime = 0.0f;
        while (elapsedTime < _duration) {
            foreach(var text in xpTexts) {
                text.color = Color.Lerp(text.color, Color.clear, (elapsedTime / _duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        transform.position = originalPos;
        moving = false;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(BattleReward))]
public class BattleRewardEditor : Editor {
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        var manager = (BattleReward)target;

        if(GUILayout.Button("Generate rewards.")) {
            manager.OpenWindow(75);
        }
    }
}

#endif