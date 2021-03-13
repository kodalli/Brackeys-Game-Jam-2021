using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState {

    private int xInput;
    protected bool isGrounded;
    private bool jumpInput;
    private bool coyoteTime;
    private bool isJumping;
    private bool jumpInputStop;
    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerDataX playerDataX, string animBoolName) : base(player, stateMachine, playerDataX, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        CheckCoyoteTime();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;

        CheckJumpMultiplier();

        if(isGrounded && player.CurrentVelocity.y < 0.01f) {
            stateMachine.ChangeState(player.LandState);
        }
        else if(jumpInput && player.JumpState.canJump()) {
            stateMachine.ChangeState(player.JumpState);
        }
        else {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocity(playerDataX.movementVelocity * xInput);

            player.Anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
    private void CheckJumpMultiplier() {
        if (isJumping) {
            if (jumpInputStop) {
                player.JumpVelocity(player.CurrentVelocity.y * playerDataX.variableJumpHeightMultiplier);
                isJumping = false;
            }
            else if (player.CurrentVelocity.y <= 0f) {
                isJumping = false;
            }
        }
    }
    private void CheckCoyoteTime() {
        if(coyoteTime && Time.time > startTime + playerDataX.coyoteTime) {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }
    public void StartCoyoteTime() => coyoteTime = true;
    public void SetIsJumping() => isJumping = true;
}
