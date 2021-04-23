using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy1 : Entity, IDamageable {
    [SerializeField] private int health;
    [SerializeField] private string enemyName;

    public string EnemyName { get { return enemyName; } }

    public event Action<Enemy1> enemyDelegate;

    public E1_IdleState idleState { get; private set; }
    public E1_MoveState moveState { get; private set; }
    public E1_PlayerDetectedState playerDetectedState { get; private set; }
    public E1_ChargeState chargeState { get; private set; }
    public E1_LookForPlayerState lookForPlayerState { get; private set; }
    public E1_MeleeAttckState meleeAttackState { get; private set; }

    [SerializeField] private SO_IdleState idleStateData;
    [SerializeField] private SO_MoveState moveStateData;
    [SerializeField] private SO_PlayerDetected playerDetectedData;
    [SerializeField] private SO_ChargeState chargeStateData;
    [SerializeField] private SO_LookForPlayer lookForPlayerStateData;
    [SerializeField] private SO_MeleeAttackState meleeAttackStateData;

    [SerializeField] private Transform meleeAttackPosition;

    public override void Start() {
        base.Start();

        health = 100;

        moveState = new E1_MoveState(this, StateMachine, "move", moveStateData, this);
        idleState = new E1_IdleState(this, StateMachine, "idle", idleStateData, this);
        playerDetectedState = new E1_PlayerDetectedState(this, StateMachine, "playerDetected", playerDetectedData, this);
        chargeState = new E1_ChargeState(this, StateMachine, "charge", chargeStateData, this);
        lookForPlayerState = new E1_LookForPlayerState(this, StateMachine, "lookForPlayer", lookForPlayerStateData, this);
        meleeAttackState = new E1_MeleeAttckState(this, StateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);

        StateMachine.Initialize(idleState);
    }
    public void TakeDamage(float damage) {

        health -= (int)damage;

        if (health <= 0) {
            Destroy(this.gameObject);
        }
        Debug.Log(health);

        if (enemyDelegate != null) enemyDelegate(this);
    }

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
