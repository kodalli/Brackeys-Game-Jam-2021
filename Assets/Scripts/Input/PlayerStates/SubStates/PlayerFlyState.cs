using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlyState : PlayerAbilityState
{
    public PlayerFlyState(PlayerX player, PlayerStateMachineX stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityY(playerData.flyVelocity);
        isAbilityDone = true;
    }
}
