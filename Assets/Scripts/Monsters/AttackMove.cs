using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Logic of Move will be in Monster class
public enum MoveType { DamageOpponent, HealUser, CauseStatusOpponent, CauseStatusUser, None }

[CreateAssetMenu(fileName = "MonsterMoveData", menuName = "ScriptableObjects/MonsterMoveDataScriptableObject", order = 1)]
public class AttackMove: ScriptableObject
{
    public string MoveName;
    public Status StatusEffect = Status.Neutral;
    public MoveType MonsterMoveType = MoveType.None;
    public MoveType SecondaryMonsterMoveType = MoveType.None;
    [Range(0, 20)] public int MPCost;
    [Range(0, 20)] public int MaxMPForMove;
    public string Description;
    
    /*
        Types of Moves
        1. Cause damage to enemy
        2. Change def or atk stats of enemy
        3. Change def or atk stats of player
        4. Cause status on enemy
        5. Cause status on player
        6. Heal player
     */
}
