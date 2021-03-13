using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected int xInput;

    private bool JumpInput;
    protected bool isGrounded;
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerDataX playerDataX, string animBoolName) : base(player, stateMachine, playerDataX, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = player.CheckIfGrounded();
    }
    public override void Enter()
    {
        base.Enter();

        player.JumpState.ResetAmountOfJumpsLeft();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        JumpInput = player.InputHandler.JumpInput;
        if (JumpInput && player.JumpState.canJump()) {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.JumpState);
        }else if (!isGrounded) {
            player.InAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
