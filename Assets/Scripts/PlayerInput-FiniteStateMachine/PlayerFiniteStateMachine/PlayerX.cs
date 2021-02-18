using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerX : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachineX StateMachine { get; private set; }

    public PlayerIdleState IdleUpState { get; private set; }
    public PlayerIdleDownState IdleDownState { get; private set; }
    public PlayerIdleLeftState IdleLeftState { get; private set; }
    public PlayerIdleRightState IdleRightState { get; private set; }
    public PlayerMoveState MoveUpState { get; private set; }
    public PlayerMoveDownState MoveDownState { get; private set; }
    public PlayerMoveRightState MoveRightState { get; private set; }
    public PlayerMoveLeftState MoveLeftState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    #region Components

    public PlayerInputHandler InputHandler { get; private set; }
    public Animator Anim { get; private set; }
    public Rigidbody2D RB { get; private set; }

    #endregion

    #region Other Variables
    public Vector2 CurrentVelocity { get; private set; }
   
    private Vector2 workspace;
    #endregion

    #region Check Transforms

    [SerializeField] private Transform groundCheck;

    public Transform thisPlayer { get; private set; }

    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        StateMachine = new PlayerStateMachineX();

        IdleUpState = new PlayerIdleState(this, StateMachine, playerData, "idleUp");
        IdleDownState = new PlayerIdleDownState(this, StateMachine, playerData, "idleDown");
        IdleLeftState = new PlayerIdleLeftState(this, StateMachine, playerData, "idleLeft");
        IdleRightState = new PlayerIdleRightState(this, StateMachine, playerData, "idleRight");
        MoveUpState =  new PlayerMoveState(this, StateMachine, playerData, "moveUp");
        MoveLeftState = new PlayerMoveLeftState(this, StateMachine, playerData, "moveLeft");
        MoveRightState = new PlayerMoveRightState(this, StateMachine, playerData, "moveRight");
        MoveDownState = new PlayerMoveDownState(this, StateMachine, playerData, "moveDown");
    }
    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        
        StateMachine.Initialize(IdleUpState);
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
    #endregion

    #region Set Functions
    public void SetVelocity(Vector2 velocity)
    {
        RB.velocity = velocity;
        CurrentVelocity = velocity;
    }
    #endregion

    #region Check Functions

    // Use when needed

    #endregion

    #region Other Functions

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    #endregion
}
