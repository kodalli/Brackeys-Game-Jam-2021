using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }

    [SerializeField] private PlayerDataX playerDataX;

    #endregion
    public PlayerInputHandler InputHandler { get; private set; }

    public Animator Anim { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    private Vector2 workspace;
    public int FacingDirection { get; private set; }

    #region Check Transforms

    [SerializeField] private Transform groundCheck;

    #endregion

    private void Awake(){
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerDataX, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerDataX, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerDataX, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerDataX, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerDataX, "land");
    }
    private void Start(){
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);
    }
    private void Update(){
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }
    private void FixedUpdate(){
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public void SetVelocity(float velocity){
        workspace.Set(velocity, CurrentVelocity.y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    public void JumpVelocity(float velocity) {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    public void CheckIfShouldFlip(int xInput){
        if(xInput != 0 && xInput != FacingDirection){
            Flip();
        }
    }
    public bool CheckIfGrounded() => Physics2D.OverlapCircle(groundCheck.position, playerDataX.groundCheckRadius, playerDataX.whatIsGround);

    #region Other Functions
    private void Flip(){
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger(); // Being used as a trigger on the "Land" animation

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(groundCheck.position, playerDataX.groundCheckRadius);
    }
    #endregion
}
