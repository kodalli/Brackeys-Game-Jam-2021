using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState {

    private int amountOfJumpsLeft;
    public PlayerJumpState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }

    public override void Enter() {
        base.Enter();

        player.InputHandler.UseJumpInput();
        player.SetVelocityY(playerData.jumpVelocity);
        isAbilityDone = true;
        amountOfJumpsLeft--;
    }
    public bool canJump() {
        if (amountOfJumpsLeft > 0) return true;
        else return false;
    }
    public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = playerData.amountOfJumps;
    public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;
}
