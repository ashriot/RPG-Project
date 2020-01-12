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
    public GameObject enemyWindows;
    public GameObject enemyAttackEffect;
    public DamageNumber damageNumber;
    public BattleActionDisplay battleActionDisplay;
    public GameObject battleMenu;
    public GameObject targetMenu;
    public GameObject skillsMenu;
    public GameObject spellsMenu;
    public GameObject characterSelectMenu;
    public BattleTooltip actionTooltip;
    public BattleNotification actionNote;
    public Text menuPlayerName;
    public BattleNotification battleNotification;
    public GameObject battleMenuDisabled;
    public Text keyCount;
    public string gameOverScene;
    public BattleAttackButton[] menuButtons;

    private int heroPartySizeOffset;
    private bool battleActive;
    private bool fleeing;
    private float waitTimer;
    private const float INITIATIVE_GROWTH = .05f;
    private const float UI_WINDOW_DISTANCE = 22f;
    
    [Header("Battle Flow Data")]
    public List<int> turnOrder = new List<int>();
    public int currentTurnId;
    public int targetedEnemyId;
    public bool battleWaiting;
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
    public GameObject[] currentTurnCursors;
    public GameObject[] tauntingCursors;
    public GameObject[] enemyTargetCursors;

    public BattleCombatant[] playerPrefabs;
    public BattleCombatant[] enemyPrefabs;
    public EnemyStatWindow[] enemyStatWindows;
    public BattleAction[] battleActions;

    public BattleTargetButton[] targetButtons;
    public BattleMagicSelect[] magicButtons;
    public ItemButton[] itemButtons;

    public Item selectedItem;
    public string selectedItemName;
    public Text itemName, itemDescription, useButtonText;
    public Text[] itemCharacterSelectNames;

    public List<BattleCombatant> combatants = new List<BattleCombatant>();

    public Image[] playerImages;
    public Text[] playerNames, playerHpValues, playerMpValues;
    public Slider[] playerHpSliders, playerMpSliders;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (canvas.gameObject.activeInHierarchy) {
            canvas.gameObject.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (!battleActive || !battleWaiting) { return; }

        // TODO: vvvv Fix this crap vvvvv
        // UpdateBattle();

        currentTurnId = turnOrder[0];
        turnOrder.RemoveAt(0);
        CalculateTurnOrder();

        var enemies = combatants.Where(c => !c.isPlayer);
        if (enemies.All(e => e.isDead)) {
            Debug.Log("YOU WIN");
            ClearAllCursors();
            StartCoroutine(EndBattleCoroutine());
        }
        
        if (!waitingForInput) {
            // status effects
            List<int> effectsToRemove = new List<int>();
            var statusEffects = combatants[currentTurnId].statusEffects;
            for(var i = 0; i < statusEffects.Count; i++) {
                statusEffects[i].duration--;
                if (statusEffects[i].duration == 0) {
                    effectsToRemove.Add(i);
                }
            }
            if (effectsToRemove.Any()) {
                foreach (var i in effectsToRemove) {
                    if (statusEffects[i].name == "Taunt") {
                        combatants[currentTurnId].isTaunting = false;
                        tauntingCursors[currentTurnId].SetActive(false);
                    }
                    statusEffects.RemoveAt(i);
                }
            }

            if (combatants[currentTurnId].isPlayer) {
                // PLAYER TURN

                enemyTargetCursors[targetedEnemyId - heroPartySizeOffset].SetActive(true);
                GameMenu.instance.currentTurnOutlines[currentTurnId].gameObject.SetActive(true);
                currentTurnCursors[currentTurnId].SetActive(true);
                if (combatants.All(x => x.isDead && !x.isPlayer)) return;
                
                AudioManager.instance.PlaySfx("test");
                battleMenu.SetActive(true);
                battleWaiting = false;
                waitingForInput = true;
            } else { // enemy turn
                enemyTargetCursors[targetedEnemyId - heroPartySizeOffset].SetActive(false);
                battleMenu.SetActive(false);
                // enemy should attack
                StartCoroutine(EnemyMoveCoroutine());
            }
        }
    }

    public void BattleStart(string[] enemyIds, bool unableToFlee) {
        if (battleActive) return;

        // ToggleEnemyButtons(false);

        cannotFlee = unableToFlee;
        battleActive = true;
        GameManager.instance.battleActive = true;
        GameMenu.instance.mainMenuButtonText.text = "<Back";
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
        skillsMenu.SetActive(false);
        battleScene.SetActive(true);
        enemyWindows.SetActive(true);
        canvas.gameObject.SetActive(true);
        // battleMenu.SetActive(true);
        actionTooltip.gameObject.SetActive(true);

        var heroes = GameManager.instance.heroes.Where(h => h.isActive).ToArray();

        for (var i = 0; i < playerPositions.Length; i++) {
            if (heroes[i].isActive) {
                for (var j = 0; j < playerPrefabs.Length; j++) {
                    if (playerPrefabs[j].id == heroes[i].id) {
                        var newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                        newPlayer.transform.parent = playerPositions[i];
                        
                        combatants.Add(newPlayer);
                        combatants[i].hp.current = heroes[i].hp.current;
                        combatants[i].hp.maximum = heroes[i].hp.max;
                        combatants[i].mp.current = heroes[i].mp.current;
                        combatants[i].mp.maximum = heroes[i].mp.max;
                        combatants[i].attack = heroes[i].attack.value;
                        combatants[i].defense = heroes[i].defense.value;
                        combatants[i].magic = heroes[i].magic.value;
                        combatants[i].speed = heroes[i].speed.value;
                        combatants[i].armor = heroes[i].armor.value;
                        combatants[i].resist = heroes[i].resist.value;
                        combatants[i].deadSprite = heroes[i].deadSprite;
                        combatants[i].mainHand = heroes[i].mainHand;
                        combatants[i].offHand = heroes[i].offHand;
                        combatants[i].skills = heroes[i].skills;
                        combatants[i].spells = heroes[i].spells;
                        combatants[i].delay = 1f;
                    }
                }
            }
        }

        for (var i = 0; i < enemyIds.Length; i++) {
            if (enemyIds[i] != string.Empty) {
                for (var j = 0; j < enemyPrefabs.Length; j++) {
                    if (enemyPrefabs[j].id == enemyIds[i]) {
                        var newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                        newEnemy.hp.SetToMax();
                        newEnemy.transform.parent = enemyPositions[i];
                        enemyStatWindows[i].gameObject.SetActive(true);
                        enemyStatWindows[i].portrait.sprite = newEnemy.portrait;
                        enemyStatWindows[i].animator.runtimeAnimatorController = newEnemy.portraitAnimatorController;
                        enemyStatWindows[i].hpSlider.fillAmount = newEnemy.hp.percent;
                        enemyStatWindows[i].mpSlider.fillAmount = newEnemy.mp.percent;
                        combatants.Add(newEnemy);
                    }
                }
            }
        }
        heroPartySizeOffset = combatants.Where(c => c.isPlayer).Count();
        OpenTargetMenu();
        SetStartingInitiative();
        CalculateTurnOrder();
        battleWaiting = true;
    }

    private void SetStartingInitiative() {
        foreach (var combatant in combatants) {
            combatant.initiative = CalculateSpeedTicks(combatant);
        }
    }

    private int CalculateSpeedTicks(BattleCombatant combatant) {
        var inverse = Mathf.Pow((1 + INITIATIVE_GROWTH), combatant.speed);
        inverse *= combatant.delay;
        var result = 1000 / inverse;
        Debug.Log(combatant.name + " intiative set to: " + result);
        return (int)result;
    }

    public void CalculateTurnOrder() {
        while (turnOrder.Count < 5) {
            for (var i = 0; i < combatants.Count; i++) {
                if (combatants[i].initiative < 0){
                    Debug.LogError("Initiative went negative!");
                    combatants[i].initiative = 1;
                }
                if (combatants[i].isDead) {
                    turnOrder.RemoveAll(to => to == i);
                    continue;
                }
                combatants[i].initiative--;
                if (combatants[i].initiative == 0) {
                    turnOrder.Add(i);
                    // TODO: Replace with the actual speed of the action they use
                    combatants[i].initiative = CalculateSpeedTicks(combatants[i]);
                }
            }
        }
    }

    public void NextTurn() {
        CalculateTurnOrder();
        if (waitingForInput) {
            waitingForInput = false;
            GameMenu.instance.currentTurnOutlines[currentTurnId].gameObject.SetActive(false);
            currentTurnCursors[currentTurnId].SetActive(false);
        }
        
        currentTurnId++;
        if (currentTurnId >= combatants.Count) {
            currentTurnId = 0;
        }

        battleWaiting = true;
        
        // update enemy target
        if (combatants[targetedEnemyId].isDead) {
            var id = -1;
            for (var i = 0; i < combatants.Count; i++) {
                if (!combatants[i].isPlayer && !combatants[i].isDead) {
                    id = i;
                    break;
                }
            }
            if (id == -1) {
                ClearEnemyTarget();
            } else {
                SetTargetedEnemyId(id);
            }
        }
        UpdateBattle();
    }

    public void UpdateBattle() {

        for (var i = 0; i < enemyStatWindows.Length; i++) {
            if (enemyStatWindows[i].gameObject.activeInHierarchy) {
                enemyStatWindows[i].hpSlider.GetComponentInChildren<Image>().fillAmount = combatants[i+4].hp.percent;
            }
        }
        var heroes = combatants.Where(c => c.isPlayer).ToList();
        var gameHeroes = GameManager.instance.heroes;
        foreach (var hero in heroes) {
            foreach (var gameHero in gameHeroes) {
                if (hero.id == gameHero.id) {
                    gameHero.hp.current = hero.hp.current;
                    gameHero.mp.current = hero.mp.current;
                    break;
                }
            }
        }

        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].hp.current < 0) {
                combatants[i].hp.current = 0;
            }

            if (combatants[i].hp.current == 0 && !combatants[i].isDead) {
                combatants[i].isDead = true; 
                // AudioManager.instance.PlaySfx("test");
                if (combatants[i].isPlayer) {
                    combatants[i].spriteRenderer.sprite = combatants[i].deadSprite;
                } else {
                    combatants[i].EnemyFade();
                    enemyStatWindows[i - heroPartySizeOffset].gameObject.SetActive(false);
                }
            } else if (combatants[i].isPlayer) {
                allPlayersDead = false;
            } else {    // enemy
                allEnemiesDead = false;
            }
        }
        
        if (combatants.All(abc => !abc.isPlayer && abc.isDead)) {
            allEnemiesDead = true;
        }

        if (allEnemiesDead || allPlayersDead) {
            ClearAllCursors();

            if (allEnemiesDead) {
                // end battle in victory
                StartCoroutine(EndBattleCoroutine());
            } else {
                // end battle in failure
                StartCoroutine(GameOverCoroutine());
            }
        }
    }

    public void ClickSkillButton() {
        var actionNames = combatants[currentTurnId].skills;
        if (actionNames.Length == 0) {
            AudioManager.instance.PlaySfx("error");
            Debug.Log("No known skills!");
            return;
        } else {
            PlayClickSound();
        }

        skillsMenu.SetActive(true);
        var actions = new List<BattleAction>();

        foreach(var n in actionNames) {
            actions.Add(BattleActionRepo.instance.GetActionByName(n));
        }

        for(var i = 0; i < menuButtons.Length; i++) {
            if (i < actions.Count) {
                menuButtons[i].gameObject.SetActive(true);
                menuButtons[i].nameText.text = actions[i].name;
                menuButtons[i].image.sprite = actions[i].sprite;
            } else {
                menuButtons[i].gameObject.SetActive(false);
                continue;
            }
        }
    }

    public void ClickSpellsButton() {
        var actionNames = combatants[currentTurnId].spells;
        if (actionNames.Length == 0) {
            AudioManager.instance.PlaySfx("error");
            Debug.Log("No known spells!");
            return;
        } else {
            PlayClickSound();
        }

        skillsMenu.SetActive(true);
        var actions = new List<BattleAction>();

        foreach(var n in actionNames) {
            actions.Add(BattleActionRepo.instance.GetActionByName(n));
        }

        for(var i = 0; i < menuButtons.Length; i++) {
            if (i < actions.Count) {
                menuButtons[i].gameObject.SetActive(true);
                menuButtons[i].nameText.text = actions[i].name;
                menuButtons[i].image.sprite = actions[i].sprite;
            } else {
                menuButtons[i].gameObject.SetActive(false);
                continue;
            }
        }
    }

    private void ClearAllCursors() {
        foreach (var cursor in enemyTargetCursors) {
            cursor.SetActive(false);
        }
        foreach (var cursor in enemyStatWindows) {
            cursor.targetBox.gameObject.SetActive(false);
        }
        foreach (var cursor in GameMenu.instance.currentTurnOutlines) {
            cursor.gameObject.SetActive(false);
        }
    }

    public IEnumerator<WaitForSeconds> EnemyMoveCoroutine() {
        battleWaiting = false;
        yield return new WaitForSeconds(turnDelay);
        var currentUnit = combatants[currentTurnId];
        var selectedActionId = Random.Range(0, currentUnit.skills.Length);
        var action = BattleActionRepo.instance.GetActionByName(currentUnit.skills[selectedActionId]);
        EnemyDeclareAttack(action);
        yield return new WaitForSeconds(turnDelay*1.25f);
        EnemyAttack(action);
        yield return new WaitForSeconds(turnDelay/4);
        NextTurn();
    }

    public void EnemyDeclareAttack(BattleAction action) {
        var position = Camera.main.WorldToScreenPoint(combatants[currentTurnId].transform.position + new Vector3(0,.8f, 0f));
        Instantiate(battleActionDisplay, position, combatants[currentTurnId].transform.rotation, canvas.transform).SetText(action.name);
    }

    public void EnemyAttack(BattleAction action) {
        List<int> players = new List<int>();

        var someoneIsTaunting = combatants.Any(c => c.isPlayer && !c.isDead && c.isTaunting);

        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].isPlayer && !combatants[i].isDead) {
                if (someoneIsTaunting) {
                    if (!combatants[i].isTaunting) {
                        continue;
                    }
                }
                players.Add(i);
            }
        }

        var selectedTargetId = players[Random.Range(0, players.Count)];
        var currentTurn = combatants[currentTurnId];
        var selectedTarget = combatants[selectedTargetId];

        action.attackPower = currentTurn.attack;
        action.minDamage = currentTurn.minDamage;
        action.maxDamage = currentTurn.maxDamage;

        StartCoroutine(FlashSprite(currentTurn.spriteRenderer, Color.clear));
        Instantiate(action.visualFx, selectedTarget.transform.position, selectedTarget.transform.rotation);
        DealDamage(selectedTargetId, action, false);
        StartCoroutine(FlashSprite(selectedTarget.spriteRenderer, Color.red));
        UpdateBattle();
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

    // public IEnumerator<WaitForSeconds> MoveWindow(GameObject gameObject, float distance) {
    //     Vector3 destPos = gameObject.transform.localPosition + new Vector3(distance, 0f, 0f);

    //     while (Vector3.Distance(gameObject.transform.localPosition, destPos)>0.01f) {
    //         Debug.Log(destPos);
    //         gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, destPos, 150f * Time.deltaTime);
    //         yield return null;
    //     }
    // }

    public void DealDamage(int selectedTargetId, BattleAction battleAction, bool isPlayer) {
        var currentTurn = combatants[currentTurnId];
        var selectedTarget = combatants[selectedTargetId];
        var attackPower = battleAction.attackPower;
        var defensePower = 0;
        var armorResist = 0;
        var numberOfAttacks = 1;
        var damageToDeal = 0;
        var isCrit = false;
        var isGraze = false;
        var isMiss = false;

        for (var i = 0; i < numberOfAttacks; i++) {
            switch (battleAction.actionType) {
                case BattleActionType.Physical:
                    defensePower = selectedTarget.defense;
                    armorResist = selectedTarget.armor;
                break;

                case BattleActionType.Magical:
                    defensePower = selectedTarget.magic;
                    armorResist = selectedTarget.resist;
                break;
                
                case BattleActionType.Healing:
                    defensePower = 1;
                break;

                default:
                Debug.LogError("Reached an invalid Battle Action Type.");
                break;
            }
            damageToDeal = Random.Range(battleAction.minDamage, battleAction.maxDamage);

            damageToDeal = Mathf.Clamp(damageToDeal - armorResist, 0, damageToDeal - armorResist);
            var attackRoll = Random.Range(1, 20);
            var hit = attackRoll + attackPower - defensePower;

            var debugString = currentTurn.name + " rolls " + attackRoll + " + " + attackPower + " - " + defensePower + " = " + hit;

            if (hit < 4) { // miss
                damageToDeal = 0;
                isMiss = true;
                debugString += " Miss!";
                AudioManager.instance.PlaySfx("swing");
            } else if (hit < 8) { // graze
                damageToDeal /= 2;
                debugString += " Graze!";
                isGraze = true;
                AudioManager.instance.PlaySfx("swing");
            } else if (hit < 22) { // hit
                damageToDeal *= 1;
                debugString += " Hit!";
                AudioManager.instance.PlaySfx("impact_a");
            } else { // crit!
                damageToDeal *= 3/2;
                isCrit = true;
                debugString += " CRIT!!";
                AudioManager.instance.PlaySfx("impact_b");
            }

            Mathf.Clamp(damageToDeal, damageToDeal - armorResist, 0);

            selectedTarget.hp.current -= battleAction.actionType != BattleActionType.Healing ? damageToDeal : damageToDeal * -1;

            debugString += " " + battleAction.name + " deals " + damageToDeal + " dmg. to "
                + selectedTarget.name + " [HP: " + selectedTarget.hp.current + "/" + selectedTarget.hp.max + "]";

            Debug.Log(debugString);
            var position = Camera.main.WorldToScreenPoint(selectedTarget.transform.position + new Vector3(0f, .5f, 0f));
            Instantiate(damageNumber, position, selectedTarget.transform.rotation, canvas.transform)
                .SetDamage(damageToDeal, isCrit, isGraze, isMiss, isPlayer);
        }
        UpdateBattle();
        UpdateUiStats();
    }

    public void UpdateUiStats() {
        GameMenu.instance.UpdateStats();
    }

    public void HeroAction(string actionName) {
        var action = BattleActionRepo.instance.GetActionByName(actionName);
        if (combatants[currentTurnId].mp.current < action.mpCost) {
            AudioManager.instance.PlaySfx("error");
            Debug.Log("Not enough MP!");
            return;
        }
        skillsMenu.SetActive(false);
        Debug.Log("Action used: " + actionName);
        StartCoroutine(HeroActionCoroutine(action));
    }
    
    public IEnumerator<WaitForSeconds> HeroActionCoroutine(BattleAction action) {
        var hits = 1;
        var currentTurn = combatants[currentTurnId];
        var selectedTarget = combatants[targetedEnemyId];
        actionTooltip.description.text = string.Empty;
        battleMenu.SetActive(false);

        // animation
        StartCoroutine(FlashSprite(currentTurn.spriteRenderer, Color.black));

        var mainHand = currentTurn.mainHand as Weapon;
        var offHand = ScriptableObject.CreateInstance<Weapon>();

        if (currentTurn.dualWielding) {
            offHand = currentTurn.offHand as Weapon;
        }

        if (action.visualFx != null) {
            Instantiate(action.visualFx, selectedTarget.transform.position, selectedTarget.transform.rotation);
        }

        
        if (action.name == "Taunt") {
            AudioManager.instance.PlaySfx("rebound");
            combatants[currentTurnId].isTaunting = true;
            BattleActionRepo.instance.ApplyStatusEffectToSelf("Taunt", 2);
            tauntingCursors[currentTurnId].SetActive(true);
        } else if (action.name == "Fireball") {
            AudioManager.instance.PlaySfx("fire_c");
            action.attackPower = currentTurn.magic;
            DealDamage(targetedEnemyId, action, true);

        } else if (action.name == "Attack") {
            action.attackPower = currentTurn.attack + mainHand.attackBonus - (currentTurn.dualWielding ? 6 : 0);
            action.minDamage = mainHand.minimumDamage;
            action.maxDamage = mainHand.maximumDamage;
            action.delay = mainHand.delay;

            // mainhand attack
            for (var i = 0; i < hits; i++) {
                DealDamage(targetedEnemyId, action, true);
            }

            if (currentTurn.dualWielding) {
                yield return new WaitForSeconds(turnDelay);

                action = ScriptableObject.CreateInstance<BattleAction>();
                action.name = "Attack";
                action.actionType = BattleActionType.Physical;
                action.attackPower = currentTurn.attack + offHand.attackBonus - (currentTurn.dualWielding ? 10 : 0);
                action.minDamage = offHand.minimumDamage;
                action.maxDamage = offHand.maximumDamage;
                action.delay = (mainHand.delay + offHand.delay) / 2f;

                for (var i = 0; i < hits; i++) {
                    DealDamage(targetedEnemyId, action, true);
                }
            }
        }

        currentTurn.mp.current -= action.mpCost;
        currentTurn.delay = action.delay;

        yield return new WaitForSeconds(turnDelay);
        NextTurn();
    }

    private void ToggleEnemyButtons(bool value) {
        foreach(var panel in enemyStatWindows) {
            panel.GetComponent<Button>().interactable = value;
        }
    }

    public void SetTargetedEnemyId(int id) {
        if (combatants[id].isDead) return;

        enemyTargetCursors[targetedEnemyId - heroPartySizeOffset].SetActive(false);
        enemyStatWindows[targetedEnemyId - heroPartySizeOffset].targetBox.gameObject.SetActive(false);
        enemyTargetCursors[id - heroPartySizeOffset].SetActive(true);
        enemyStatWindows[id - heroPartySizeOffset].targetBox.gameObject.SetActive(true);
        targetedEnemyId = id;
    }

    public void ClearEnemyTarget() {
        enemyTargetCursors[targetedEnemyId - heroPartySizeOffset].SetActive(false);
        enemyStatWindows[targetedEnemyId - heroPartySizeOffset].targetBox.gameObject.SetActive(false);
    }

    public void PlayClickSound() {
        AudioManager.instance.PlaySfx("select_a");
    }

    public void OpenTargetMenu(/* string actionName */) {
        // Debug.Log("Clicked a button!");
        // var action = GetBattleAction(actionName);

        // BattleManager.instance.actionTooltip.description.text = action.description;
        
        // targetMenu.SetActive(true);

        ToggleEnemyButtons(true);
        var enemyIds = new List<int>();

        for (var i = 0; i < combatants.Count; i++) {
            if (!combatants[i].isPlayer) {
                enemyIds.Add(i);
            }
        }

        for (var i = 0; i < enemyStatWindows.Length; i++) {
            if (enemyIds.Count > i && combatants[enemyIds[i]].hp.current > 0) {
                var enemy = combatants[enemyIds[i]];

                // targetButtons[i].actionName = action.actionName;
                enemyStatWindows[i].targetId = enemyIds[i];

            } else {
                enemyStatWindows[i].gameObject.SetActive(false);
            }
        }
        targetedEnemyId = 4;
        SetTargetedEnemyId(4);
    }

    public void OpenMagicMenu() {
        actionTooltip.gameObject.SetActive(false);
        targetMenu.SetActive(false);

        // magicMenu.SetActive(true);
        battleMenuDisabled.SetActive(true);

        var magicNames = combatants[currentTurnId].skills;

        for (var i = 0; i < magicButtons.Length; i++) {
            if (i >= magicNames.Length) {
                magicButtons[i].gameObject.SetActive(false);
                continue;
            }

            var magicInfo = battleActions.Where(ba => ba.name == magicNames[i]).Single();

            var newButton = magicButtons[i];
            newButton.gameObject.SetActive(true);
            newButton.magicName = magicInfo.name;
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
        battleMenu.SetActive(false);
        AudioManager.instance.PlayBgm("victory");

        yield return new WaitForSeconds(1.5f);

        UIFade.instance.FadeToBlack();
        canvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].isPlayer) {
                for (var j = 0; j < GameManager.instance.heroes.Length; j++) {
                    if (combatants[i].name == GameManager.instance.heroes[j].name) {
                        GameManager.instance.heroes[j].hp.current = combatants[i].hp.current;
                        GameManager.instance.heroes[j].mp.current = combatants[i].mp.current;
                        break;
                    }
                }
            }

            Destroy(combatants[i].gameObject);
        }

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        enemyWindows.SetActive(false);
        combatants.Clear();
        currentTurnId = 0;
        if (fleeing) {
            fleeing = false;
            GameManager.instance.battleActive = false;
        } else {
            BattleReward.instance.OpenWindow(xpEarned);
        }

        AudioManager.instance.PlayBgm(FindObjectOfType<CameraController>().musicNameToPlay);
    }

    public IEnumerator<WaitForSeconds> GameOverCoroutine() {
        battleNotification.text.text = "Game Over!";
        battleNotification.Activate();
        battleActive = false;
        yield return new WaitForSeconds(2f);
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(.5f);
        battleScene.SetActive(false);
        enemyWindows.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }

    public void Back() {
        actionTooltip.gameObject.SetActive(false);
        // magicMenu.SetActive(false);
        targetMenu.SetActive(false);
        battleMenuDisabled.SetActive(false);
    }

    public BattleAction GetBattleAction (string name) {
        for (var i = 0; i < battleActions.Length; i++) {
            if (battleActions[i].name == name) {
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
            foreach(var player in BattleManager.instance.combatants) {
                if (player.isPlayer) {
                    player.hp.current = 1;
                }
            }
        }
        if(GUILayout.Button("God Mode!!")) {
            foreach(var player in GameManager.instance.heroes) {
                // player.hp.current = 999;
                // player.hp.max = 999;
                // player.mp.current = 999;
                // player.mp.max = 999;
                // player.attack = 5000;
                // player.defense = 500;
            }
            foreach(var player in BattleManager.instance.combatants) {
                if (player.isPlayer) {
                    player.hp.current = 999;
                    player.hp.maximum = 999;
                    player.mp.current = 999;
                    player.mp.maximum = 999;
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

