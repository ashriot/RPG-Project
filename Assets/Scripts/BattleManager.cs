using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {
    
    public static BattleManager instance;

    [Header("Object References")]
    public Canvas canvas;
    public GameObject battleScene;
    public GameObject enemyAttackEffect;
    public DamageNumber damageNumber;
    public BattleActionDisplay battleActionDisplay;
    public BattleTargetButton targetButton;
    public GameObject statsWindow;
    public GameObject battleMenu;
    public GameObject targetMenu;
    public GameObject magicMenu;
    public GameObject itemMenu;
    public GameObject characterSelectMenu;
    public BattleTooltip actionTooltip;
    public BattleNotification actionNote;
    public Text menuPlayerName;
    public BattleNotification battleNotification;
    public GameObject battleMenuDisabled;
    public Text keyCount;
    public string gameOverScene;

    private bool battleActive;
    private bool fleeing;
    private float waitTimer;
    private const float UI_WINDOW_DISTANCE = 22f;
    
    [Header("Battle Flow Data")]
    public int currentTurnId;
    public bool turnWaiting;
    public bool waitingForInput;
    public float turnDelay = .1f;
    public float chanceToFlee = .35f;
    public bool cannotFlee;

    [Header("Battle Rewards Data")]
    public int xpEarned;
    public string[] itemsReceived;

    [Header("Battle Data")]
    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public Image[] statWindows;
    public BattleCharacter[] playerPrefabs;
    public BattleCharacter[] enemyPrefabs;
    public BattleAction[] battleActions;

    public BattleTargetButton[] targetButtons;
    public BattleMagicSelect[] magicButtons;
    public ItemButton[] itemButtons;

    public Item selectedItem;
    public string selectedItemName;
    public Text itemName, itemDescription, useButtonText;
    public Text[] itemCharacterSelectNames;

    public List<BattleCharacter> activeBattleCharacters = new List<BattleCharacter>();

    public Image[] playerImages;
    public Text[] playerNames, playerHpValues, playerMpValues;
    public Slider[] playerHpSliders, playerMpSliders;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start() {
        // keyCount.text = PlayerController.instance.keyCount.ToString();
    }

    // Update is called once per frame
    void Update() {
        if (waitTimer > 0) {
            waitTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate() {
        if (turnWaiting) {
            UpdateBattle();
            while (activeBattleCharacters[currentTurnId].isDead) {
                    currentTurnId++;
                    if (currentTurnId >= activeBattleCharacters.Count) {
                        currentTurnId = 0;
                    }
                }
            if (!waitingForInput) {
                if (activeBattleCharacters.Count == 0) return;
                if (activeBattleCharacters[currentTurnId].isPlayer) {
                    // TODO: FIX THIS
                    menuPlayerName.text = activeBattleCharacters[currentTurnId].name;
                    if (activeBattleCharacters.All(x => x.isDead && !x.isPlayer)) return;
                    StartCoroutine(MoveWindow(statWindows[currentTurnId].gameObject, UI_WINDOW_DISTANCE));
                    
                    AudioManager.instance.PlaySfx("test");
                    battleMenuDisabled.SetActive(false);
                    battleMenu.SetActive(true);
                    waitingForInput = true;
                } else { // enemy turn
                    battleMenu.SetActive(false);
                    // enemy should attack
                    StartCoroutine(EnemyMoveCoroutine());
                }
            }
        }
    }

    public void BattleStart(string[] enemyNames, bool unableToFlee) {
        if (battleActive) return;

        cannotFlee = unableToFlee;
        battleActive = true;
        GameManager.instance.battleActive = true;
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
        battleScene.SetActive(true);
        statsWindow.SetActive(true);

        AudioManager.instance.PlaySfx("test");

        var activeHeroes = GameManager.instance.heroes.Where(h => h.isActive).ToArray();

        for (var i = 0; i < activeHeroes.Length; i++) {
            var newPlayer = Instantiate(activeHeroes[i], playerPositions[i].position, playerPositions[i].rotation);
            newPlayer.transform.parent = playerPositions[i];
                        
                        // activeBattleCharacters.Add(newPlayer);
                        // activeBattleCharacters[i].currentHp = activeHeroes[i].currentHp;
                        // activeBattleCharacters[i].maxHp = activeHeroes[i].maxHp;
                        // activeBattleCharacters[i].currentMp = activeHeroes[i].currentMp;
                        // activeBattleCharacters[i].maxMp = activeHeroes[i].maxMp;
                        // activeBattleCharacters[i].attack = activeHeroes[i].attack;
                        // activeBattleCharacters[i].defense = activeHeroes[i].defense;
                        // activeBattleCharacters[i].magic = activeHeroes[i].magic;
                        // activeBattleCharacters[i].speed = activeHeroes[i].speed;
        }

        // for (var i = 0; i < playerPositions.Length; i++) {
        //     if (activeHeroes[i].gameObject.activeInHierarchy) {
        //         for (var j = 0; j < playerPrefabs.Length; j++) {
        //             if (playerPrefabs[j].name == activeHeroes[i].name) {
        //                 var newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
        //                 newPlayer.transform.parent = playerPositions[i];
                        
        //                 activeBattleCharacters.Add(newPlayer);
        //                 activeBattleCharacters[i].currentHp = activeHeroes[i].currentHp;
        //                 activeBattleCharacters[i].maxHp = activeHeroes[i].maxHp;
        //                 activeBattleCharacters[i].currentMp = activeHeroes[i].currentMp;
        //                 activeBattleCharacters[i].maxMp = activeHeroes[i].maxMp;
        //                 activeBattleCharacters[i].attack = activeHeroes[i].attack;
        //                 activeBattleCharacters[i].defense = activeHeroes[i].defense;
        //                 activeBattleCharacters[i].magic = activeHeroes[i].magic;
        //                 activeBattleCharacters[i].speed = activeHeroes[i].speed;
        //             }
        //         }
        //     }
        // }

        for (var i = 0; i < enemyNames.Length; i++) {
            if (enemyNames[i] != string.Empty) {
                for (var j = 0; j < enemyPrefabs.Length; j++) {
                    if (enemyPrefabs[j].name == enemyNames[i]) {
                        var newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                        newEnemy.transform.parent = enemyPositions[i];
                        activeBattleCharacters.Add(newEnemy);
                    }
                }
            }
        }

        turnWaiting = true;
        currentTurnId = Random.Range(0, activeBattleCharacters.Count);
        UpdateUiStats();
    }

    public void NextTurn() {
        if (waitingForInput) {
            statWindows[currentTurnId].color = Color.white;
            waitingForInput = false;
        }
        
        currentTurnId++;
        if (currentTurnId >= activeBattleCharacters.Count) {
            currentTurnId = 0;
        }

        turnWaiting = true;
        
        UpdateBattle();
    }

    public void UpdateBattle() {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (var i = 0; i < activeBattleCharacters.Count; i++) {
            if (activeBattleCharacters[i].currentHp < 0) {
                activeBattleCharacters[i].currentHp = 0;
            }

            if (activeBattleCharacters[i].currentHp == 0 && !activeBattleCharacters[i].isDead) {
                activeBattleCharacters[i].isDead = true; 
                AudioManager.instance.PlaySfx("test");
                if (activeBattleCharacters[i].isPlayer) {
                    activeBattleCharacters[i].spriteRenderer.sprite = activeBattleCharacters[i].deadSprite;
                } else {
                    activeBattleCharacters[i].EnemyFade();
                }
            } else if (activeBattleCharacters[i].isPlayer) {
                allPlayersDead = false;
                activeBattleCharacters[i].spriteRenderer.sprite = activeBattleCharacters[i].aliveSprite;
            } else {    // enemy
                allEnemiesDead = false;
            }
        }
        
        if (activeBattleCharacters.All(abc => !abc.isPlayer && abc.isDead)) {
            allEnemiesDead = true;
        }

        if (allEnemiesDead || allPlayersDead) {
            if (allEnemiesDead) {
                // end battle in victory
                StartCoroutine(EndBattleCoroutine());
            } else {
                // end battle in failure
                StartCoroutine(GameOverCoroutine());
            }
        }
        // else {
        //     while (activeBattleCharacters[currentTurnId].isDead) {
        //         currentTurnId++;
        //         if (currentTurnId >= activeBattleCharacters.Count) {
        //             currentTurnId = 0;
        //         }
        //     }

        //     UpdateUiStats();
        // }
    }

    public IEnumerator<WaitForSeconds> EnemyMoveCoroutine() {
        turnWaiting = false;
        yield return new WaitForSeconds(turnDelay);
        EnemyDeclareAttack();
        yield return new WaitForSeconds(turnDelay*2);
        EnemyAttack();
        yield return new WaitForSeconds(turnDelay/2);
        NextTurn();
    }

    public void EnemyDeclareAttack() {
        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedActionId = Random.Range(0, currentTurn.battleActions.Length);
        var action = GetBattleAction(currentTurn.battleActions[selectedActionId]);

        var position = Camera.main.WorldToScreenPoint(currentTurn.transform.position + new Vector3(0, +1.5f, 0f));
        Instantiate(battleActionDisplay, position, currentTurn.transform.rotation, canvas.transform).SetText(action.actionName);
    }

    public void EnemyAttack() {
        List<int> players = new List<int>();

        for (var i = 0; i < activeBattleCharacters.Count; i++) {
            if (activeBattleCharacters[i].isPlayer && !activeBattleCharacters[i].isDead) {
                players.Add(i);
            }
        }

        var selectedTargetId = players[Random.Range(0, players.Count)];
        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedTarget = activeBattleCharacters[selectedTargetId];
        var selectedActionId = Random.Range(0, currentTurn.battleActions.Length);
        var action = GetBattleAction(currentTurn.battleActions[selectedActionId]);

        StartCoroutine(FlashSprite(currentTurn.spriteRenderer, Color.clear));
        Instantiate(action.effect, selectedTarget.transform.position, selectedTarget.transform.rotation);
        DealDamage(selectedTargetId, action);
        StartCoroutine(FlashSprite(selectedTarget.spriteRenderer, Color.red));
    }

    public IEnumerator<WaitForSeconds> FlashSprite(SpriteRenderer spriteRenderer, Color color) {
        var originalColor = spriteRenderer.color;
        for (int n = 0; n < 2; n++)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator<WaitForSeconds> FlashImage(Image image, Color color) {
        var originalColor = image.color;
        for (int n = 0; n < 2; n++)
        {
            image.color = color;
            yield return new WaitForSeconds(0.05f);
            image.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator<WaitForSeconds> MoveWindow(GameObject gameObject, float distance) {
        Vector3 destPos = gameObject.transform.localPosition + new Vector3(distance, 0f, 0f);

        while (Vector3.Distance(gameObject.transform.localPosition, destPos)>0.01f) {
            Debug.Log(destPos);
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, destPos, 150f * Time.deltaTime);
            yield return null;
        }
    }

    public void DealDamage(int selectedTargetId, BattleAction battleAction) {
        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedTarget = activeBattleCharacters[selectedTargetId];
        var attackPower = 0;
        var defensePower = 0;
        var damageToDeal = 0;

        switch (battleAction.actionType)
        {
            case BattleActionType.Physical:
                attackPower = currentTurn.attack;
                defensePower = selectedTarget.defense;
            break;

            case BattleActionType.Magical:
                attackPower = currentTurn.magic;
                defensePower = selectedTarget.magic;
            break;
            
            case BattleActionType.Healing:
                attackPower = currentTurn.magic;
                defensePower = 1;
            break;

            default:
            Debug.LogError("Reached an invalid Battle Action Type.");
            break;
        }

        var damageCalculation = ((float)attackPower / defensePower) * battleAction.power * Random.Range(.9f, 1.1f);
        damageToDeal = Mathf.RoundToInt(damageCalculation);

        selectedTarget.currentHp -= battleAction.actionType != BattleActionType.Healing ? damageToDeal : damageToDeal * -1;

        Debug.Log(currentTurn.name + " deals " + damageToDeal + "(" + damageCalculation + ") to "
            + selectedTarget.name + " [HP: " + selectedTarget.currentHp + "/" + selectedTarget.maxHp + "]");

        Instantiate(damageNumber, selectedTarget.transform.position, selectedTarget.transform.rotation).SetDamage(damageToDeal);

        UpdateUiStats();
    }

    public void UpdateUiStats() {
        for (var i = 0; i < playerNames.Length; i++) {
            if (activeBattleCharacters.Count <= i) {
                playerNames[i].gameObject.transform.parent.gameObject.SetActive(false);
                break;
            }
            if (activeBattleCharacters[i].isPlayer) {
                var playerData = activeBattleCharacters[i];

                playerNames[i].gameObject.SetActive(true);
                playerNames[i].text = playerData.name;
                playerHpValues[i].text = Mathf.Clamp(playerData.currentHp, 0, int.MaxValue).ToString();
                playerMpValues[i].text = Mathf.Clamp(playerData.currentMp, 0, int.MaxValue).ToString();
                playerHpSliders[i].value = (float)playerData.currentHp / playerData.maxHp;
                playerMpSliders[i].value = (float)playerData.currentMp / playerData.maxMp;
                playerImages[i].sprite = playerData.portrait;
            }
            else {
                playerNames[i].gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string actionName, int selectedTargetId) {
        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedTarget = activeBattleCharacters[selectedTargetId];
        var action = new BattleAction();

        var selectedActionId = Random.Range(0, currentTurn.battleActions.Length);


        for (var i = 0; i < battleActions.Length; i++) {
            if (battleActions[i].actionName == actionName) {
                Instantiate(battleActions[i].effect, selectedTarget.transform.position, selectedTarget.transform.rotation);
                action = battleActions[i];
            }
        }

        currentTurn.currentMp -= action.mpCost;

        var position = Camera.main.WorldToScreenPoint(currentTurn.transform.position + new Vector3(0, +1.5f, 0f));
        // Instantiate(battleActionDisplay, position, currentTurn.transform.rotation, canvas.transform).SetText(action.actionName);

        StartCoroutine(FlashSprite(currentTurn.spriteRenderer, Color.blue));
        DealDamage(selectedTargetId, action);

        if (currentTurn.isPlayer) {
            StartCoroutine(MoveWindow(statWindows[currentTurnId].gameObject, -UI_WINDOW_DISTANCE));
        }

        actionTooltip.gameObject.SetActive(false);
        battleMenu.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        NextTurn();
    }

    public void OpenTargetMenu(string actionName) {
        var action = GetBattleAction(actionName);

        BattleManager.instance.actionTooltip.gameObject.SetActive(true);
        BattleManager.instance.actionTooltip.description.text = action.description;
        
        targetMenu.SetActive(true);

        var enemyIds = new List<int>();

        for (var i = 0; i < activeBattleCharacters.Count; i++) {
            if (!activeBattleCharacters[i].isPlayer) {
                enemyIds.Add(i);
            }
        }

        for (var i = 0; i < targetButtons.Length; i++) {
            if (enemyIds.Count > i && activeBattleCharacters[enemyIds[i]].currentHp > 0) {
                var enemy = activeBattleCharacters[enemyIds[i]];

                targetButtons[i].transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position + new Vector3(0, -1.2f, 0f));
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].actionName = action.actionName;
                targetButtons[i].targetId = enemyIds[i];
                targetButtons[i].targetName.text = enemy.name;

                var hpPercent = (float)enemy.currentHp / enemy.maxHp;

                var color = new Color(.51f, .86f, .2f);

                if (hpPercent < .2f) {
                    color = new Color(.86f, .32f, .2f);
                } else if (hpPercent < .4f) {
                    color = new Color(.86f, .53f, .2f);
                } else if (hpPercent < .6f) {
                    color = new Color(.86f, .7f, .2f);
                } else if (hpPercent < .8f) {
                    color = new Color(.8f, .86f, .2f);
                }

                targetButtons[i].targetName.color = color;
            } else {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMagicMenu() {
        actionTooltip.gameObject.SetActive(false);
        targetMenu.SetActive(false);

        magicMenu.SetActive(true);
        battleMenuDisabled.SetActive(true);

        var magicNames = activeBattleCharacters[currentTurnId].battleActions;

        for (var i = 0; i < magicButtons.Length; i++) {
            if (i >= magicNames.Length) {
                magicButtons[i].gameObject.SetActive(false);
                continue;
            }

            var magicInfo = battleActions.Where(ba => ba.actionName == magicNames[i]).Single();

            var newButton = magicButtons[i];
            newButton.gameObject.SetActive(true);
            newButton.magicName = magicInfo.actionName;
            newButton.description = magicInfo.description;
            newButton.mpCost = magicInfo.mpCost;
            newButton.nameText.text = newButton.magicName;
            newButton.costText.text = newButton.mpCost.ToString();
        }

        // for (var i = 0; i < magicButtons.Length; i++) {
        //     if (activeBattleCharacters[currentTurnId].battleActions.Length > i) {
        //         magicButtons[i].gameObject.SetActive(true);
        //         magicButtons[i].magicName = activeBattleCharacters[currentTurnId].battleActions[i];
        //         magicButtons[i].nameText.text = magicButtons[i].magicName;

        //         for (var j = 0; j < battleActions.Length; j++) {
        //             if (battleActions[j].actionName == magicButtons[i].magicName) {
        //                 magicButtons[i].mpCost = battleActions[j].mpCost;
        //                 magicButtons[i].costText.text = magicButtons[i].mpCost.ToString();
        //                 break;
        //             }
        //         }
        //     } else {
        //         magicButtons[i].gameObject.SetActive(false);
        //     }
        // }
    }

    public void Flee() {
        if (cannotFlee) {
            battleNotification.text.text = "Cannot flee!";
            battleNotification.Activate();
        } else {
            var fleeRoll = Random.Range(0, 1f);
            if (fleeRoll < chanceToFlee) {
                fleeing = true;
                // end the battle
                StartCoroutine(EndBattleCoroutine());
            } else {
                NextTurn();
                battleNotification.text.text = "Couldn't flee!";
                battleNotification.Activate();
            }
        }
    }
    
    public IEnumerator<WaitForSeconds> EndBattleCoroutine() {
        battleActive = false;
        battleNotification.text.text = "You win!";
        battleMenu.SetActive(false);
        statsWindow.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemMenu.SetActive(false);
        battleNotification.Activate();

        yield return new WaitForSeconds(1.5f);

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        for (var i = 0; i < activeBattleCharacters.Count; i++) {
            if (activeBattleCharacters[i].isPlayer) {
                for (var j = 0; j < GameManager.instance.heroes.Length; j++) {
                    if (activeBattleCharacters[i].name == GameManager.instance.heroes[j].name) {
                        GameManager.instance.heroes[j].hp.current = activeBattleCharacters[i].currentHp;
                        GameManager.instance.heroes[j].mp.current = activeBattleCharacters[i].currentMp;
                        break;
                    }
                }
            }

            Destroy(activeBattleCharacters[i].gameObject);
        }

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattleCharacters.Clear();
        currentTurnId = 0;
        if (fleeing) {
            fleeing = false;
            GameManager.instance.battleActive = false;
        } else {
            BattleReward.instance.OpenWindow(xpEarned, itemsReceived);
        }

        // AudioManager.instance.PlayBgm(FindObjectOfType<CameraController>().musicIdToPlay);
    }

    public IEnumerator<WaitForSeconds> GameOverCoroutine() {
        battleNotification.text.text = "Game Over!";
        battleNotification.Activate();
        battleActive = false;
        statsWindow.SetActive(false);
        yield return new WaitForSeconds(2f);
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(.5f);
        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }

    public void Back() {
        actionTooltip.gameObject.SetActive(false);
        magicMenu.SetActive(false);
        targetMenu.SetActive(false);
        battleMenuDisabled.SetActive(false);
    }

    public BattleAction GetBattleAction (string name) {
        for (var i = 0; i < battleActions.Length; i++) {
            if (battleActions[i].actionName == name) {
                return battleActions[i];
            }
        }
        Debug.LogError("Couldn't find battle action '" + name + "'.");
        return null;
    }
}

[CustomEditor(typeof(BattleManager))]
public class BattleManagerEditor : Editor {
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        BattleManager manager = (BattleManager)target;

        if(GUILayout.Button("Start a Battle against 1")) {
            manager.BattleStart(new string[] { "Eyeball" }, false);
        }
        if(GUILayout.Button("Start a Battle against 2")) {
            manager.BattleStart(new string[] { "Eyeball", "Spider" }, false);
        }
        if(GUILayout.Button("Start a Battle against 3")) {
            manager.BattleStart(new string[] { "Eyeball", "Spider", "Skeleton" }, false);
        }
        if(GUILayout.Button("Start a Battle against 6")) {
            manager.BattleStart(new string[] { "Eyeball", "Spider", "Skeleton", "Eyeball", "Eyeball", "Spider" }, false);
        }
        if(GUILayout.Button("Set all players' HP to 1")) {
            foreach(var player in GameManager.instance.heroes) {
                player.hp.current = 1;
            }
            foreach(var player in BattleManager.instance.activeBattleCharacters) {
                if (player.isPlayer) {
                    player.currentHp = 1;
                }
            }
        }
        if(GUILayout.Button("God Mode!!")) {
            foreach(var player in GameManager.instance.heroes) {
                // player.currentHp = 999;
                // player.maxHp = 999;
                // player.currentMp = 999;
                // player.maxMp = 999;
                // player.attack = 5000;
                // player.defense = 500;
            }
            foreach(var player in BattleManager.instance.activeBattleCharacters) {
                if (player.isPlayer) {
                    player.currentHp = 999;
                    player.maxHp = 999;
                    player.currentMp = 999;
                    player.maxMp = 999;
                    player.attack = 5000;
                    player.defense = 500;
                }
            }
            manager.UpdateBattle();
        }
        if (GUILayout.Button("Next turn.")) {
            manager.NextTurn();
        }
    }
}

