using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : EnemyState {
    protected SO_PlayerDetected stateData;

    protected bool isPlayerInMinAgroRange;
    protected bool isPlayerInMaxAgroRange;
    protected bool performLongRangeAction;
    protected bool performCloseRangeAction;

    public PlayerDetectedState(Entity entity, EnemyStateMachine stateMachine, string animBoolName, SO_PlayerDetected stateData) : base(entity, stateMachine, animBoolName) {
        this.stateData = stateData;
    }
    public override void DoChecks() {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();

        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
    }
    public override void Enter() {
        base.Enter();

        performLongRangeAction = false;
        entity.SetVelocity(0f);
    }
    public override void Exit() {
        base.Exit();
    }
    public override void LogicUpdate() {
        base.LogicUpdate();
        if (Time.time >= startTime + stateData.longRangeActionTime) {
            performLongRangeAction = true;
        }
    }
    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
