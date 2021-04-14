using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_MeleeAttckState : MeleeAttackState {
    private Enemy1 enemy;
    public E1_MeleeAttckState(Entity entity, EnemyStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState stateData, Enemy1 enemy) : base(entity, stateMachine, animBoolName, attackPosition, stateData) {
        this.enemy = enemy;
    }
    public override void LogicUpdate() {
        base.LogicUpdate();

        if (isAnimationFinished) {
            if (isPlayerInMinAgroRange) {
                stateMachine.ChangeState(enemy.playerDetectedState);
            } else {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }
}
