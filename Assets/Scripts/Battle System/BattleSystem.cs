using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Begin,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    private void Start()
    {
        
    }


    IEnumerator SetupBattle()
    {
        yield return new WaitForSeconds(0f);
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
