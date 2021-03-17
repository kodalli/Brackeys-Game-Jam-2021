using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState {

    private Vector2 detectedPos;
    private Vector2 cornerPos;

    private Vector2 startPos;
    private Vector2 stopPos;

    private bool isHanging;
    private bool isClimbing;

    private int xInput;
    private int yInput;
    private bool jumpInput;

    private float stateTime = 0.0f;
    private readonly float debugTime = 5f;

    public PlayerLedgeClimbState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger() {
        base.AnimationTrigger();
        isHanging = true;
    }

    public override void Enter() {
        base.Enter();

        stateTime = 0f;

        player.SetVelocityToZero();
        player.transform.position = detectedPos;
        cornerPos = player.DetermineCornerPos();

        startPos.Set(cornerPos.x - (player.FacingDirection * playerData.startOffset.x), cornerPos.y - playerData.startOffset.y);
        stopPos.Set(cornerPos.x + (player.FacingDirection * playerData.stopOffset.x), cornerPos.y + playerData.stopOffset.y);

        player.transform.position = startPos;

    }

    public override void Exit() {
        base.Exit();

        isHanging = false;

        if (isClimbing) {
            player.transform.position = stopPos;
            isClimbing = false;
        }

    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        BugHandler();

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;

        player.SetVelocityToZero();
        player.transform.position = startPos;

        if ((xInput == player.FacingDirection && jumpInput) && isHanging && !isClimbing) {
            isClimbing = true;
            stateMachine.ChangeState(player.JumpState);
        }
        else if (yInput == -1 && isHanging && !isClimbing) {
            stateMachine.ChangeState(player.InAirState);
        }

    }

    private void BugHandler() {

        // seconds in float
        stateTime += Time.deltaTime;
        // turn seconds in float to int
        var seconds = (int)(stateTime % 60);
        if (seconds > debugTime) {
            isClimbing = true;
            stateMachine.ChangeState(player.JumpState);
        }
    }

    public void SetDetectedPos(Vector2 pos) => detectedPos = pos;
}
