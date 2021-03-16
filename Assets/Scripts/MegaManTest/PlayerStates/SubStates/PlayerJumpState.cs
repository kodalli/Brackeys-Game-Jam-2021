using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState {
    private int amountOfJumpsLeft;
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerDataX playerDataX, string animBoolName) : base(player, stateMachine, playerDataX, animBoolName) {
        amountOfJumpsLeft = playerDataX.amountofJumps;
    }

    public override void Enter() {
        base.Enter();

        player.JumpVelocity(playerDataX.jumpVelocity);
        isAbilityDone = true;
        amountOfJumpsLeft--;
        player.InAirState.SetIsJumping();
    }
    public bool canJump(){
        if(amountOfJumpsLeft > 0) { return true; }
        else { return false; }
    }
    public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = playerDataX.amountofJumps;
    public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;
}
