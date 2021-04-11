using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeState : EnemyState {
    protected D_ChargeState stateData;

    protected bool isPlayerInMinAgroRange;
    protected bool isDetectingLedge;
    protected bool isDetectingWall;
    protected bool isChargeTimeOver;

    public ChargeState(Entity entity, EnemyStateMachine stateMachine, string animBoolName, D_ChargeState stateData) : base(entity, stateMachine, animBoolName) {
        this.stateData = stateData;
    }
    public override void DoChecks() {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        isDetectingLedge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();
    }
    public override void Enter() {
        base.Enter();

        isChargeTimeOver = false;
        entity.SetVelocity(stateData.chargeSpeed);
    }
    public override void Exit() {
        base.Exit();
    }
    public override void LogicUpdate() {
        base.LogicUpdate();
        if(Time.time >= startTime + stateData.chargeTime){
            isChargeTimeOver = true;
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

}
