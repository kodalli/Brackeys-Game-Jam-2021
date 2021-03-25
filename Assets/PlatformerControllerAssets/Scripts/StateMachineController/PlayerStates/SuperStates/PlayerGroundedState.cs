using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState {

    protected int xInput;
<<<<<<< Updated upstream
=======
    private bool dashInput;
>>>>>>> Stashed changes

    private bool jumpInput;
    private bool isGrounded;
    private bool isTouchingWall;
<<<<<<< Updated upstream
=======
 
>>>>>>> Stashed changes
    public PlayerGroundedState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
        isTouchingWall = player.CheckIfTouchingWall();
    }

    public override void Enter() {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
<<<<<<< Updated upstream
=======
        dashInput = player.InputHandler.DashInput;
>>>>>>> Stashed changes

        if (jumpInput && player.JumpState.canJump()) {
            stateMachine.ChangeState(player.JumpState);
        } else if (!isGrounded) {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
<<<<<<< Updated upstream
=======
        } else if (dashInput && player.DashState.CheckIfCanDash()) {
            stateMachine.ChangeState(player.DashState);
>>>>>>> Stashed changes
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
