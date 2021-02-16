using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerStateX
{
    // version 1 
    protected Vector2 input;
    //version 2
    protected int xInput;

    public bool FlyInput;

    public PlayerGroundedState(PlayerX player, PlayerStateMachineX stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        input = player.InputHandler.RawMovementInput;
        xInput = player.InputHandler.NormInputX;
        FlyInput = player.InputHandler.FlyInput;

        if(FlyInput)
        {
            player.InputHandler.UseFlyInput();
            stateMachine.ChangeState(player.FlyState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
