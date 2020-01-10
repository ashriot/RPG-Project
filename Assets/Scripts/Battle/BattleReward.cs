using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour {

    public static BattleReward instance;

    public Text[] xpTexts;
    public int[] xpRewards;

    public bool markQuestComplete;
    public string questNameToComplete;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenWindow(int xpEarned) {
        PlayerController.instance.inBattle = false;

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

        GameManager.instance.battleActive = false;
        StartCoroutine(CloseWindow(2f));
    
        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questNameToComplete);
        }
    }

    public IEnumerator CloseWindow(float _duration) {
        StartCoroutine(SlideIntoPosition());

        float elapsedTime = 0.0f;
        while (elapsedTime < _duration) {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Fade out!");

        StartCoroutine(FadeOut(2f));
    }

    public IEnumerator SlideIntoPosition() {
        var moveTime = .0005f;
        var endPos = transform.position + new Vector3(366f, 0f, 0f);
        float sqrRemainingDistance = (transform.position - endPos).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;
        
        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, endPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - endPos).sqrMagnitude;

            yield return null;
        }
    }

    public IEnumerator FadeOut(float _duration) {
        float elapsedTime = 0.0f;
        while (elapsedTime < _duration) {
            foreach(var text in xpTexts) {
            text.color = Color.Lerp(text.color, Color.clear, (elapsedTime / _duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            }
        }
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