using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerX : MonoBehaviour {

    #region Unity Editor Functions
#if UNITY_EDITOR
    void OnGUI() {
        if (!playerData.debugMode) return;

        // Calculate Frame Rate
        var current = (int)(1f / Time.unscaledDeltaTime);
        var avgFrameRate = (int)current;

        // Show Current State 
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 40;
        string state = StateMachine.CurrentState.ToString();

        // Display on Editor Window
        GUI.Label(new Rect(0, 0, 1000, 100), state, guiStyle);
        GUI.Label(new Rect(0, 100, 1000, 100), avgFrameRate.ToString(), guiStyle);
    }
#endif
    #endregion

    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    #region Components
    public PlayerInputHandler InputHandler { get; private set; }
    public Animator Anim { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public SpriteRenderer SR { get; private set; }
    public Transform DashTimeIndicator { get; private set; }
    public Transform DashDirectionIndicator { get; private set; }
    public Material DashTimeIndicatorMaterial { get; private set; }

    #endregion

    #region Check Transforms

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;

    #endregion

    #region Other Variables
    private Vector2 previousVelocity;
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }

    #endregion

    #region Unity Callback Functions
    private void Awake() {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "inAir");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimbState");
        DashState = new PlayerDashState(this, StateMachine, playerData, "dash");

    }
    private void Start() {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        DashDirectionIndicator = transform.Find("DashDirectionIndicator");
        DashTimeIndicator = transform.Find("DashTimeIndicator");
        DashTimeIndicatorMaterial = DashTimeIndicator.GetComponent<Renderer>().material;

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);
    }
    private void Update() {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }
    private void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Set Functions

    public void SetVelocityToZero() {
        RB.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetWallJumpVelocity(float velocity, Vector2 angle, int direction) {
        angle.Normalize();
        previousVelocity.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }
    public void SetDashVelocity(float velocity, Vector2 direction) {
        previousVelocity = direction * velocity;
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }

    public void SetVelocityX(float velocity) {
        previousVelocity.Set(velocity, CurrentVelocity.y);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }
    public void SetVelocityY(float velocity) {
        previousVelocity.Set(CurrentVelocity.x, velocity);
        RB.velocity = previousVelocity;
        CurrentVelocity = previousVelocity;
    }

    #endregion

    #region Check Functions

    public bool CheckIfTouchingWall() => Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    public bool CheckIfTouchingWallBack() => Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    public bool CheckIfGrounded() => Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    public bool CheckIfTouchingLedge() => Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);

    public void CheckIfShouldFlip(int xInput) {
        if (xInput != 0 && xInput != FacingDirection) {
            Flip();
        }
    }

    #endregion

    #region Animation Triggers
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    #endregion

    #region Other Functions

    // Used so Animation Events on animations can be used

    public Vector2 DetermineCornerPos() {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
        float xDistance = xHit.distance;
        previousVelocity.Set(xDistance * FacingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)previousVelocity, Vector2.down, ledgeCheck.position.y - wallCheck.position.y, playerData.whatIsGround);
        float yDistance = yHit.distance;

        previousVelocity.Set(wallCheck.position.x + (xDistance * FacingDirection), ledgeCheck.position.y - yDistance);
        Debug.DrawRay(ledgeCheck.position + (Vector3)previousVelocity, Vector2.down, Color.red, playerData.whatIsGround);
        return previousVelocity;
    }


    private void Flip() {
        FacingDirection *= -1;
        SR.flipX = FacingDirection != 1;
    }

    private void OnDrawGizmos() { // Used to Check Wall Check Distance
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * playerData.wallCheckDistance * FacingDirection));
    }
    #endregion

}
