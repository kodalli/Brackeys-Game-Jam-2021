using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerGroundedState {
    public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerDataX playerDataX, string animBoolName) : base(player, stateMachine, playerDataX, animBoolName) {
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if(xInput != 0) {
            stateMachine.ChangeState(player.MoveState);
        }
        else if (isAnimationFinished) {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
