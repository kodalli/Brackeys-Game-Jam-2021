using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Player Data/Base Player Data")]
public class PlayerData : ScriptableObject {

    [Header("Debug On/Off")]
    public bool debugMode;

    [Header("Player Info")]
    public int maxHealth = 100;

    [Header("Prefabs")]
    public GameObject explodeEffectPrefab;

    [Header("Audio Clips")]
    public AudioClip jumpLandedClip;
    public AudioClip shootBulletClip;
    public AudioClip takingDamageClip;
    public AudioClip explodeEffectClip;


    #region State Variables

    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 23f;
    public int amountOfJumps = 1;

    [Header("Air State")]
    public float coyoteTime = 0.2f;
    public float fallMulitiplier = 5f;
    public float lowJumpMulitiplier = 10f;

    [Header("Dash State")]
    public float dashCoolDown = 0.5f;
    public float dashMaxHoldTime = 5f;

    [Tooltip("Slows down time down during dash if enabled.")]
    public float holdTimeScale = 0.25f;

    [Tooltip("Duration of the dash animation.")]
    public float dashTime = 0.2f;
    public float dashVelocity = 30f;

    [Tooltip("Changes drag field in rigidybody, slows down player.")]
    public float drag = 10f;
    public float variableDashMultiplier = 0.2f;
    public float distanceBetweenAfterImages = 0.5f;

    [Tooltip("Toggles time freeze during dash.")]
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

    [Header("Shoot State")]
    public float bulletDamage = 1f;
    public float bulletSpeed = 5f;
    public Vector2 bulletDirection = new Vector2(1, 0);
    public float bulletDestroyDelay = 2f;

    #endregion
}
