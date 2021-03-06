using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveYState : PlayerGroundedState
{
    public PlayerMoveYState(Player player, PlayerStateMachine stateMachine, PlayerDataX playerDataX, string animBoolName) : base(player, stateMachine, playerDataX, animBoolName)
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

        player.CheckIfShouldFlipY(yInput);
        
        player.SetVelocity(playerDataX.movementVelocity  * yInput );

        if(yInput == 0){
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
