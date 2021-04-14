using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState {

    // Input
    private int xInput;
    private bool jumpInput;
    private bool jumpInputStop;
    private bool dashInput;

    // Checks
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isTouchingWallBack;
    private bool oldIsTouchingWall;
    private bool oldIsTouchingWallBack;
    private bool isTouchingLedge;

    // private bool isJumping;
    private bool coyoteTime;
    private bool wallJumpCoyoteTime;
    private float startWallJumpCoyoteTime;


    public PlayerInAirState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        oldIsTouchingWall = isTouchingWall;
        oldIsTouchingWallBack = isTouchingWallBack;

        isGrounded = player.CheckIfGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
        isTouchingWallBack = player.CheckIfTouchingWallBack();
        isTouchingLedge = player.CheckIfTouchingLedge();

        if (isTouchingWall && !isTouchingLedge) {
            player.LedgeClimbState.SetDetectedPos(player.transform.position);
        }

        if (!wallJumpCoyoteTime && !isTouchingWall && !isTouchingWallBack && (oldIsTouchingWall || oldIsTouchingWallBack)) {
            StartWallJumpCoyoteTime();
        }

    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();

        // Uncomment this if easy wall jumping is enabled (the commented if else below)   
        // -----------
        //oldIsTouchingWall = oldIsTouchingWallBack = false;
        //isTouchingWall = isTouchingWallBack = false;

    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        CheckCoyoteTime();
        CheckWallJumpCoyoteTime();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputStop = player.InputHandler.JumpInputStop;
        dashInput = player.InputHandler.DashInput;

        CheckJumpMultiplier();

        if (player.InputHandler.AttackInputs[(int)CombatInputs.primary]) {
            stateMachine.ChangeState(player.PrimaryAttackState);
        } else if (player.InputHandler.AttackInputs[(int)CombatInputs.secondary]) {
            stateMachine.ChangeState(player.SecondaryAttackState);
        } else if (isGrounded && player.CurrentVelocity.y < 0.01f) {
            stateMachine.ChangeState(player.LandState);
        } else if (isTouchingWall && !isTouchingLedge) {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
          // ** Use this ELSE IF to enable wall jumping without first sliding aka while falling while touching wall **
          //else if (jumpInput && (isTouchingWall || isTouchingWallBack || wallJumpCoyoteTime)) {
          //    StopWallJumpCoyoteTime();
          //    isTouchingWall = player.CheckIfTouchingWall();
          //    player.WallJumpState.DetermineWallJumpDirection(isTouchingWall);
          //    stateMachine.ChangeState(player.WallJumpState);
          //}
          else if (jumpInput && player.JumpState.canJump()) {
            stateMachine.ChangeState(player.JumpState);
        } else if (isTouchingWall && xInput == player.FacingDirection && player.CurrentVelocity.y <= 0) {
            stateMachine.ChangeState(player.WallSlideState);
        } else if (dashInput && player.DashState.CheckIfCanDash()) {
            stateMachine.ChangeState(player.DashState);
        } else {
            player.CheckIfShouldFlip(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);

            player.Anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
    private void CheckJumpMultiplier() {
        if (player.CurrentVelocity.y < 0) {
            player.SetVelocityY(player.CurrentVelocity.y + Physics2D.gravity.y * (playerData.fallMulitiplier - 1) * Time.deltaTime);
        } else if (player.CurrentVelocity.y > 0 && jumpInputStop) {
            player.SetVelocityY(player.CurrentVelocity.y + Physics2D.gravity.y * (playerData.lowJumpMulitiplier - 1) * Time.deltaTime);
        }
    }
    private void CheckCoyoteTime() {
        if (coyoteTime && Time.time > startTime + playerData.coyoteTime) {
            coyoteTime = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }
    public void StartCoyoteTime() => coyoteTime = true;
    private void CheckWallJumpCoyoteTime() {
        if (wallJumpCoyoteTime && Time.time > startWallJumpCoyoteTime + playerData.coyoteTime) {
            wallJumpCoyoteTime = false;
        }
    }
    public void StartWallJumpCoyoteTime() {
        wallJumpCoyoteTime = true;
        startWallJumpCoyoteTime = Time.time;
    }

    public void StopWallJumpCoyoteTime() => wallJumpCoyoteTime = false;
}
