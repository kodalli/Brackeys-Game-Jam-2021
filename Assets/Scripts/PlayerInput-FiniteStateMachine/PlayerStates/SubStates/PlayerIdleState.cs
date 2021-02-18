﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerX player, PlayerStateMachineX stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(Vector2.zero);
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (input != Vector2.zero)
        {
            if (input.x > 0)
                stateMachine.ChangeState(player.MoveRightState);
            if (input.x < 0)
                stateMachine.ChangeState(player.MoveLeftState);
            if (input.y > 0)
                stateMachine.ChangeState(player.MoveUpState);
            if (input.y < 0)
                stateMachine.ChangeState(player.MoveDownState);
        }

        //if(xInput != 0)
        //{
        //    stateMachine.ChangeState(player.MoveState);
        //}

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}