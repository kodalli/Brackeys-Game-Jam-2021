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
    public MonsterScriptableObject playerMonsterData, enemyMonsterData;
    public Transform playerBattleLocation, enemyBattleLocation;

    private void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {
        GameObject playerObj = Instantiate(playerMonsterData.Prefab);
        playerObj.transform.position = playerBattleLocation.position;

        //playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyObj = Instantiate(enemyMonsterData.Prefab);
        enemyObj.transform.position = enemyBattleLocation.position;

        // enemyUnit = enemyGO.GetComponent<Unit>();

        // dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        // playerHUD.SetHUD(playerUnit);
        // enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    #region Player Coroutines
    IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(0f);
    }

    IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(0f);
    }
    #endregion

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
