using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit1State : PlayerDamagedState {

    bool isTakingDamage;
    bool isInvincible;
    bool hitSideRight;

    public int enemyDamage = 1;

    float hitForceX;
    float hitForceY;

    public PlayerHit1State(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
        isTakingDamage = false;
        isInvincible = false;
    }

    public override void AnimationTrigger() {
        base.AnimationTrigger();
    }
    public override void Enter() {
        base.Enter();

        hitForceX = 4f; hitForceY = 5f;

        TakeDamage(enemyDamage);
    }
    public override void Exit() {
        base.Exit();
    }
    public override void LogicUpdate() {
        base.LogicUpdate();
        if (isAnimationFinished) {
            stateMachine.ChangeState(player.IdleState);
        }
    }
    public void HitSide(bool rightSide) {
        this.hitSideRight = rightSide;
    }
    public void Invincible(bool invincibility) {
        this.isInvincible = invincibility;
    }
    void TakeDamage(int damage) {
        if (!isInvincible) {
            player.currentHealth -= damage;
            player.currentHealth = Mathf.Clamp(player.currentHealth, 0, playerData.maxHealth);
            UIHealthBar.Instance.SetValue(player.currentHealth / (float)playerData.maxHealth);
            if (player.currentHealth <= 0) {
                player.Defeat();
            } else {
                StartDamageAnimation();
            }
        }
    }
    void StartDamageAnimation() {
        if (!isTakingDamage) {
            isTakingDamage = true;
            isInvincible = true;
            if (hitSideRight) hitForceX = -hitForceX;
            player.RB.drag = 0f;
            player.RB.velocity = Vector2.zero;
            player.RB.AddForce(new Vector2(hitForceX, hitForceY), ForceMode2D.Impulse);
            SoundManager.Instance.Play(playerData.takingDamageClip);
        }
    }
}
