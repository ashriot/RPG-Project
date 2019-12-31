using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour {

    public float timeBetweenBattles = 10f;
    public bool activateOnEnter, activateOnStay, activateOnExit;
    public bool deactivateAfterBattle;
    public bool cannotFlee;
    public bool shouldCompleteQuest;
    public string questNameToComplete;

    public BattleType[] potentialBattles;

    private bool inZone;
    private float betweenBattlesCounter;

    // Start is called before the first frame update
    void Start() {
        betweenBattlesCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);
    }

    // Update is called once per frame
    void Update() {
        if (inZone && PlayerController.instance.canMove) {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                betweenBattlesCounter -= Time.deltaTime;
            }

            if (betweenBattlesCounter <= 0) {
                betweenBattlesCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);
                StartCoroutine(StartBattleCoroutine());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (activateOnEnter) {
                StartCoroutine(StartBattleCoroutine());
            } else {
                inZone = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            if (activateOnExit) {
                StartCoroutine(StartBattleCoroutine());
            } else {
                inZone = false;
            }
        }
    }

    public IEnumerator<WaitForSeconds> StartBattleCoroutine() {
        UIFade.instance.FadeToBlack();
        GameManager.instance.battleActive = true;

        var selectedBattleId = Random.Range(0, potentialBattles.Length);

        BattleManager.instance.itemsReceived = potentialBattles[selectedBattleId].itemRewards;
        BattleManager.instance.xpEarned = potentialBattles[selectedBattleId].xpReward;

        yield return new WaitForSeconds(1.5f);

        BattleManager.instance.BattleStart(potentialBattles[selectedBattleId].enemies, cannotFlee);
        UIFade.instance.FadeFromBlack();

        if (deactivateAfterBattle) {
            gameObject.SetActive(false);
        }

        BattleReward.instance.markQuestComplete = shouldCompleteQuest;
        BattleReward.instance.questNameToComplete = questNameToComplete;
    }
}
