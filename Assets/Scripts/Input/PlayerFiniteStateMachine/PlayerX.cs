using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerX : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachineX StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerFlyState FlyState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    public PlayerInputHandler InputHandler { get; private set; }
    public Animator Anim { get; private set; }

    public Vector2 CurrentVelocity { get; private set; }
    public bool facingRight { get; private set; }
    public int facingDirection { get; private set; }
    public Rigidbody2D RB { get; private set; }

    private Vector2 workspace;

    #region Check Transforms

    [SerializeField] private Transform groundCheck;

    public Transform thisPlayer { get; private set; }

    #endregion

    private void Awake()
    {
        StateMachine = new PlayerStateMachineX();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState =  new PlayerMoveState(this, StateMachine, playerData, "move");
        FlyState = new PlayerFlyState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
    }
    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        thisPlayer = gameObject.transform;
        facingRight = false;
        facingDirection = -1;
        
        StateMachine.Initialize(IdleState);
    }
    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }
    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #region Set Functions
    public void SetVelocity(Vector2 velocity)
    {
        RB.velocity = velocity;
        CurrentVelocity = velocity;
    }
    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    #endregion

    #region Check Functions
    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }
    public void CheckIfFlip()
    {
        if(RB.velocity.x >0 && !facingRight || RB.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }
    public void CheckIfFlipV2(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
            FlipV2();
    }
    #endregion

    #region Other Functions

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    private void FlipV2()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    #endregion
}
