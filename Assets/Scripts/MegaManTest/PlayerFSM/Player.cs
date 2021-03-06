using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerMoveYState MoveYState { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    [SerializeField] private PlayerDataX playerDataX;

    public Animator Anim { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    public Vector2 CurrentVelocityY { get; private set; }
    private Vector2 workspace;
    private Vector2 workspace2;
    public int FacingDirection { get; private set; }
    public int FacingDirection2 { get; private set; }

    private void Awake(){
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerDataX, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerDataX, "move");
        MoveYState = new PlayerMoveYState(this, StateMachine, playerDataX, "moveY");
    }
    private void Start(){
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();

        FacingDirection = 1;
        FacingDirection2 = 1;

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
    public void SetVelocityY(float velocity){
        workspace.Set(CurrentVelocityY.x, velocity);
        RB.velocity = workspace2;
        CurrentVelocityY = workspace2;
    }
    public void CheckIfShouldFlip(int xInput){
        if(xInput != 0 && xInput != FacingDirection){
            Flip();
        }
    }
    public void CheckIfShouldFlipY(int yInput){
        if(yInput != 0 && yInput != FacingDirection2){
            FlipY();
        }
    }
    private void Flip(){
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    private void FlipY(){
        FacingDirection2 *= -1;
        transform.Rotate(180.0f, 0.0f, 0.0f);
    }
}
