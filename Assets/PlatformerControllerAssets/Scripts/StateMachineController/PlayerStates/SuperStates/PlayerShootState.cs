using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootState : PlayerState {

    private int xInput;

    public bool keyShoot;
    public bool isShooting;
    private float shootTime;
    private bool keyShootRelease;


    public PlayerShootState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
        player.Anim.SetBool("runShoot", false);
    }
    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        keyShoot = player.InputHandler.KeyShoot;

        player.CheckIfShouldFlip(xInput);

        PlayerShootInput();

        if (xInput != 0) {
            player.Anim.SetBool("runShoot", true);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        } else if (keyShootRelease) {
            stateMachine.ChangeState(player.IdleState);
        }
    }
    public void PlayerShootInput() {
        float shootTimeLength = 0;
        float keyShootReleaseTimeLength = 0;

        if (keyShoot && keyShootRelease) {
            isShooting = true;
            keyShootRelease = false;
            shootTime = Time.time;
            PlayerX.Instance.ShootBullet(); // Shoot Bullet
        }
        if (!keyShoot && !keyShootRelease) {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        if (isShooting) {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength > 0.25 || keyShootReleaseTimeLength >= 0.15) {
                isShooting = false;
            }
        }
    }
}
