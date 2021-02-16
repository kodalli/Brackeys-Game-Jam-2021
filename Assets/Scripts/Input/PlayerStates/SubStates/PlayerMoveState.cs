using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerX player, PlayerStateMachineX stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        player.CheckIfFlip();

        player.SetVelocity(playerData.movementVelocity * input);
        if (input == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if(/*input != Vector2.zero &&*/ FlyInput)
        {
            player.InputHandler.UseFlyInput();
            stateMachine.ChangeState(player.FlyState);
        }

        //if(xInput > 0)
        //{
        //    player.thisPlayer.transform.rotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
        //}
        //if (xInput < 0)
        //{
        //    player.thisPlayer.transform.rotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
        //}

        //player.SetVelocityX(playerData.movementVelocity * xInput);
        //if (xInput == 0)
        //{
        //    stateMachine.ChangeState(player.IdleState);
        //}

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
