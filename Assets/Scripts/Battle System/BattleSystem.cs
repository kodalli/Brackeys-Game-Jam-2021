﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public enum BattleState {
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost,
    Run,
}

public class BattleSystem : MonoBehaviour {
    #region Variables
    public BattleState state;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI AttackButtonFusedText1, AttackButtonFusedText2, AttackButtonFusedText3;
    [SerializeField] private TextMeshProUGUI atkToolTip;

    [SerializeField] private BattleNPC enemyNPC;
    [SerializeField] private Transform playerBattleLocation, enemyBattleLocation;
    [SerializeField] private BattleHUD playerHUD, enemyHUD;
    [SerializeField] private GameObject combatOptionsPanel;
    [SerializeField] private GameObject atkButton2, atkButton3, fuseButton, separateButton;

    [SerializeField] [Range(0f, 1f)] private float runSuccessProbabity = 0.7f;
    [SerializeField] private string sceneName = "Starting Village";
    [SerializeField] private float xpMultiplier = 0.2f;

    private Monster playerUnit, enemyUnit;
    private GameObject playerObj, enemyObj;
    private int playerSquadCount, enemySquadCount;
    #endregion

    private void Start() {
        if (PlayerControlSave.Instance.localPlayerData.enemyData != null)
            enemyNPC.Unwrap(PlayerControlSave.Instance.localPlayerData.enemyData);
        state = BattleState.Start;
        combatOptionsPanel.SetActive(false);
        StartCoroutine(SetupBattle());
    }

    #region Battle Setup
    IEnumerator SetupBattle() {
        playerSquadCount = enemySquadCount = 0;

        // Player setup
        BringPlayerMonsterIn();

        // Enemy setup
        BringEnemyMonsterIn();

        // if no usable player units then end the battle
        if (playerUnit.CurrentStatus == Status.Fainted)
            yield break;

        enemyNPC.State = NPCStatus.Fighting;

        // Update UI and HUD
        yield return StartCoroutine(FancyText($"{enemyNPC.Name} chooses {enemyUnit.Name} for battle...", 2f));

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }
    #endregion

    #region Player Coroutines
    IEnumerator PlayerAttack(AttackMove move) {

        // disable combat menu
        combatOptionsPanel.SetActive(false);

        yield return StartCoroutine(FancyText($"{playerUnit.Name} used {move.MoveName}", 1.5f));

        // Do status effect
        yield return StartCoroutine(ApplyStatusEffect(move, playerUnit, enemyUnit));

        // Do damage
        yield return StartCoroutine(ApplyDamage(move, playerUnit, enemyUnit, enemyHUD));

        // Do heal
        yield return StartCoroutine(ApplyHeal(move, playerUnit, playerHUD));

        yield return StartCoroutine(FancyText("The attack is successful!", 1.5f));

        var isDead = enemyUnit.CurrentStatus.Equals(Status.Fainted);

        // if cur monster dead go next -> enemy turn, else go -> enemy turn
        if (isDead) {
            StartCoroutine(DrawNextEnemyMonster());
        }
        else {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator DrawNextPlayerMonster() {
        // Called when this current monster dies
        // Do some animation for drawing another guy
        // swap out prefab
        // save unit data
        // swap playerUnit to new unit
        // if no more units then player lost and end battle

        Destroy(playerObj);

        yield return StartCoroutine(FancyText($"{playerUnit.Name} has been defeated!", 1f));

        playerSquadCount++;

        if (playerSquadCount < PlayerControlSave.Instance.localPlayerData.squad.Count) {
            BringPlayerMonsterIn();

            yield return StartCoroutine(FancyText($"{playerUnit.Name} enters the fight!", 1f));

            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
        else {
            state = BattleState.Lost;
            EndBattle();
        }

    }

    IEnumerator DrawPlayerMonster(int index) {
        playerSquadCount = index;
        BringPlayerMonsterIn();
        yield return StartCoroutine(FancyText($"{playerUnit.Name} enters the fight!", 1f));
        StartCoroutine(FancyText($"What will {playerUnit.Name} do?", 0f));
    }

    #endregion

    #region Enemy Coroutines
    IEnumerator EnemyTurn(bool? runAttemptFail = false) {
        // Do attack
        // Apply status
        // Check if player dead, if dead go to next monster else end battle
        combatOptionsPanel.SetActive(false);

        if (runAttemptFail != null && runAttemptFail == true) {
            yield return StartCoroutine(FancyText("Run attempt failed!", 1f));
        }

        // Check if turn possible based on status, if not end turn

        var canMove = CheckIfCanMove(enemyUnit.CurrentStatus);

        if(!canMove) {
            yield return StartCoroutine(FancyText($"{enemyUnit.Name} is {enemyUnit.CurrentStatus} and can't move...", 1.5f));
            enemyUnit.CurrentStatus = Status.Neutral;
            state = BattleState.PlayerTurn;
            PlayerTurn();
            yield break;
        }

        yield return StartCoroutine(FancyText($"{enemyUnit.Name} attacks!", 1.5f));

        // fuse unit if possible
        if (!enemyUnit.Fused && enemyNPC.items.Count > 0) {
            var fusedMonsterSO = GetFusedMonsterFromRandomItem();
            BringFusedEnemyMonsterIn(fusedMonsterSO);
            yield return StartCoroutine(FancyText($"{enemyNPC.Name} fused to make {enemyUnit.Name}", 1.5f));
        }

        // Select random move
        var move = enemyUnit.MoveSet[Random.Range(0, enemyUnit.MoveSet.Count)];

        yield return StartCoroutine(FancyText($"{enemyUnit.Name} uses {move.MoveName}", 1.5f));

        // Do status effect
        yield return StartCoroutine(ApplyStatusEffect(move, enemyUnit, playerUnit));

        // Do damage 
        yield return StartCoroutine(ApplyDamage(move, enemyUnit, playerUnit, playerHUD));

        // Do heal
        yield return StartCoroutine(ApplyHeal(move, enemyUnit, enemyHUD));

        yield return new WaitForSeconds(1f);

        var isDead = playerUnit.CurrentStatus.Equals(Status.Fainted);

        if (isDead) {
            // Go to next monster else lost
            yield return StartCoroutine(FancyText($"{playerUnit} fainted!", 1f));
            StartCoroutine(DrawNextPlayerMonster());
        }
        else {
            // Check if turn possible based on status, if not end turn

            canMove = CheckIfCanMove(playerUnit.CurrentStatus);

            if (!canMove) {
                yield return StartCoroutine(FancyText($"{playerUnit.Name} is {playerUnit.CurrentStatus} and can't move...", 1.5f));
                playerUnit.CurrentStatus = Status.Neutral;
                state = BattleState.EnemyTurn;
                StartCoroutine(EnemyTurn());
            } else {
                state = BattleState.PlayerTurn;
                PlayerTurn();
            }
        }
    }

    IEnumerator DrawNextEnemyMonster() {
        // Called when current monster dies
        // Do some animation for drawing another guy
        // swap out prefab
        // save unit data
        // swap enemyUnit to new unit
        // if no more units then enemy lost and end battle

        Destroy(enemyObj);
        var previousMonster = enemyNPC.squad[enemySquadCount];
        enemyNPC.monstersDict[previousMonster.Name] = enemyUnit;

        yield return StartCoroutine(FancyText($"{enemyUnit.Name} has been defeated!", 1f));

        enemySquadCount++;

        if (enemySquadCount < enemyNPC.squad.Count) {
            BringEnemyMonsterIn();

            yield return StartCoroutine(FancyText($"{enemyUnit.Name} enters the fight!", 1f));

            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
        else {
            state = BattleState.Won;
            EndBattle();
        }
    }
    #endregion

    #region Bring Monsters In
    void BringPlayerMonsterIn() {
        if (playerSquadCount > PlayerControlSave.Instance.localPlayerData.squad.Count - 1) {
            state = BattleState.Lost;
            EndBattle();
            return;
        }

        var currentMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];

        playerUnit = PlayerControlSave.Instance.localPlayerData.monstersDict[currentMonster.Name];

        // check if fainted, cycle through squad, if no one then end battle
        if (playerUnit.CurrentStatus == Status.Fainted) {
            for (int i = 0; i < PlayerControlSave.Instance.localPlayerData.squad.Count; i++) {
                playerSquadCount = i;
                currentMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];
                playerUnit = PlayerControlSave.Instance.localPlayerData.monstersDict[currentMonster.Name];
                if (playerUnit.CurrentStatus != Status.Fainted) {
                    break;
                }
            }
            if (playerUnit.CurrentStatus == Status.Fainted) {
                state = BattleState.Lost;
                EndBattle();
            }
        }

        // destroy previous monster prefab sprite
        Destroy(playerObj);

        // create new player prefab with current monster sprite
        playerObj = Instantiate(currentMonster.Prefab);
        playerObj.transform.position = playerBattleLocation.position;
        playerObj.GetComponent<SpriteRenderer>().flipX = true;
        playerObj.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

        // update ui
        playerHUD.SetHUD(playerUnit);

        ApplyUnfusedButtonText();
    }

    void BringFusedPlayerMonsterIn(MonsterScriptableObject fusedMonster) {
        // adjust hp and level for fused monster brought in
        var hpDifference = GetHpDifference(playerUnit);
        var level = playerUnit.GetLevel();
        var currentMonster = fusedMonster;

        playerUnit = new Monster(currentMonster);
        playerUnit.LevelUp(level);
        playerUnit.AddHP(hpDifference);

        // remove old monster
        Destroy(playerObj);

        // create new fused monster sprite
        playerObj = Instantiate(currentMonster.Prefab);
        playerObj.transform.position = playerBattleLocation.position;
        playerObj.GetComponent<SpriteRenderer>().flipX = true;
        playerObj.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

        // update ui
        playerHUD.SetHUD(playerUnit);

        ApplyFusedButtonText();
    }

    // pick random item and return fused mosnter
    MonsterScriptableObject GetFusedMonsterFromRandomItem() => enemyNPC.items[Random.Range(0, enemyNPC.items.Count)].FuseMonsterWithItem(enemyUnit);

    void BringFusedEnemyMonsterIn(MonsterScriptableObject fusedMonster) {
        var hpDifference = GetHpDifference(enemyUnit);
        var level = enemyUnit.GetLevel();
        var currentMonster = fusedMonster;

        enemyUnit = new Monster(currentMonster);
        enemyUnit.LevelUp(level);
        enemyUnit.AddHP(hpDifference);

        Destroy(enemyObj);

        enemyObj = Instantiate(currentMonster.Prefab);
        enemyObj.transform.position = enemyBattleLocation.position;
        enemyObj.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

        enemyHUD.SetHUD(enemyUnit);
    }

    void BringEnemyMonsterIn() {
        var currentMonster = enemyNPC.squad[enemySquadCount];

        enemyUnit = enemyNPC.monstersDict[currentMonster.Name];
        enemyObj = Instantiate(currentMonster.Prefab);
        enemyObj.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        enemyObj.transform.position = enemyBattleLocation.position;

        enemyHUD.SetHUD(enemyUnit);
    }
    #endregion

    #region End Battle
    void EndBattle() {
        combatOptionsPanel.SetActive(false);
        // apply xp winnings at end of battle
        if (state == BattleState.Won) {
            StartCoroutine(ApplyXPEarnings());
            //StartCoroutine(FancyText("You won the battle!", 0f));
            enemyNPC.State = NPCStatus.Defeated;
            PlayerControlSave.Instance.localPlayerData.enemyData = new NPCBattleWrapper(enemyNPC); 
            PlayerControlSave.Instance.localPlayerData.defeatedEnemyNames.Add(enemyNPC.Name);
        }
        else if (state == BattleState.Lost) {
            StartCoroutine(FancyText("You were defeated.", 0f));
            enemyNPC.State = NPCStatus.Standby;
        }
        else if (state == BattleState.Run) {
            StartCoroutine(FancyText("Escape successful!", 0f));
            enemyNPC.State = NPCStatus.Standby;
        }

        // Change Scene
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene() {
        PlayerControlSave.Instance.SaveData();
        Debug.Log("change scene " + GlobalControlSave.Instance.savedPlayerData.squad.Count);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator ApplyXPEarnings() {
        var enemiesDefeated = enemyNPC.squad.Count;
        var xpEarnedPerEnemy = enemyUnit.GetXPFromLevel(enemyUnit.GetLevel());
        var amountXPEarned = Mathf.Max(Mathf.RoundToInt(xpMultiplier * xpEarnedPerEnemy * enemiesDefeated), 1);
        var squadDict = PlayerControlSave.Instance.localPlayerData.monstersDict;
        foreach (KeyValuePair<string, Monster> userMonster in squadDict) {
            Debug.Log("xp before " + PlayerControlSave.Instance.localPlayerData.monstersDict[userMonster.Key].GetCurXP());
            squadDict[userMonster.Key].AddXP(amountXPEarned);
            yield return StartCoroutine(FancyText($"{userMonster.Key} earned {amountXPEarned}XP", 1f));
            Debug.Log("xp after " + PlayerControlSave.Instance.localPlayerData.monstersDict[userMonster.Key].GetCurXP());
        }

        yield return StartCoroutine(FancyText("You won the battle!", 0f));
    }

    #endregion

    #region Battle Turn Helpers

    bool CheckIfCanMove(Status unitStatus) {
        //Neutral,
        //Stunned,
        //Enraged,
        //Depressed,
        //Traumatized,
        //High,
        //Sad,
        //Happy,
        //Horny,
        //Confused,
        //Woke,
        //Fainted,

        switch (unitStatus) {
            case Status.Stunned:
                return false;
            case Status.Confused:
                return false;
            case Status.Fainted:
                return false;
            case Status.Woke:
                return false;
            default:
                return true;
        }
    }

    IEnumerator ApplyStatusEffect(AttackMove move, Monster user, Monster opponent) {
        // handle status effect
        if (move.StatusEffect != Status.Neutral) {
            if (move.MonsterMoveType == MoveType.CauseStatusOpponent || move.SecondaryMonsterMoveType == MoveType.CauseStatusOpponent) {
                opponent.CurrentStatus = move.StatusEffect;
                yield return StartCoroutine(FancyText($"{opponent.Name} is {move.StatusEffect}", 1.5f));
                //Debug.Log(enemyUnit.CurrentStatus);
            }
            if (move.MonsterMoveType == MoveType.CauseStatusUser || move.SecondaryMonsterMoveType == MoveType.CauseStatusUser) {
                user.CurrentStatus = move.StatusEffect;
                yield return StartCoroutine(FancyText($"{user.Name} is {move.StatusEffect}", 1.5f));
                //Debug.Log(playerUnit.CurrentStatus);
            }
        }
    }

    IEnumerator ApplyDamage(AttackMove move, Monster user, Monster opponent, BattleHUD opponentHUD) {
        if (move.MonsterMoveType == MoveType.DamageOpponent || move.SecondaryMonsterMoveType == MoveType.DamageOpponent) {
            var previousHP = opponent.CurHP;
            var damageTaken = opponent.TakeDamage(user.CurAtk);
            var currentHP = opponent.CurHP;

            Debug.Log($"{user.Name} atk {user.CurAtk} dmg done {damageTaken} opponent def {opponent.CurDef}");

            while (previousHP > currentHP) {
                previousHP--;
                opponentHUD.SetHP(previousHP);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator ApplyHeal(AttackMove move, Monster user, BattleHUD userHUD) {

        if (move.MonsterMoveType == MoveType.HealUser || move.SecondaryMonsterMoveType == MoveType.HealUser) {
            var previousHP = user.CurHP;
            user.Heal();
            var currentHP = user.CurHP;

            while (previousHP < currentHP) {
                previousHP++;
                userHUD.SetHP(previousHP);
                yield return new WaitForEndOfFrame();
            }

            yield return StartCoroutine(FancyText($"{user.Name} healed...", 1f));
        }
    }
    #endregion

    #region Helpers
    void PlayerTurn() {
        StartCoroutine(FancyText($"What will {playerUnit.Name} do?", 0f));
        combatOptionsPanel.SetActive(true);
    }

    int GetHpDifference(Monster unit) {
        return unit.CurHP - unit.GetMaxHP();
    }

    void TestAttack(int index) {
        try {
            var move = playerUnit.MoveSet[index];
            DoAttack(move);
            //Debug.Log(move.MoveName);
        }
        catch {
            Debug.Log("Index out of bounds");
        }

    }

    void DoAttack(AttackMove move) {
        if (state != BattleState.PlayerTurn)
            return;

        dialogueText.text = "";
        StartCoroutine(PlayerAttack(move));
        state = BattleState.EnemyTurn;
    }

    IEnumerator FancyText(string text, float waitTime) {

        var i = 0;
        while (waitTime > 0) {
            // skip dialogue
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E)) {
                dialogueText.text = text;
                yield break;
            }
            dialogueText.text = text.Substring(0, i);
            if (i < text.Length) i++;
            waitTime -= Time.deltaTime;
            yield return default;
        }
    }


    #endregion

    #region Buttons
    void ApplyFusedButtonText() {
        var moveset = playerUnit.MoveSet;

        AttackButtonFusedText1.text = moveset[0].MoveName;
        AttackButtonFusedText2.text = moveset[1].MoveName;
        AttackButtonFusedText3.text = moveset[2].MoveName;

        fuseButton.SetActive(false);
        separateButton.SetActive(true);
        atkButton2.SetActive(true);
        atkButton3.SetActive(true);
    }

    void ApplyUnfusedButtonText() {
        var moveset = playerUnit.MoveSet;

        AttackButtonFusedText1.text = moveset[0].MoveName;

        fuseButton.SetActive(true);
        separateButton.SetActive(false);
        atkButton2.SetActive(false);
        atkButton3.SetActive(false);
    }

    public void OnFusedAttackButton1() {
        TestAttack(0);
    }

    public void OnFusedAttackButton2() {
        TestAttack(1);
    }

    public void OnFusedAttackButton3() {
        TestAttack(2);
    }


    public void OnFuseButton(ItemData item) {
        // fuse logic
        // ask player to choose item to fuse
        // if no items to fuse, fuse should be grayed out
        // once fused
        // Destroy player monster prefab
        // Bring fused monster in
        // Got to enemy turn

        if (state != BattleState.PlayerTurn)
            return;

        if (playerUnit.Fused)
            return;
        BringFusedPlayerMonsterIn(item.FuseMonsterWithItem(playerUnit));
    }

    public void OnSeparateButton() {
        var hpDifference = GetHpDifference(playerUnit);

        BringPlayerMonsterIn();

        // separated back to unfused monster
        if (hpDifference != 0) {
            // separate can't kill the unfused monster, set to 1 hp
            if (playerUnit.CurHP + hpDifference < 1) {
                playerUnit.AddHP(1 - playerUnit.CurHP);
            }
            else {
                playerUnit.AddHP(hpDifference);
            }

            playerHUD.SetHP(playerUnit.CurHP);
            //Debug.Log("current hp " + playerUnit.CurHP);

        }
    }

    public void OnRunButton() {
        if (state != BattleState.PlayerTurn)
            return;

        var runSuccess = Random.Range(0, 1) < runSuccessProbabity;

        if (runSuccess) {
            state = BattleState.Run;
            EndBattle();
        }
        else {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn(true));
        }
    }

    public void OnSquadSelectButton(string unitName) {
        // get index and draw that monster from player squad
        var squad = PlayerControlSave.Instance.localPlayerData.squad;
        var index = squad.TakeWhile(unit => unit.Name != unitName).Count();
        StartCoroutine(DrawPlayerMonster(index));
    }
    #endregion

    #region Hover ToolTips
    void TestAtkHover(int index) {
        try {
            var move = playerUnit.MoveSet[index];
            atkToolTip.text = move.Description;
            //Debug.Log(move.MoveName);
        }
        catch {
            Debug.Log("Index out of bounds");
        }

    }
    public void OnHoverAtk1() {
        TestAtkHover(0);
    }
    public void OnHoverAtk2() {
        TestAtkHover(1);
    }
    public void OnHoverAtk3() {
        TestAtkHover(2);
    }
    public void OnHoverFuse() {
        atkToolTip.text = "Fuse current homie with an item to make them stronger.";
    }
    public void OnHoverSeparate() {
        atkToolTip.text = "Separates homie to base state.";
    }

    #endregion
}
