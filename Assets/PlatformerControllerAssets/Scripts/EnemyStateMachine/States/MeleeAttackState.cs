using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState {

    protected SO_MeleeAttackState stateData;

    public MeleeAttackState(Entity entity, EnemyStateMachine stateMachine, string animBoolName, Transform attackPosition, SO_MeleeAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition) {
        this.stateData = stateData;
    }

    public override void FinishAttack() {
        base.FinishAttack();
    }
    public override void TriggerAttack() {
        base.TriggerAttack();

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, stateData.attackRadius, stateData.whatIsPlayer);

        foreach (Collider2D collider in detectedObjects) {
            // collider.transform.SendMessage("Damage", attackDetails);
            PlayerX.Instance.Hit1State.HitSide(entity.AliveGO.transform.position.x > PlayerX.Instance.transform.position.x);
            PlayerX.Instance.Hit1State.SetHitForce(stateData.hitForceX, stateData.hitForceY);
            PlayerX.Instance.Hit1State.enemyDamage = stateData.attackDamage;
            PlayerX.Instance.StateMachine.ChangeState(PlayerX.Instance.Hit1State);
        }
    }
}
