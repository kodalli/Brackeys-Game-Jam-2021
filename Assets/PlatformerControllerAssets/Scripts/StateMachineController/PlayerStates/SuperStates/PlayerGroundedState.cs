using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState {

    protected int xInput;
    private bool dashInput;
    private bool jumpInput;
    private bool keyShoot;

    private bool isGrounded;
 
    public PlayerGroundedState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter() {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
        player.DashState.ResetCanDash();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        dashInput = player.InputHandler.DashInput;
        keyShoot = player.InputHandler.KeyShoot;

        if (jumpInput && player.JumpState.canJump()) {
            stateMachine.ChangeState(player.JumpState);
        } else if (!isGrounded) {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        } else if (dashInput && player.DashState.CheckIfCanDash()) {
            stateMachine.ChangeState(player.DashState);
        }
        else if (keyShoot || (keyShoot && xInput != 0)) {
            stateMachine.ChangeState(player.ShootState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
