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
    private int dashX;
    private int dashY;

    readonly int HealthBar = Shader.PropertyToID("_Health");

    public PlayerDashState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }
    public override void AnimationTrigger() {
        base.AnimationTrigger();
        player.Anim.enabled = false;
    }
    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
    }
    public override void Enter() {
        base.Enter();

        // If you're in this state, it means you've pressed dash, so you let the state and InputHandler know
        CanDash = false;
        isHolding = true; // If you're in this state, then you are pressing dash, so it is initially true
        player.InputHandler.UseDashInput();
        dashDirection = Vector2.right * player.FacingDirection;

        if (playerData.dashTimeFreeze) Time.timeScale = playerData.holdTimeScale; // Slows down time if its enabled in playerData

        startTime = Time.unscaledTime;
    }

    public override void Exit() {
        base.Exit();

        // Fall down cleaner
        if (player.CurrentVelocity.y > 0) {
            player.SetVelocityY(player.CurrentVelocity.y * playerData.variableDashMultiplier);
        }
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (!isExitingState) {

            // Set x and y velocity of player
            player.Anim.SetFloat("yVelocity", player.CurrentVelocity.y);
            player.Anim.SetFloat("xVelocity", Mathf.Abs(player.CurrentVelocity.x));

            // Logic for when Dash button is being held down
            if (isHolding) {

                player.DashTimeIndicatorMaterial.SetFloat(HealthBar, Time.unscaledTime - startTime);

                // ** Set all InputHandler variables **
                dashX = player.InputHandler.DashInputX;
                dashY = player.InputHandler.DashInputY;
                dashInputStop = player.InputHandler.DashInputStop;

                player.DashTimeIndicator.gameObject.SetActive(true);

                RotateIndicator(); // Rotate Indicator based on ArrowKeys Input

                if (dashInputStop || Time.unscaledTime >= startTime + playerData.dashMaxHoldTime) {
                    ApplyDash(); // Apply Dash Logic
                }
            }
            else {
                ApplyDashTimeEnd(); // Apply Dash if buttoh is held down too long
            }
        }
    }
    private void ApplyDash() {
        isHolding = false;
        startTime = Time.time;
        player.DashTimeIndicator.gameObject.SetActive(false);
        if (playerData.dashTimeFreeze) Time.timeScale = 1f; // Enable Time Freeze

        // ** Use this for arrow key dashing **
        // ** Remove if you want to use mousePos dashing **
        dashDirection.x = dashX;
        dashDirection.y = dashY;

        if(dashDirection == Vector2.zero) {
            dashDirection.x = player.FacingDirection;
        }

        player.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x)); // Rotate Player based on dash direction
        player.RB.drag = playerData.drag;
        player.Anim.enabled = true;
        player.SetDashVelocity(playerData.dashVelocity, dashDirection.normalized); // Actually dash the player in wanted direction
        player.DashDirectionIndicator.gameObject.SetActive(false); // Disable the indicator once dash is released
    }
    private void ApplyDashTimeEnd() {
        player.Anim.enabled = true;
        player.SetDashVelocity(playerData.dashVelocity, dashDirection);
        player.DashTimeIndicator.gameObject.SetActive(false);

        if (Time.time >= startTime + playerData.dashTime) {
            player.RB.drag = 0f;
            isAbilityDone = true; // Moves player back into AirState
            lashDashTime = Time.time;
        }
    }
    public bool CheckIfCanDash() => CanDash && Time.time > lashDashTime + playerData.dashCoolDown;
    public void ResetCanDash() => CanDash = true;
    public void RotateIndicator() {
        if (dashX != 0 || dashY != 0) {
            player.DashDirectionIndicator.gameObject.SetActive(true);
            if (dashX > 0) {
                player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, 45f - player.InputHandler.angleRotations.right);
            }
            else if (dashX < 0) {
                player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, 45f - player.InputHandler.angleRotations.left);
            }
            else if (dashY > 0) {
                player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, 45f - player.InputHandler.angleRotations.up);
            }
            else if (dashY < 0) {
                player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, 45f - player.InputHandler.angleRotations.down);
            }
            else {
                player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, 45f); ;
            }
        }
        else {
            player.DashDirectionIndicator.gameObject.SetActive(false);
        }
    }

}
