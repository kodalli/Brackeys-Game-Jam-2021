using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Debug Variables")]
    public bool debugMode;

    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 23f;
    public int amountOfJumps = 1;

    [Header("Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Dash State")]
    public float dashCoolDown = 0.5f;
    public float dashMaxHoldTime = 5f;
    public float holdTimeScale = 0.25f;
    public float dashTime = 0.2f;
    public float dashVelocity = 30f;
    public float drag = 10f;
    public float variableDashMultiplier = 0.2f;
    public float distanceBetweenAfterImages = 0.5f;
    public bool dashTimeFreeze;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 2f;

    [Header("Wall Jump State")]
    public float wallJumpVelocity = 20f;
    public float wallJumpTime = 0.4f;
    public Vector2 wallJumpAngle = new Vector2(1, 5);

    [Header("Ledge Climb State")]
    public Vector2 startOffset = new Vector2(0.6f, 0.675f);
    public Vector2 stopOffset = new Vector2(0.1f, 0.4f);
    public float ledgeHangTime = 4f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.3f;
    public float wallCheckDistance = 0.5f;
    public LayerMask whatIsGround;
}
