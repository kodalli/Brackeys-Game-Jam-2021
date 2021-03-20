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

    public PlayerDashState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }
    public override void AnimationTrigger() {
        base.AnimationTrigger();
    }

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
    }
    public override void Enter() {
        base.Enter();

        // If you're in this state, it means you've pressed dash, so you let the state and InputHandler know
        CanDash = false;
        player.InputHandler.UseDashInput();

        isHolding = true; // If you're in this state, then you are pressing dash, so it is initially true
        dashDirection = Vector2.right * player.FacingDirection;

        if (playerData.dashTimeStop) Time.timeScale = playerData.holdTimeScale;

        stateEntryTime = Time.unscaledTime;
        // player.DashDirectionIndicator.gameObject.SetActive(true);
    }

    public override void Exit() {
        base.Exit();

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

                // ** Set all InputHandler variables **
                dashX = player.InputHandler.DashInputX;
                dashY = player.InputHandler.DashInputY;
                dashDirectionInput = player.InputHandler.DashDirectionInput;
                dashInputStop = player.InputHandler.DashInputStop;

                if (dashDirectionInput != Vector2.zero) {
                    dashDirection = dashDirectionInput;
                    dashDirection.Normalize();
                }

                // float angle = Vector2.SignedAngle(Vector2.right, dashDirection); // Calculate angle to rotate indicator based on Mouse Position
                // player.DashDirectionIndicator.rotation = Quaternion.Euler(0f, 0f, angle - 45f); // Rotate Indicator Based on Mouse Position
                RotateObject(); // TODO:: Rotate Indicator based on ArrowKeys Input

                if (dashInputStop || Time.unscaledTime >= stateEntryTime + playerData.dashMaxHoldTime) {
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
        Time.timeScale = 1f;
        stateEntryTime = Time.time;

        // ** Use this for arrow key dashing **
        // ** Remove if you want to use mousePos dashing **
        dashDirection.x = dashX;
        dashDirection.y = dashY;

        player.CheckIfShouldFlip(Mathf.RoundToInt(dashDirection.x)); // Rotate Player based on dash direction
        player.RB.drag = playerData.drag;
        player.SetDashVelocity(playerData.dashVelocity, dashDirection.normalized); // Actually dash the player in wanted direction
        player.DashDirectionIndicator.gameObject.SetActive(false); // Disable the indicator once dash is released
    }
    private void ApplyDashTimeEnd() {
        player.SetDashVelocity(playerData.dashVelocity, dashDirection);

        if (Time.time >= stateEntryTime + playerData.dashTime) {
            player.RB.drag = 0f;
            isAbilityDone = true; // Moves player back into AirState
            lashDashTime = Time.time;
        }
    }
    public bool CheckIfCanDash() => CanDash && Time.time > lashDashTime + playerData.dashCoolDown;
    public void ResetCanDash() => CanDash = true;
    public void RotateObject() {
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
