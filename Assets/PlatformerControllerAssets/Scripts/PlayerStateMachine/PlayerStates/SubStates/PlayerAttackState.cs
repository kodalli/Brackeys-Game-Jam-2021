using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState {
    private Weapon weapon;

    private int xInput;

    private float velocityToSet;
    private bool setVelocity;

    private bool shouldCheckFlip;

    public PlayerAttackState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Enter() {
        base.Enter();

        setVelocity = false;
        shouldCheckFlip = true;

        // *** REMOVE AFTER SECONDARY ATTACK IMPLEMENTED ***

        if (!weapon)
            stateMachine.ChangeState(player.IdleState);

        weapon?.EnterWeapon();
    }
    public override void Exit() {
        base.Exit();

        weapon?.ExitWeapon();
    }
    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;

        if (shouldCheckFlip) {
            player.CheckIfShouldFlip(xInput);
        }

        if (setVelocity) {
            player.SetVelocityX(velocityToSet * player.FacingDirection);
        }
    }

    public void SetWeapon(Weapon weapon) {
        this.weapon = weapon;
        weapon.InitializeWeapon(this);
    }
    public void SetPlayerVelocity(float velocity) {
        player.SetVelocityX(velocity * player.FacingDirection);

        velocityToSet = velocity;
        setVelocity = true;
    }

    public void SetFlipCheck(bool check) {
        shouldCheckFlip = check;
    }

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

}
