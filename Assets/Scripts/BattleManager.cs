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

    public GameObject openWindow;
    public Sprite fleeSprite;
    public Sprite backSprite;
    public Sprite menuSprite;
    public Sprite[] enemyIcons;
    public Sprite[] actionIcons;
    public Sprite[] heroIcons;

    private int partySizeOffset;
    private bool battleActive;
    private bool fleeing;
    private float waitTimer;
    private const float INITIATIVE_GROWTH = .05f;
    private const float UI_WINDOW_DISTANCE = 22f;
    
    [Header("Battle Flow Data")]
    // public List<int> turnOrder = new List<int>();
    public List<int> potentialTurnOrder = new List<int>();
    public int combatantId;
    public int targetId;
    public BattleCombatant combatant;
    public BattleCombatant target;
    public BattleCombatant enemyTarget;
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
    public GameObject[] chargingCursors;
    public GameObject[] enemyTargetCursors;

    public BattleCombatant[] playerPrefabs;
    public BattleCombatant[] enemyPrefabs;
    public EnemyStatWindow[] enemyStatWindows;
    public Ability[] battleActions;

    public GameObject classSkillButton;
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
    public SpriteRenderer[] tpSpriteRenderers;
    public Sprite tp1Sprite, tp2Sprite, tp3Sprite;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (canvas.gameObject.activeInHierarchy) {
            canvas.gameObject.SetActive(false);
        }
    }

    public void BattleStart(string[] enemyIds, bool unableToFlee) {
        if (battleActive) return;

        xpEarned = 0;
        cannotFlee = unableToFlee;
        battleActive = true;
        GameManager.instance.battleActive = true;
        SetMenuButtonToFlee();
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
        skillsMenu.SetActive(false);
        battleScene.SetActive(true);
        enemyWindows.SetActive(true);
        canvas.gameObject.SetActive(true);
        actionTooltip.gameObject.SetActive(true);

        var heroes = GameManager.instance.heroes.Where(h => h.isActive).ToArray();
        PlayerController.instance.GetComponent<SpriteRenderer>().enabled = false;
        foreach(var member in PlayerController.instance.partyMembers) {
            member.GetComponent<SpriteRenderer>().enabled = false;
        }

        for (var i = 0; i < playerPositions.Length; i++) {
            if (heroes[i].isActive) {
                for (var j = 0; j < playerPrefabs.Length; j++) {
                    if (playerPrefabs[j].id == heroes[i].id) {
                        var newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                        newPlayer.transform.parent = playerPositions[i];
                        
                        combatants.Add(newPlayer);
                        combatants[i].classSkillName = heroes[i].classSkillName;
                        var description = heroes[i].classSkillDescription.Replace("\\n", "\n");
                        combatants[i].classSkillDescription = description;
                        combatants[i].hp.current = heroes[i].hp.current;
                        combatants[i].hp.baseMax = heroes[i].hp.maximum;
                        combatants[i].mp.current = heroes[i].mp.current;
                        combatants[i].mp.baseMax = heroes[i].mp.maximum;
                        combatants[i].attack = heroes[i].attack.value;
                        combatants[i].defense = heroes[i].defense.value;
                        combatants[i].magic = heroes[i].magic.value;
                        combatants[i].speed = heroes[i].speed.value;
                        combatants[i].armor = heroes[i].armor.value;
                        combatants[i].resist = heroes[i].resist.value;
                        combatants[i].deadSprite = heroes[i].deadSprite;
                        combatants[i].mainHand = heroes[i].handEquipment[0];
                        combatants[i].offHand = heroes[i].handEquipment[1];
                        combatants[i].abilities = heroes[i].skills;
                        combatants[i].spells = heroes[i].spells;
                        combatants[i].tpChanceBase = heroes[i].tpChanceBase;
                        combatants[i].delay = 1f;
                        combatants[i].deflect = heroes[i].armor.value * 2;
                    }
                }
            }
        }

        for (var i = 0; i < enemyIds.Length; i++) {
            if (enemyIds[i] != string.Empty) {
                var enemyPrefab = GetEnemyPrefabById(enemyIds[i]);
                var newEnemy = Instantiate(enemyPrefab, enemyPositions[i].position, enemyPositions[i].rotation);
                enemyStatWindows[i].enemyIcon.sprite = enemyIcons[i];
                SetEnemyTargetAndAction(newEnemy, enemyStatWindows[i].actionIcon, enemyStatWindows[i].targetIcon);
                newEnemy.hp.SetToMax();
                newEnemy.deflect = newEnemy.armor *2;
                newEnemy.transform.parent = enemyPositions[i];
                enemyStatWindows[i].gameObject.SetActive(true);
                enemyStatWindows[i].portrait.sprite = newEnemy.portrait;
                enemyStatWindows[i].animator.runtimeAnimatorController = newEnemy.portraitAnimatorController;
                enemyStatWindows[i].hpSlider.fillAmount = newEnemy.hp.percent;
                enemyStatWindows[i].tempHpSlider.fillAmount = (float)newEnemy.deflect / newEnemy.hp.maximum;
                enemyStatWindows[i].mpSlider.fillAmount = newEnemy.mp.percent;
                combatants.Add(newEnemy);
                Debug.Log(xpEarned);
                xpEarned += newEnemy.xp;
            }
        }
        partySizeOffset = combatants.Where(c => c.isPlayer).Count();
        OpenTargetMenu();
        SetInitialTicks();
        battleWaiting = true;
        UpdateUiStats();
        CountdownTicks();
    }

    private BattleCombatant GetEnemyPrefabById(string enemyId) {
        var enemy = Resources.Load<BattleCombatant>("Enemies/" + enemyId);

        if (enemy != null) {
            return enemy;
        }

        Debug.LogError("Couldn't find enemy '" + enemyId +"'!");
        return null;
    }

    private void SetInitialTicks() {
        foreach (var c in combatants) {
            CalculateSpeedTicks(c, 1f);
        }
    }

    void FixedUpdate() {
        if (!battleActive || !battleWaiting) { return; }

        // currentTurnId = turnOrder[0];
        // potentialTurnOrder.RemoveAt(0);
        
        if (!waitingForInput) {
            // status effects
            combatant = combatants[combatantId];
            target = combatants[targetId];
            var effectsToRemove = new List<int>();
            var statusEffects = combatant.statusEffects
                .Where(se => se.baseStatusEffect.expiry == StatusEffectExpiries.StartOfTurn).ToList();
            for(var i = 0; i < statusEffects.Count; i++) {
                statusEffects[i].stacks--;
                if (statusEffects[i].stacks == 0) {
                    effectsToRemove.Add(i);
                }
            }
            if (effectsToRemove.Any()) {
                foreach (var i in effectsToRemove) {
                    if (statusEffects[i].name == "Taunt") {
                        combatant.isTaunting = false;
                        tauntingCursors[combatantId].SetActive(false);
                    }
                    statusEffects.RemoveAt(i);
                }
            }

            if (combatant.isPlayer) {
                // PLAYER TURN
                if (combatants.All(x => x.isDead && !x.isPlayer)) return;

                if (combatant.isCharging) {
                    enemyTargetCursors[targetId - partySizeOffset].SetActive(false);
                    enemyTargetCursors[combatant.targetId - partySizeOffset].SetActive(true);
                    HeroAction(combatant.chargedAbilityName);
                } else {
                    AudioManager.instance.PlaySfx("end_turn");
                    classSkillButton.GetComponentInChildren<Text>().text = combatant.classSkillName;
                    classSkillButton.GetComponentInChildren<ButtonLongPress>().description = combatant.classSkillDescription;
                    battleMenu.SetActive(true);
                    enemyTargetCursors[targetId - partySizeOffset].SetActive(true);
                    GameMenu.instance.heroStatPanels[combatantId].currentTurnCursor.gameObject.SetActive(true);
                    currentTurnCursors[combatantId].SetActive(true);
                }
                battleWaiting = false;
                waitingForInput = true;
            } else {
                // ENEMY TURN
                enemyTargetCursors[targetId - partySizeOffset].SetActive(false);
                battleMenu.SetActive(false);
                // enemy should attack
                StartCoroutine(EnemyMoveCoroutine());
            }
        }
    }

    
    private void CalculateSpeedTicks(BattleCombatant combatant, float delay) {
        var inverse = Mathf.Pow((1 + INITIATIVE_GROWTH), combatant.speed);
        var result = 1000 / inverse;
        result *= delay;
        // Debug.Log(combatant.name + " ticks: " + (int)result + " delay: " + delay);
        combatant.ticks = (int)result;
    }

    // public void PredictTurnOrder() {
    //     while (potentialTurnOrder.Count < 5) {
    //         for (var i = 0; i < combatants.Count; i++) {
    //             if (combatants[i].ticks < 0){
    //                 Debug.LogError("Initiative went negative!");
    //                 combatants[i].ticks = 1;
    //             }
    //             if (combatants[i].isDead) {
    //                 potentialTurnOrder.RemoveAll(to => to == i);
    //                 continue;
    //             }
    //             combatants[i].ticks--;
    //             if (combatants[i].ticks == 0) {
    //                 potentialTurnOrder.Add(i);
    //                 // TODO: Replace with the actual speed of the action they use
    //                 combatants[i].ticks = CalculateSpeedTicks(combatants[i], 1f);
    //             }
    //         }
    //     }
    // }

    public void CountdownTicks() {
        while(true) {
            for (var i = 0; i < combatants.Count; i++) {
                if (!combatants[i].isDead && combatants[i].ticks == 0) {
                    combatantId = i;
                    return;
                } else {
                    combatants[i].ticks--;
                }
            }
        }
    }

    public void NextTurn() {
        CheckForBattleEnd();
        // while (turnOrder.Count < 10) {
        //     PredictTurnOrder();
        // }

        UpdateUiStats();
        CountdownTicks();
        
        if (waitingForInput) {
            waitingForInput = false;
        }

        battleWaiting = true;
        
        // update enemy target
        CheckIfEnemyTargetIsDead();
    }

    private void CheckIfEnemyTargetIsDead() {
        if (target.isDead) {
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
    }

    private void UpdateEnemyStatWindows() {
        for (var i = 0; i < enemyStatWindows.Length; i++) {
            if (enemyStatWindows[i].gameObject.activeInHierarchy) {
                enemyStatWindows[i].hpSlider.fillAmount = combatants[i+4].hp.percent;
                enemyStatWindows[i].tempHpSlider.fillAmount = (float)combatants[i+4].deflect / combatants[i+4].hp.maximum;
            }
        }
    }

    public void CheckForBattleEnd() {
        UpdateUiStats();
        UpdateEnemyStatWindows();

        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].isDead) {
                if (combatants[i].isPlayer) {
                    combatants[i].spriteRenderer.sprite = combatants[i].deadSprite;
                } else {
                    if (enemyStatWindows[i - partySizeOffset].gameObject.activeInHierarchy) {
                        combatants[i].EnemyFade();
                        enemyStatWindows[i - partySizeOffset].gameObject.SetActive(false);
                    }
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

    public void ClickAttackButton() {
        PlayClickSound();
        HeroAction("Attack");
    }

    private void SetMenuButtonToBack(GameObject window) {
        openWindow = window;
        GameMenu.instance.mainMenuButtonIcon.sprite = backSprite;
        GameMenu.instance.mainMenuButtonText.text = "<Back";
    }

    public void SetMenuButtonToFlee() {
        if (openWindow != null) {
            openWindow.SetActive(false);
            openWindow = null;
        }
        GameMenu.instance.mainMenuButtonIcon.sprite = fleeSprite;
        GameMenu.instance.mainMenuButtonText.text = "Flee";
    }

    private void ResetMainMenuButton() {
        openWindow = null;
        GameMenu.instance.mainMenuButtonIcon.sprite = menuSprite;
        GameMenu.instance.mainMenuButtonText.text = "Menu";
    }

    public void ClickSkillButton() {
        var actionNames = combatant.abilities;
        if (actionNames.Length == 0) {
            AudioManager.instance.PlaySfx("error");
            Debug.Log("No known skills!");
            return;
        } else {
            PlayClickSound();
        }

        skillsMenu.SetActive(true);

        SetMenuButtonToBack(skillsMenu);
        var actions = new List<Ability>();

        foreach(var n in actionNames) {
            actions.Add(BattleActionRepo.instance.GetAbilityByName(n));
        }

        for(var i = 0; i < menuButtons.Length; i++) {
            if (i < actions.Count) {
                menuButtons[i].gameObject.SetActive(true);
                menuButtons[i].nameText.text = actions[i].name;
                menuButtons[i].image.sprite = actions[i].sprite;
                menuButtons[i].GetComponent<ButtonLongPress>().description = actions[i].description;
            } else {
                menuButtons[i].gameObject.SetActive(false);
                continue;
            }
        }
    }

    public void ClickSpellsButton() {
        var actionNames = combatant.spells;
        if (actionNames.Length == 0) {
            AudioManager.instance.PlaySfx("error");
            Debug.Log("No known spells!");
            return;
        } else {
            PlayClickSound();
        }

        skillsMenu.SetActive(true);
        SetMenuButtonToBack(skillsMenu);

        var actions = new List<Ability>();

        foreach(var n in actionNames) {
            actions.Add(BattleActionRepo.instance.GetAbilityByName(n));
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
        Debug.Log("clearing cursors");
        foreach (var cursor in enemyTargetCursors) {
            cursor.SetActive(false);
        }
        foreach (var cursor in enemyStatWindows) {
            cursor.targetCursor.gameObject.SetActive(false);
        }
        foreach (var hero in GameMenu.instance.heroStatPanels) {
            hero.currentTurnCursor.gameObject.SetActive(false);
        }
    }

    private void SetEnemyTargetAndAction(BattleCombatant enemy, Image actionIcon, Image targetIcon) {
        var selectedAbilityId = Random.Range(0, enemy.abilities.Length);
        enemy.chargedAbilityName = enemy.abilities[selectedAbilityId];
        var selectedAbilityType = BattleActionRepo.instance.GetAbilityTypeByName(enemy.chargedAbilityName);
        actionIcon.sprite = actionIcons[(int)selectedAbilityType];

        List<BattleCombatant> playerCombatants = new List<BattleCombatant>();
        var someoneIsTaunting = combatants.Any(c => c.isPlayer && !c.isDead && c.isTaunting);
        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].isPlayer && !combatants[i].isDead) {
                if (someoneIsTaunting) {
                    if (!combatants[i].isTaunting) {
                        continue;
                    }
                }
                playerCombatants.Add(combatants[i]);
            }
        }

        enemy.targetId = Random.Range(0, playerCombatants.Count);
        targetIcon.sprite = heroIcons[enemy.targetId];
    }

    private int UpdateEnemyTargetDueToTauntOrHide() {
        List<BattleCombatant> playerCombatants = new List<BattleCombatant>();
        var someoneIsTaunting = combatants.Any(c => c.isPlayer && !c.isDead && c.isTaunting);
        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].isPlayer && !combatants[i].isDead) {
                if (someoneIsTaunting) {
                    if (!combatants[i].isTaunting) {
                        continue;
                    }
                }
                playerCombatants.Add(combatants[i]);
            }
        }

        return Random.Range(0, playerCombatants.Count);
    }

    public IEnumerator<WaitForSeconds> EnemyMoveCoroutine() {
        battleWaiting = false;
        yield return new WaitForSeconds(turnDelay);
        var someoneIsTaunting = combatants.Any(c => c.isPlayer && !c.isDead && c.isTaunting);
        if (someoneIsTaunting) {
            combatant.targetId = UpdateEnemyTargetDueToTauntOrHide();
        }
        var ability = BattleActionRepo.instance.GetAbilityByName(combatant.chargedAbilityName);
        EnemyDeclareAttack(ability);
        yield return new WaitForSeconds(turnDelay*1.25f);
        foreach(var action in ability.actions) {
            EnemyAttack(action);
            yield return new WaitForSeconds(turnDelay/4);
        }
        SetEnemyTargetAndAction(combatant, enemyStatWindows[combatantId - partySizeOffset].actionIcon, enemyStatWindows[combatantId - partySizeOffset].targetIcon);
        NextTurn();
    }

    public void EnemyDeclareAttack(Ability ability) {
        var position = Camera.main.WorldToScreenPoint(combatant.transform.position + new Vector3(0,.8f, 0f));
        Instantiate(battleActionDisplay, position, combatant.transform.rotation, canvas.transform).SetText(ability.name);
    }

    public void EnemyAttack(AbilityAction action) {
        enemyTarget = combatants[combatant.targetId];

        // action.attackPower = currentTurn.attack;
        // action.minDamage = currentTurn.minDamage;
        // action.maxDamage = currentTurn.maxDamage;

        StartCoroutine(FlashSprite(combatant.spriteRenderer, Color.clear));
        Instantiate(action.abilityFx, enemyTarget.transform.position, enemyTarget.transform.rotation);
        DealDamage(action, false);
        StartCoroutine(FlashSprite(enemyTarget.spriteRenderer, Color.red));
        CalculateSpeedTicks(combatant, 1f);
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

    public void DealDamage(AbilityAction action, bool isPlayer) {
        var localTarget = isPlayer ? target : enemyTarget;

        var hitPower = 0;
        var avoidPower = 0;
        var armorResist = 0;
        var numberOfAttacks = 1;
        var damageToDeal = 0;
        var isCrit = false;
        var isGraze = false;
        var isMiss = false;
        
        switch (action.hitType) {
            case StatTypes.Attack:
                hitPower = combatant.attack;
                break;
            case StatTypes.Defense:
                hitPower = combatant.defense;
                break;
            case StatTypes.Magic:
                hitPower = combatant.magic;
                break;
            case StatTypes.Speed:
                hitPower = combatant.speed;
                break;
            default:
                Debug.LogError("Missing Attack StatType!");
                break;
        };
        switch (action.avoidType) {
            case StatTypes.Attack:
                avoidPower = localTarget.attack;
                armorResist = 0;
                break;
            case StatTypes.Defense:
                avoidPower = localTarget.defense;
                armorResist = localTarget.armor;
                break;
            case StatTypes.Magic:
                avoidPower = localTarget.magic;
                armorResist = localTarget.resist;
                break;
            case StatTypes.Speed:
                avoidPower = localTarget.speed;
                armorResist = 0;
                break;
            default:
                Debug.LogError("Missing Defense StatType!");
                break;
        };

        switch (action.damageType) {
            case DamageTypes.Physical:
                armorResist = localTarget.armor;
                break;
            case DamageTypes.Magical:
                armorResist = localTarget.resist;
                break;
            case DamageTypes.True:
                armorResist = 0;
                break;
            default:
                Debug.LogError("Missing damage type!");
                break;
        }

        for (var i = 0; i < numberOfAttacks; i++) {
            if (action.isWeaponAttack) {
                var weapon = combatant.mainHand as Weapon;
                action.minimumPotency = weapon.minimumDamage;
                action.maximumPotency = weapon.maximumDamage;
                // hitPower += combatant.mainHand.statBonuses[(int)Stats.Attack] - (combatant.isDualWielding ? 6 : 0);
            }

            damageToDeal = Random.Range(action.minimumPotency, action.maximumPotency);
            var db = " base dmg " + damageToDeal;

            var attackRoll = Random.Range(1, 20);

            var hit = attackRoll + hitPower - avoidPower;

            var debugString = combatant.name + " rolls " + attackRoll + " + " + hitPower + " - " + avoidPower + " = " + hit;

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
            } else if (hit < 20) { // hit
                damageToDeal *= 1;
                debugString += " Hit!";
                AudioManager.instance.PlaySfx("impact_a");
            } else { // crit!
                var critDmg = (float) damageToDeal * 1.5f;
                damageToDeal = (int) critDmg;
                isCrit = true;
                debugString += " CRIT!!";
                AudioManager.instance.PlaySfx("impact_b");
            }

            damageToDeal = Mathf.Clamp(damageToDeal - armorResist, 0, damageToDeal - armorResist);

            db += " modified dmg " + damageToDeal + " resist: " + armorResist;

            debugString += " deals " + damageToDeal + " dmg to "
                + localTarget.name + " [HP: " + localTarget.hp.current + "/" + localTarget.hp.maximum + "]" + db;

            Debug.Log(debugString);
            var position = Camera.main.WorldToScreenPoint(localTarget.transform.position + new Vector3(0f, .5f, 0f));
            Instantiate(damageNumber, position, localTarget.transform.rotation, canvas.transform)
                .SetDamage(damageToDeal, isCrit, isGraze, isMiss, isPlayer);

            if (localTarget.deflect > 0) {
                if (localTarget.deflect > damageToDeal) {
                    localTarget.deflect -= damageToDeal;
                } else {
                    damageToDeal -= localTarget.deflect;
                    localTarget.deflect = 0;
                    localTarget.hp.Decerease(damageToDeal);
                }
            } else {
                localTarget.hp.Decerease(damageToDeal);
            }
            UpdateEnemyStatWindows();
            UpdateUiStats();
        }
    }

    public void UpdateUiStats() {
        for(var i = 0; i < combatants.Count; i++) {
            foreach(var se in combatants[i].statusEffects) {
                if (se.name == "Taunt") {
                    combatants[i].isTaunting = true;
                    tauntingCursors[i].SetActive(true);
                }
            }
        }

        var heroes = combatants.Where(c => c.isPlayer).ToList();
        var gameHeroes = GameManager.instance.heroes;
        foreach (var hero in heroes) {
            foreach (var gameHero in gameHeroes) {
                if (hero.id == gameHero.id) {
                    gameHero.deflect = hero.deflect;
                    gameHero.hp.current = hero.hp.current;
                    gameHero.mp.current = hero.mp.current;
                    break;
                }
            }
        }
        GameMenu.instance.UpdateStats();
    }

    public void HeroAction(string actionName) {
        var abilityRef = BattleActionRepo.instance.GetAbilityByName(actionName);
        var ability = Instantiate(abilityRef);

        if (combatant.isCharging) {
            ability.delay = 0;
        } else {
            if (combatant.mp.current < ability.mpCost) {
                AudioManager.instance.PlaySfx("error");
                Debug.Log("Not enough MP!");
                return;
            }
        }
        battleMenu.SetActive(false);
        skillsMenu.SetActive(false);
        Debug.Log("Action used: " + actionName);

        StartCoroutine(HeroActionCoroutine(ability));
    }
    
    public IEnumerator<WaitForSeconds> HeroActionCoroutine(Ability ability) {
        if (openWindow != null) {
            SetMenuButtonToFlee();
        }
        if (ability.chargeType == ChargeTypes.Charge) {
            if (!combatant.isCharging) {
                combatant.isCharging = true;
                AudioManager.instance.PlaySfx("special_a");
                combatant.chargedAbilityName = ability.name;
                combatant.targetId = targetId;
                chargingCursors[combatantId].SetActive(true);
                CalculateSpeedTicks(combatant, ability.delay);
                GameMenu.instance.heroStatPanels[combatantId].currentTurnCursor.gameObject.SetActive(false);
                currentTurnCursors[combatantId].SetActive(false);
                yield return new WaitForSeconds(turnDelay);
                NextTurn();
                yield break;
            } else {
                targetId = combatant.targetId;
                target = combatants[targetId];
                CheckIfEnemyTargetIsDead();
                chargingCursors[combatantId].SetActive(false);
                var position = Camera.main.WorldToScreenPoint(combatant.transform.position + new Vector3(0,.8f, 0f));
                Instantiate(battleActionDisplay, position, combatant.transform.rotation, canvas.transform).SetText(ability.name);
                yield return new WaitForSeconds(turnDelay * 2);
            }
        }
        
        combatant.mp.current -= ability.mpCost;
        UpdateUiStats();

        var hits = 1;

        // animation
        StartCoroutine(FlashSprite(combatant.spriteRenderer, Color.black));
        // loop through actions
        foreach(var action in ability.actions) {
            var localTarget = action.targetType == TargetTypes.Self ? combatant : target;
            if (action.abilityFx != null) {
                Instantiate(action.abilityFx, localTarget.transform.position, localTarget.transform.rotation);
            }

            if (action.isWeaponAttack) {
                var mainHand = combatant.mainHand as Weapon;
                var offHand = ScriptableObject.CreateInstance<Weapon>();

                if (combatant.isDualWielding) {
                    offHand = combatant.offHand as Weapon;
                }
                // Main Hand
                action.hitType = mainHand.hitType;
                action.avoidType = mainHand.avoidType;
                action.hitModifier = combatant.isDualWielding ? mainHand.baseAttack - 6 : mainHand.baseAttack;
                action.minimumPotency = mainHand.minimumDamage;
                action.maximumPotency = mainHand.maximumDamage;
                for (var i = 0; i < hits; i++) {
                    DealDamage(action, true);
                }

                if (combatant.isDualWielding) {
                    yield return new WaitForSeconds(turnDelay);
                    ability.delay = (mainHand.delay + offHand.delay) / 2;

                    action.hitType = offHand.hitType;
                    action.avoidType = offHand.avoidType;
                    action.hitModifier = combatant.isDualWielding ? offHand.baseAttack - 10 : offHand.baseAttack;
                    action.minimumPotency = offHand.minimumDamage;
                    action.maximumPotency = offHand.maximumDamage;
                    for (var i = 0; i < hits; i++) {
                        DealDamage(action, true);
                    }

                } else {    // Main Hand Only
                    ability.delay = mainHand.delay;
                }
            }

            foreach(var se in action.statusEffects) {
                localTarget.statusEffects.Add(se);
            }

            if (action.isEffectOnly) {
                // Gain MP
                if (action.avoidType == StatTypes.Mp) {
                    Debug.Log("MP increased by " + (int)((float)combatant.mp.missing * action.potencyModifier));
                    combatant.mp.Increase((int)((float)combatant.mp.missing * action.potencyModifier));
                }
                // Gain Armor
                if (action.avoidType == StatTypes.Armor) {
                    var value = 0f;
                    if (action.hitType == StatTypes.Magic) {
                        value = combatant.magic;
                    }
                    value *= action.potencyModifier;
                    combatant.armor += (int)value;
                    Debug.Log("Armor increased by " + (int)value);
                }
            } else {
                if (!action.isWeaponAttack) {
                    DealDamage(action, true);
                }
            }
        }

        // temporary
        if (combatant.tp < 3) {
            if(combatant.tpChance < combatant.tpChanceBase) {
                combatant.tpChance = combatant.tpChanceBase;
            }
            var roll = Random.Range(0f, 1f);
            Debug.Log("TP Chance: " + combatant.tpChance * 100 + "% Roll: " + roll*100);
            if(roll < combatant.tpChance) {
                combatant.tpChance = combatant.tpChanceBase;
                combatant.tp++;
                tpSpriteRenderers[combatantId].gameObject.SetActive(true);
            } else {
                combatant.tpChance = Mathf.Clamp(combatant.tpChance + .15f, 0f, 1f);
            }
        }

        if (combatant.tp == 1) {
            tpSpriteRenderers[combatantId].sprite = tp1Sprite;
        } else if (combatant.tp == 2) {
            tpSpriteRenderers[combatantId].sprite = tp2Sprite;
        } else if (combatant.tp == 3) {
            tpSpriteRenderers[combatantId].sprite = tp3Sprite;
        } else if (combatant.tp == 0) {
            tpSpriteRenderers[combatantId].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(turnDelay);
        CalculateSpeedTicks(combatant, ability.delay);
        GameMenu.instance.heroStatPanels[combatantId].currentTurnCursor.gameObject.SetActive(false);
        currentTurnCursors[combatantId].SetActive(false);

        if (combatant.isCharging) {
            combatant.isCharging = false;
            enemyTargetCursors[combatant.targetId - partySizeOffset].SetActive(false);
        }

        NextTurn();
    }

    private void ToggleEnemyButtons(bool value) {
        foreach(var panel in enemyStatWindows) {
            panel.GetComponent<Button>().interactable = value;
        }
    }

    public void SetTargetedEnemyId(int id) {
        if (combatants[id].isDead) return;

        enemyTargetCursors[targetId - partySizeOffset].SetActive(false);
        enemyStatWindows[targetId - partySizeOffset].targetCursor.gameObject.SetActive(false);
        enemyTargetCursors[id - partySizeOffset].SetActive(true);
        enemyStatWindows[id - partySizeOffset].targetCursor.gameObject.SetActive(true);
        targetId = id;
        target = combatants[targetId];
    }

    public void ClearEnemyTarget() {
        enemyTargetCursors[targetId - partySizeOffset].SetActive(false);
        enemyStatWindows[targetId - partySizeOffset].targetCursor.gameObject.SetActive(false);
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
        targetId = 4;
        SetTargetedEnemyId(4);
    }

    public void OpenMagicMenu() {
        actionTooltip.gameObject.SetActive(false);
        targetMenu.SetActive(false);

        // magicMenu.SetActive(true);
        battleMenuDisabled.SetActive(true);

        var magicNames = combatant.abilities;

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
        GameMenu.instance.heroStatPanels[combatantId].currentTurnCursor.gameObject.SetActive(false);
        currentTurnCursors[combatantId].SetActive(false);battleActive = false;
        ClearAllCursors();
        ClearEnemyTarget();
        enemyWindows.SetActive(false);
        battleMenu.SetActive(false);
        AudioManager.instance.PlayBgm("victory");

        yield return new WaitForSeconds(1.25f);

        UIFade.instance.FadeOut();

        yield return new WaitForSeconds(.5f);

        UIFade.instance.Collapse();
        battleScene.SetActive(false);
        canvas.gameObject.SetActive(false);

        PlayerController.instance.GetComponent<SpriteRenderer>().enabled = true;
        foreach(var member in PlayerController.instance.partyMembers) {
            member.GetComponent<SpriteRenderer>().enabled = true;
        }

        yield return new WaitForSeconds(1.25f);

        for (var i = 0; i < combatants.Count; i++) {
            if (combatants[i].isPlayer) {
                for (var j = 0; j < GameManager.instance.heroes.Length; j++) {
                    if (combatants[i].id == GameManager.instance.heroes[j].id) {
                        GameManager.instance.heroes[j].hp.current = combatants[i].hp.current;
                        GameManager.instance.heroes[j].mp.current = combatants[i].mp.current;
                        GameManager.instance.heroes[j].deflect = 0;
                        GameManager.instance.heroes[j].barrier = 0;
                        break;
                    }
                }
            }

            Destroy(combatants[i].gameObject);
        }

        
        combatants.Clear();
        combatantId = 0;
        
        if (fleeing) {
            fleeing = false;
            GameManager.instance.battleActive = false;
        } else {
            BattleReward.instance.OpenWindow(xpEarned);
        }

        AudioManager.instance.PlayBgm(FindObjectOfType<CameraController>().musicNameToPlay);
    }

    public IEnumerator<WaitForSeconds> GameOverCoroutine() {
        GameMenu.instance.heroStatPanels[combatantId].currentTurnCursor.gameObject.SetActive(false);
        currentTurnCursors[combatantId].SetActive(false);battleActive = false;
        ClearAllCursors();
        ClearEnemyTarget();
        battleNotification.text.text = "Game Over!";
        battleNotification.Activate();
        battleActive = false;
        yield return new WaitForSeconds(2f);
        UIFade.instance.Expand();
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

    public Ability GetBattleAction (string name) {
        for (var i = 0; i < battleActions.Length; i++) {
            if (battleActions[i].name == name) {
                return battleActions[i];
            }
        }
        Debug.LogError("Couldn't find battle action '" + name + "'.");
        return null;
    }

    public void ShowTooltip(string description) {
        actionTooltip.description.text = description;
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
                    player.hp.baseMax = 999;
                    player.mp.current = 999;
                    player.mp.baseMax = 999;
                    player.attack = 5000;
                    player.defense = 500;
                }
            }
            manager.CheckForBattleEnd();
        }
        if (GUILayout.Button("Next turn.")) {
            manager.NextTurn();
        }
    }
}

