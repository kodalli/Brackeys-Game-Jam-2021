using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootState : PlayerAbilityState {

    private bool shootInput;
    private bool isShooting;
    public PlayerShootState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger() {
        base.AnimationTrigger();
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();

        player.InputHandler.UseShootInput();
        // Debug.Log("in Dash State");
    }

    public override void Exit() {
        base.Exit();
        player.Anim.SetBool("hasShot", true);
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        shootInput = player.InputHandler.ShootInput;
        player.Invoke("ShootBullet", 0.1f);
        isAnimationFinished = true;
        player.Anim.SetBool("hasShot", true);
        if (isAnimationFinished && !isExitingState)
            stateMachine.ChangeState(player.IdleState);
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
