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

    void Awake() {
        instance = this;
    }

    public void OpenWindow(int xpEarned) {
        originalPos = transform.position;
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
        StartCoroutine(CloseWindow(2f));
    
        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questNameToComplete);
        }
    }

    public IEnumerator CloseWindow(float _duration) {
        GameMenu.instance.UpdateStats();
        StartCoroutine(MoveTowards(transform, transform.localPosition + new Vector3(55f, 0f, 0f), .25f));

        float elapsedTime = 0.0f;
        while (elapsedTime < _duration) {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }


        StartCoroutine(FadeOut(1.5f));
    }

    IEnumerator MoveTowards(Transform objectToMove, Vector3 toPosition, float duration) {
        float counter = 0;
        while (counter < duration) {
            counter += Time.deltaTime;
            Vector3 currentPos = objectToMove.localPosition;
            float time = Vector3.Distance(currentPos, toPosition) / (duration - counter) * Time.deltaTime;
            objectToMove.localPosition = Vector3.MoveTowards(currentPos, toPosition, time);
            yield return null;
        }
}

    public IEnumerator FadeOut(float _duration) {
        Debug.Log(gameObject.transform.localPosition);
        float elapsedTime = 0.0f;
        while (elapsedTime < _duration) {
            foreach(var text in xpTexts) {
                text.color = Color.Lerp(text.color, Color.clear, (elapsedTime / _duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        transform.position = originalPos;
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