using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject playerPrefab, enemyPrefab;
    public Transform playerBattleLocation, enemyBattleLocation;

    private void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleLocation);
        //playerUnit = playerGO.GetComponent<Unit>();
        // Get player data

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleLocation);
        // enemyUnit = enemyGO.GetComponent<Unit>();
        // Get enemy data

        // dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        // playerHUD.SetHUD(playerUnit);
        // enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(0f);
    }

    IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(0f);
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0f);
    }

    void EndBattle()
    {

    }

    void PlayerTurn()
    {

    }

    #region Buttons
    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerHeal());
    }
    #endregion
}
