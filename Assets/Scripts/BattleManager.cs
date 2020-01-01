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
    public BattleTargetButton targetButton;
    public GameObject statsWindow;
    public GameObject battleMenu;
    public GameObject targetMenu;
    public GameObject magicMenu;
    public GameObject itemMenu;
    public GameObject characterSelectMenu;
    public BattleNotification battleNotification;
    public string gameOverScene;

    private bool battleActive;
    private bool fleeing;
    
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

    // Start is called before the first frame update
    void Start() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
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
                    if (activeBattleCharacters.All(x => x.isDead && !x.isPlayer)) return;
                    statWindows[currentTurnId].color = Color.yellow;
                    StartCoroutine(FlashImage(statWindows[currentTurnId], Color.yellow));
                    
                    AudioManager.instance.PlaySfx(5);
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

        AudioManager.instance.PlayBgm(0);

        var activeHeroes = GameManager.instance.heroes.Where(h => h.isActive).ToArray();

        for (var i = 0; i < activeHeroes.Length; i++) {
            var newPlayer = Instantiate(activeHeroes[i], playerPositions[i].position, playerPositions[i].rotation);
            newPlayer.transform.parent = playerPositions[i];
                        
                        activeBattleCharacters.Add(newPlayer);
                        activeBattleCharacters[i].currentHp = activeHeroes[i].currentHp;
                        activeBattleCharacters[i].maxHp = activeHeroes[i].maxHp;
                        activeBattleCharacters[i].currentMp = activeHeroes[i].currentMp;
                        activeBattleCharacters[i].maxMp = activeHeroes[i].maxMp;
                        activeBattleCharacters[i].attack = activeHeroes[i].attack;
                        activeBattleCharacters[i].defense = activeHeroes[i].defense;
                        activeBattleCharacters[i].magic = activeHeroes[i].magic;
                        activeBattleCharacters[i].speed = activeHeroes[i].speed;
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
                AudioManager.instance.PlaySfx(0);
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
        EnemyAttack();
        yield return new WaitForSeconds(turnDelay/2);
        NextTurn();
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
        var actionPower = 0;

        var selectedActionId = Random.Range(0, currentTurn.battleActions.Length);
        for (var i = 0; i < battleActions.Length; i++) {
            if (battleActions[i].actionName == currentTurn.battleActions[selectedActionId]) {
                Instantiate(battleActions[i].effect, selectedTarget.transform.position, selectedTarget.transform.rotation);
                actionPower = battleActions[i].power;
            }
        }

        StartCoroutine(FlashSprite(currentTurn.spriteRenderer, Color.clear));
        //Instantiate(enemyAttackEffect, currentTurn.transform.position, currentTurn.transform.rotation);

        DealDamage(selectedTargetId, actionPower);
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

    public void DealDamage(int selectedTargetId, int battlePower, bool isFriendly = false) {
        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedTarget = activeBattleCharacters[selectedTargetId];
        var damageToDeal = 0;

        if (isFriendly) {
            damageToDeal = Mathf.RoundToInt(-battlePower);
        } else {

            var attackPower = currentTurn.attack;
            var defensePower = selectedTarget.defense;

            var damageCalculation = ((float)attackPower / defensePower) * battlePower * Random.Range(.9f, 1.1f);
            damageToDeal = Mathf.RoundToInt(damageCalculation);

            selectedTarget.currentHp -= damageToDeal;

            Debug.Log(currentTurn.name + " deals " + damageToDeal + "(" + damageCalculation + ") to "
                + selectedTarget.name + " [HP: " + selectedTarget.currentHp + "/" + selectedTarget.maxHp + "]");
        }

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
                playerImages[i].sprite = playerData.aliveSprite;
            }
            else {
                playerNames[i].gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string actionName, int selectedTargetId) {
        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedTarget = activeBattleCharacters[selectedTargetId];
        var actionPower = 0;

        var selectedActionId = Random.Range(0, currentTurn.battleActions.Length);
        for (var i = 0; i < battleActions.Length; i++) {
            if (battleActions[i].actionName == actionName) {
                Instantiate(battleActions[i].effect, selectedTarget.transform.position, selectedTarget.transform.rotation);
                actionPower = battleActions[i].power;
            }
        }

        StartCoroutine(FlashSprite(currentTurn.spriteRenderer, Color.blue));
        DealDamage(selectedTargetId, actionPower);

        battleMenu.SetActive(false);
        targetMenu.SetActive(false);
        NextTurn();
    }

    public void OpenTargetMenu(string actionName) {
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

                targetButtons[i].actionName = actionName;
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
        magicMenu.SetActive(true);

        for (var i = 0; i < magicButtons.Length; i++) {
            if (activeBattleCharacters[currentTurnId].battleActions.Length > i) {
                magicButtons[i].gameObject.SetActive(true);
                magicButtons[i].magicName = activeBattleCharacters[currentTurnId].battleActions[i];
                magicButtons[i].nameText.text = magicButtons[i].magicName;

                for (var j = 0; j < battleActions.Length; j++) {
                    if (battleActions[j].actionName == magicButtons[i].magicName) {
                        magicButtons[i].mpCost = battleActions[j].mpCost;
                        magicButtons[i].costText.text = magicButtons[i].mpCost.ToString();
                        break;
                    }
                }
            } else {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenItemMenu() {
        itemMenu.SetActive(true);
        selectedItem = GameManager.instance.GetItemDetails(GameManager.instance.inventory[0]);

        GameManager.instance.SortItems();
        for (var i = 0; i < itemButtons.Length; i++ ) {
            itemButtons[i].buttonId = i;

            if (GameManager.instance.inventory[i] != "") {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.inventory[i]).itemSprite;
                itemButtons[i].quantityText.text = GameManager.instance.inventoryQuantity[i].ToString();
            } else {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].quantityText.text = string.Empty;
            }
        }
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
    
    public void SelectItem(Item item) {
        selectedItem = item;

        if (selectedItem.itemType == ItemType.item) {
            useButtonText.text = "Use";
        } else if (selectedItem.itemType == ItemType.armor || selectedItem.itemType == ItemType.weapon ) {
            useButtonText.text = "Equip";
        }

        itemName.text = selectedItem.itemName;
        itemDescription.text = selectedItem.description;
    }

    public void OpenItemCharacterSelect() {
        characterSelectMenu.SetActive(true);
        for (var i = 0; i < itemCharacterSelectNames.Length; i++) {
            itemCharacterSelectNames[i].text = GameManager.instance.playerStats[i].characterName;
            itemCharacterSelectNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void UseItem(int selectedCharacterId) {
        selectedItem.Use(selectedCharacterId);

        var currentTurn = activeBattleCharacters[currentTurnId];
        var selectedTarget = activeBattleCharacters[selectedCharacterId];

        Instantiate(enemyAttackEffect, currentTurn.transform.position, currentTurn.transform.rotation);
        DealDamage(selectedCharacterId, selectedItem.potencyValue, true);
        
        var playerStats = GameManager.instance.playerStats;
        for (var i = 0; i < playerPositions.Length; i++) {
            if (playerStats[i].gameObject.activeInHierarchy) {
                for (var j = 0; j < playerPrefabs.Length; j++) {
                    if (playerPrefabs[j].name == playerStats[i].characterName) {
                        var newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                        newPlayer.transform.parent = playerPositions[i];
                        
                        activeBattleCharacters.Add(newPlayer);
                        activeBattleCharacters[i].currentHp = playerStats[i].currentHp;
                        activeBattleCharacters[i].maxHp = playerStats[i].maxHp;
                        activeBattleCharacters[i].currentMp = playerStats[i].currentMp;
                        activeBattleCharacters[i].maxMp = playerStats[i].maxMp;
                        activeBattleCharacters[i].attack = playerStats[i].attack;
                        activeBattleCharacters[i].defense = playerStats[i].defense;
                        activeBattleCharacters[i].magic = playerStats[i].magic;
                        activeBattleCharacters[i].speed = playerStats[i].speed;
                    }
                }
            }
        }

        UpdateBattle();
        itemMenu.SetActive(false);
        characterSelectMenu.SetActive(false);
        NextTurn();
    }

    public void ItemBack() {
        itemMenu.SetActive(false);
        characterSelectMenu.SetActive(false);
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
                for (var j = 0; j < GameManager.instance.playerStats.Length; j++) {
                    if (activeBattleCharacters[i].name == GameManager.instance.playerStats[j].characterName) {
                        GameManager.instance.playerStats[j].currentHp = activeBattleCharacters[i].currentHp;
                        GameManager.instance.playerStats[j].currentMp = activeBattleCharacters[i].currentMp;
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

        AudioManager.instance.PlayBgm(FindObjectOfType<CameraController>().musicIdToPlay);
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
            foreach(var player in GameManager.instance.playerStats) {
                player.currentHp = 1;
            }
            foreach(var player in BattleManager.instance.activeBattleCharacters) {
                if (player.isPlayer) {
                    player.currentHp = 1;
                }
            }
        }
        if(GUILayout.Button("God Mode!!")) {
            foreach(var player in GameManager.instance.playerStats) {
                player.currentHp = 999;
                player.maxHp = 999;
                player.currentMp = 999;
                player.maxMp = 999;
                player.attack = 5000;
                player.defense = 500;
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

