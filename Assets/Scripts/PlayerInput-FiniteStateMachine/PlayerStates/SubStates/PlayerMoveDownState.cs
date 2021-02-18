﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveDownState : PlayerGroundedState
{
    public PlayerMoveDownState(PlayerX player, PlayerStateMachineX stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        player.SetVelocity(playerData.movementVelocity * input);

        if (input == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleDownState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
