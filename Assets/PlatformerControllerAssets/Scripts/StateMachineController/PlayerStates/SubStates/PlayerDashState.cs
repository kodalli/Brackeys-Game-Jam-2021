using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState {

    public bool CanDash { get; private set; }
    private bool isHolding;

    private float lashDashTime;

    private Vector2 dashDirection;
    private Vector2 dashDirectionInput;
    private bool dashInputStop;

    public PlayerDashState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Enter() {
        base.Enter();

        // If you're in this state, it means you've pressed dash, so you let the state and InputHandler know
        CanDash = false;
        player.InputHandler.UseDashInput();

        isHolding = true;
        dashDirection = Vector2.right * player.FacingDirection;

        Time.timeScale = playerData.holdTimeScale;
        startTime = Time.unscaledTime;

        player.DashDirectionIndicator.gameObject.SetActive(true);
    }

    public override void Exit() {
        base.Exit();

        if(player.CurrentVelocity.y > 0) {
            player.SetVelocityY(player.CurrentVelocity.y * playerData.variableDashMultiplier);
        }
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (!isExitingState) {
            if (isHolding) {
                dashDirectionInput = player.InputHandler.DashDirectionInput;
                dashInputStop = player.InputHandler.DashInputStop;

                if(dashDirectionInput != Vector2.zero) {
                    dashDirection = dashDirectionInput;
                    dashDirection.Normalize();
                }

                float angle = Vector2.SignedAngle(Vector2.right, dashDirection);
                player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, angle - 45f);

                if(dashInputStop || Time.unscaledTime >= startTime + playerData.dashMaxHoldTime) {
                    isHolding = false;
                    Time.timeScale = 1f;
                    startTime = Time.time;
                    player.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x));
                    player.RB.drag = playerData.drag;
                    player.SetDashVelocity(playerData.dashVelocity, dashDirection);
                    player.DashDirectionIndicator.gameObject.SetActive(false);
                }
            }
            else {
                player.SetDashVelocity(playerData.dashVelocity, dashDirection);

                if(Time.time >= startTime + playerData.dashTime) {
                    player.RB.drag = 0f;
                    isAbilityDone = true;
                    lashDashTime = Time.time;
                }
            }
        }
    }
    public bool CheckIfCanDash() => CanDash && Time.time > lashDashTime + playerData.dashCoolDown;
    public void ResetCanDash() => CanDash = true;

}
