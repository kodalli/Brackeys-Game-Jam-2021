using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [SerializeField] private BattleNPC enemyNPC;
    [SerializeField] private Transform playerBattleLocation, enemyBattleLocation;
    [SerializeField] private BattleHUD playerHUD, enemyHUD;
    [SerializeField] private GameObject combatOptionsPanel;
    [SerializeField] private GameObject atkButton2, atkButton3, fuseButton, separateButton;

    [SerializeField] [Range(0f, 1f)] private float runSuccessProbabity = 0.7f;
    [SerializeField] private string sceneName = "Starting Village";

    private Monster playerUnit, enemyUnit;
    private GameObject playerObj, enemyObj;
    private int playerSquadCount, enemySquadCount;
    #endregion

    private void Start() {
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
        //dialogueText.text = "A wild " + enemyUnit.Name + " approaches...";
        StartCoroutine(FancyText("A wild " + enemyUnit.Name + " approaches..."));

        yield return new WaitForSeconds(2f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }
    #endregion

    #region Player Coroutines
    IEnumerator PlayerAttack(AttackMove move) {
        // Do attack, probabilty to miss or not effective
        // Apply status
        // Check if enemy dead, if dead go to next enemy else end battle

        // Health bar damaged animation
        combatOptionsPanel.SetActive(false);

        //dialogueText.text = playerUnit.Name + " used " + move.MoveName;
        var text = playerUnit.Name + " used " + move.MoveName;
        StartCoroutine(FancyText(text));

        yield return new WaitForSeconds(1.5f);

        // handle status effect
        if (move.StatusEffect != Status.Neutral) {
            if (move.MonsterMoveType == MoveType.CauseStatusOpponent || move.SecondaryMonsterMoveType == MoveType.CauseStatusOpponent) {
                enemyUnit.CurrentStatus = move.StatusEffect;
                //dialogueText.text = enemyUnit.Name + " is " + move.StatusEffect;
                text = enemyUnit.Name + " is " + move.StatusEffect;
                StartCoroutine(FancyText(text));
                yield return new WaitForSeconds(1.5f);
            }
            if (move.MonsterMoveType == MoveType.CauseStatusUser || move.SecondaryMonsterMoveType == MoveType.CauseStatusUser) {
                playerUnit.CurrentStatus = move.StatusEffect;
                //dialogueText.text = playerUnit.Name + " is " + move.StatusEffect;
                text = playerUnit.Name + " is " + move.StatusEffect;
                StartCoroutine(FancyText(text));
                yield return new WaitForSeconds(1.5f);
            }
        }

        // do damage to enemy
        if (move.MonsterMoveType == MoveType.DamageOpponent || move.SecondaryMonsterMoveType == MoveType.DamageOpponent) {
            var previousHP = enemyUnit.CurHP;
            enemyUnit.TakeDamage(playerUnit.CurAtk);
            var currentHP = enemyUnit.CurHP;

            while (previousHP > currentHP) {
                previousHP--;
                enemyHUD.SetHP(previousHP);
                yield return new WaitForEndOfFrame();
            }
        }

        var isDead = enemyUnit.CurrentStatus.Equals(Status.Fainted);

        //dialogueText.text = "The attack is successful!";
        text = "The attack is successful!";
        StartCoroutine(FancyText(text));

        yield return new WaitForSeconds(1.5f);

        if (isDead) {
            StartCoroutine(DrawNextEnemyMonster());
        }
        else {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerHeal() {
        yield return new WaitForSeconds(0f);
    }

    IEnumerator DrawNextPlayerMonster() {
        // Called when this current monster dies
        // Do some animation for drawing another guy
        // swap out prefab
        // save unit data
        // swap playerUnit to new unit
        // if no more units then player lost and end battle


        Destroy(playerObj);
        var previousMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];
        PlayerControlSave.Instance.localPlayerData.monstersDict[previousMonster.Name] = playerUnit;

        //dialogueText.text = playerUnit.Name + " has been defeated!";
        StartCoroutine(FancyText(playerUnit.Name + " has been defeated!"));

        yield return new WaitForSeconds(1f);

        // save enemy state?

        playerSquadCount++;

        if (playerSquadCount < PlayerControlSave.Instance.localPlayerData.squad.Count) {
            BringPlayerMonsterIn();

            //dialogueText.text = playerUnit.Name + " enters the fight!";
            StartCoroutine(FancyText(playerUnit.Name + " enters the fight!"));

            yield return new WaitForSeconds(1f);

            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
        else {
            state = BattleState.Lost;
            EndBattle();
        }

    }

    IEnumerator DrawPlayerMonster(int index) {
        if (index < PlayerControlSave.Instance.localPlayerData.squad.Count) {
            playerSquadCount = index;
            BringPlayerMonsterIn();
            //dialogueText.text = playerUnit.Name + " enters the fight!";
            StartCoroutine(FancyText(playerUnit.Name + " enters the fight!"));
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region Enemy Coroutines
    IEnumerator EnemyTurn(bool? runAttemptFail = false) {
        // Do attack
        // Apply status
        // Check if player dead, if dead go to next monster else end battle
        combatOptionsPanel.SetActive(false);

        if (runAttemptFail != null && runAttemptFail == true) {
            //dialogueText.text = "Run attempt failed!";
            StartCoroutine(FancyText("Run attempt failed!"));
            yield return new WaitForSeconds(1f);
        }

        //dialogueText.text = enemyUnit.Name + " attacks!";
        StartCoroutine(FancyText(enemyUnit.Name + " attacks!"));

        yield return new WaitForSeconds(1f);

        // Health bar damaged animation
        var previousHP = playerUnit.CurHP;
        playerUnit.TakeDamage(enemyUnit.CurAtk);
        var currentHP = playerUnit.CurHP;
        while (previousHP > currentHP) {
            previousHP--;
            playerHUD.SetHP(previousHP);
            yield return new WaitForEndOfFrame();
        }

        var isDead = playerUnit.CurrentStatus.Equals(Status.Fainted);

        yield return new WaitForSeconds(1f);

        if (isDead) {
            // Go to next monster else lost
            StartCoroutine(DrawNextPlayerMonster());
        }
        else {
            state = BattleState.PlayerTurn;
            PlayerTurn();
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

        //dialogueText.text = enemyUnit.Name + " has been defeated!";
        StartCoroutine(FancyText(enemyUnit.Name + " has been defeated!"));

        yield return new WaitForSeconds(1f);

        // save player state?

        enemySquadCount++;

        if (enemySquadCount < enemyNPC.squad.Count) {
            BringEnemyMonsterIn();

            //dialogueText.text = enemyUnit.Name + " enters the fight!";
            StartCoroutine(FancyText(enemyUnit.Name + " enters the fight!"));

            yield return new WaitForSeconds(1f);

            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
        else {
            state = BattleState.Won;
            EndBattle();
        }
    }

    IEnumerator DrawEnemyMonsterIn(int index) {
        if (index < enemyNPC.squad.Count) {
            enemySquadCount = index;
            BringEnemyMonsterIn();
            //dialogueText.text = enemyUnit.Name + " enters the fight!";
            StartCoroutine(FancyText(enemyUnit.Name + " enters the fight!"));
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Bring Monsters In
    void BringPlayerMonsterIn() {
        var currentMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];

        playerUnit = PlayerControlSave.Instance.localPlayerData.monstersDict[currentMonster.Name];

        // check if fainted, cycle through squad, if no one then end battle
        if (playerUnit.CurrentStatus == Status.Fainted) {
            for (int i = 0; i < PlayerControlSave.Instance.localPlayerData.squad.Count; i++) {
                playerSquadCount = i;
                currentMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];
                playerUnit = PlayerControlSave.Instance.localPlayerData.monstersDict[currentMonster.Name];
                //Debug.Log(playerUnit.Name + ", " + playerUnit.CurrentStatus);
                if (playerUnit.CurrentStatus != Status.Fainted) {
                    break;
                }
            }
            if (playerUnit.CurrentStatus == Status.Fainted) {
                state = BattleState.Lost;
                EndBattle();
            }
        }

        Destroy(playerObj);

        playerObj = Instantiate(currentMonster.Prefab);
        playerObj.transform.position = playerBattleLocation.position;
        playerObj.GetComponent<SpriteRenderer>().flipX = true;

        playerHUD.SetHUD(playerUnit);

        ApplyUnfusedButtonText();
    }

    void BringFusedPlayerMonsterIn(MonsterScriptableObject fusedMonster) {
        var hpDifference = GetHpDifference();
        var level = playerUnit.GetLevel();
        var currentMonster = fusedMonster;

        playerUnit = new Monster(currentMonster);
        playerUnit.LevelUp(level);
        playerUnit.AddHP(hpDifference);

        Destroy(playerObj);

        playerObj = Instantiate(currentMonster.Prefab);
        playerObj.transform.position = playerBattleLocation.position;
        playerObj.GetComponent<SpriteRenderer>().flipX = true;

        playerHUD.SetHUD(playerUnit);

        ApplyFusedButtonText();
    }

    void BringEnemyMonsterIn() {
        var currentMonster = enemyNPC.squad[enemySquadCount];

        enemyUnit = enemyNPC.monstersDict[currentMonster.Name];
        enemyObj = Instantiate(currentMonster.Prefab);
        enemyObj.transform.position = enemyBattleLocation.position;

        enemyHUD.SetHUD(enemyUnit);
    }
    #endregion

    #region End Battle
    void EndBattle() {
        combatOptionsPanel.SetActive(false);
        // apply xp winnings at end of battle
        if (state == BattleState.Won) {
            //dialogueText.text = "You won the battle!";
            StartCoroutine(FancyText("You won the battle!"));
            enemyNPC.State = NPCStatus.Defeated;
            PlayerControlSave.Instance.localPlayerData.enemyData.State = enemyNPC.State;
            PlayerControlSave.Instance.localPlayerData.defeatedEnemyNames.Add(enemyNPC.Name);
        }
        else if (state == BattleState.Lost) {
            //dialogueText.text = "You were defeated.";
            StartCoroutine(FancyText("You were defeated."));
            enemyNPC.State = NPCStatus.Standby;
        }
        else if (state == BattleState.Run) {
            //dialogueText.text = "Escape successful!";
            StartCoroutine(FancyText("Escape successful!"));
            enemyNPC.State = NPCStatus.Standby;
        }

        // Change Scene
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene() {
        PlayerControlSave.Instance.SaveData();
        //PlayerControlSave.Instance.DisplayMonstersDict();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
    #endregion

    #region Helpers

    void PlayerTurn() {
        //dialogueText.text = "What will " + playerUnit.Name + " do?";
        StartCoroutine(FancyText("What will " + playerUnit.Name + " do?"));
        combatOptionsPanel.SetActive(true);
    }


    void ApplyXPEarnings() {

    }

    int GetHpDifference() {
        return playerUnit.CurHP - playerUnit.GetMaxHP();
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

    IEnumerator FancyText(string text) {
        for (int i = 0; i < text.Length + 1; i++) {
            dialogueText.text = text.Substring(0, i);
            yield return new WaitForEndOfFrame();
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

    public void OnAttackButton() {
        //if (state != BattleState.PlayerTurn)
        //    return;

        //dialogueText.text = "";
        ////StartCoroutine(PlayerAttack());
        //state = BattleState.EnemyTurn;
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

        BringFusedPlayerMonsterIn(item.FuseMonsterWithItem(playerUnit));
    }

    public void OnSeparateButton() {
        var hpDifference = GetHpDifference();

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
            Debug.Log("current hp " + playerUnit.CurHP);

        }
    }

    public void OnItemsButton() {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerHeal());
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

    public void OnSquadButton() {
        if (state != BattleState.PlayerTurn)
            return;

        var dict = PlayerControlSave.Instance.localPlayerData.monstersDict;
        foreach (KeyValuePair<string, Monster> monster in dict) {
            Debug.Log(monster.Key + ", status " + monster.Value.CurrentStatus + ", hp " + monster.Value.CurHP);
        }
    }
    #endregion
}
