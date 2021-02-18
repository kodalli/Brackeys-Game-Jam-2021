﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost,
}

public class BattleSystem : MonoBehaviour
{
    #region Variables
    public BattleState state;
    public TextMeshProUGUI dialogueText;

    [SerializeField] private BattleNPC enemyNPC;
    [SerializeField] private Transform playerBattleLocation, enemyBattleLocation;
    [SerializeField] private BattleHUD playerHUD, enemyHUD;

    private Monster playerUnit, enemyUnit;
    private GameObject playerObj, enemyObj;
    private int playerSquadCount, enemySquadCount;
    #endregion

    private void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }

    #region Battle Setup
    IEnumerator SetupBattle()
    {
        playerSquadCount = enemySquadCount = 0;

        // Player setup
        var firstMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];
        playerUnit = PlayerControlSave.Instance.localPlayerData.monstersDict[firstMonster.Name];

        playerObj = Instantiate(firstMonster.Prefab);
        playerObj.transform.position = playerBattleLocation.position;


        // Enemy setup
        var firstEnemyMonster = enemyNPC.squad[enemySquadCount];
        enemyUnit = enemyNPC.monstersDict[firstEnemyMonster.Name];

        enemyObj = Instantiate(firstEnemyMonster.Prefab);
        enemyObj.transform.position = enemyBattleLocation.position;

        enemyNPC.State = NPCStatus.Fighting;

        // Update UI and HUD
        dialogueText.text = "A wild " + enemyUnit.Name + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2.5f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }
    #endregion

    #region Player Coroutines
    IEnumerator PlayerAttack()
    {
        // Do attack, probabilty to miss or not effective
        // Apply status
        // Check if enemy dead, if dead go to next enemy else end battle

        enemyUnit.TakeDamage(playerUnit.GetCurrentAtk());

        var isDead = enemyUnit.GetStatus().Equals(Status.Fainted);

        enemyHUD.SetHP(enemyUnit.GetCurrentHP());
        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            StartCoroutine(DrawNextEnemyMonster());
        }
        else
        {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(0f);
    }

    IEnumerator DrawNextPlayerMonster()
    {
        // Called when this current monster dies
        // Do some animation for drawing another guy
        // swap out prefab
        // save unit data
        // swap playerUnit to new unit
        // if no more units then player lost and end battle
        Destroy(playerObj);
        var previousMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];
        PlayerControlSave.Instance.localPlayerData.monstersDict[previousMonster.Name] = playerUnit;

        dialogueText.text = playerUnit.Name + " has been defeated!";

        yield return new WaitForSeconds(1f);

        // save enemy state?

        playerSquadCount++;

        if (playerSquadCount < PlayerControlSave.Instance.localPlayerData.squad.Count)
        {
            BringPlayerMonsterIn();

            dialogueText.text = playerUnit.Name + " enters the fight!";

            yield return new WaitForSeconds(1f);

            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
        else
        {
            state = BattleState.Lost;
            EndBattle();
        }

    }

    IEnumerator DrawPlayerMonster(int index)
    {
        if (index < PlayerControlSave.Instance.localPlayerData.squad.Count)
        {
            playerSquadCount = index;
            BringPlayerMonsterIn();
            dialogueText.text = playerUnit.Name + " enters the fight!";
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region Enemy Coroutines
    IEnumerator EnemyTurn()
    {
        // Do attack
        // Apply status
        // Check if player dead, if dead go to next monster else end battle

        dialogueText.text = enemyUnit.Name + " attacks!";

        yield return new WaitForSeconds(1f);

        playerUnit.TakeDamage(enemyUnit.GetCurrentAtk());

        playerHUD.SetHP(playerUnit.GetCurrentHP());

        var isDead = playerUnit.GetStatus().Equals(Status.Fainted);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            // Go to next monster else lost
            StartCoroutine(DrawNextPlayerMonster());
        }
        else
        {
            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
    }

    IEnumerator DrawNextEnemyMonster()
    {
        // Called when current monster dies
        // Do some animation for drawing another guy
        // swap out prefab
        // save unit data
        // swap enemyUnit to new unit
        // if no more units then enemy lost and end battle
        Destroy(enemyObj);
        var previousMonster = enemyNPC.squad[enemySquadCount];
        enemyNPC.monstersDict[previousMonster.Name] = enemyUnit;

        dialogueText.text = enemyUnit.Name + " has been defeated!";

        yield return new WaitForSeconds(1f);

        // save player state?

        enemySquadCount++;

        if (enemySquadCount < enemyNPC.squad.Count)
        {
            BringEnemyMonsterIn();

            dialogueText.text = enemyUnit.Name + " enters the fight!";

            yield return new WaitForSeconds(1f);

            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = BattleState.Won;
            EndBattle();
        }
    }

    IEnumerator DrawEnemyMonsterIn(int index)
    {
        if (index < enemyNPC.squad.Count)
        {
            enemySquadCount = index;
            BringEnemyMonsterIn();
            dialogueText.text = enemyUnit.Name + " enters the fight!";
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Helpers
    void EndBattle()
    {
        // apply xp winnings at end of battle
        if (state == BattleState.Won)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.Lost)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "What will " + playerUnit.Name + " do?";
    }

    void BringPlayerMonsterIn()
    {
        var currentMonster = PlayerControlSave.Instance.localPlayerData.squad[playerSquadCount];
        playerUnit = PlayerControlSave.Instance.localPlayerData.monstersDict[currentMonster.Name];
        playerObj = Instantiate(currentMonster.Prefab);
        playerObj.transform.position = playerBattleLocation.position;
        playerHUD.SetHUD(playerUnit);
    }

    void BringEnemyMonsterIn()
    {
        var currentMonster = enemyNPC.squad[enemySquadCount];
        enemyUnit = enemyNPC.monstersDict[currentMonster.Name];
        enemyObj = Instantiate(currentMonster.Prefab);
        enemyObj.transform.position = enemyBattleLocation.position;
        enemyHUD.SetHUD(enemyUnit);
    }

    void ApplyXPEarnings()
    {

    }
    #endregion

    #region Buttons
    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnItemsButton()
    {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerHeal());
    }

    public void OnRunButton()
    {
        if (state != BattleState.PlayerTurn)
            return;
    }

    public void OnSquadButton()
    {
        if (state != BattleState.PlayerTurn)
            return;
    }
    #endregion
}