using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootState : PlayerState {

    private int xInput;
    private bool keyShoot;
    private bool jumpInput;

    private bool isShooting;
    private float shootTime;
    private bool keyShootRelease;

    private bool isGrounded;


    public PlayerShootState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void DoChecks() {
        base.DoChecks();

        player.CheckIfGrounded();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
        player.Anim.SetBool("idleShoot", false);
        player.Anim.SetBool("runShoot", false);
        player.Anim.SetBool("inAirShoot", false);
    }
    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        keyShoot = player.InputHandler.KeyShoot;
        jumpInput = player.InputHandler.JumpInput;

        player.CheckIfShouldFlip(xInput);
        PlayerShootInput();

        if (xInput != 0) {
            player.Anim.SetBool("runShoot", true);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        } else if (xInput == 0 && isGrounded) {
            player.Anim.SetBool("idleShoot", true);
        } else if (jumpInput) {
            stateMachine.ChangeState(player.JumpState);
        } else if (!keyShoot) {
            stateMachine.ChangeState(player.IdleState);
        }
    }
    void PlayerShootInput() {
        float shootTimeLength = 0;
        float keyShootReleaseTimeLength = 0;

        if (keyShoot && keyShootRelease) {
            isShooting = true;
            keyShootRelease = false;
            shootTime = Time.time;
            SoundManager.Instance.Play(playerData.shootBulletClip);
            player.ShootBullet();
        }
        if (!keyShoot && !keyShootRelease) {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        if (isShooting) {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength > 0.25 || keyShootReleaseTimeLength >= 0.25) {
                isShooting = false;
            }
        }
    }

}
