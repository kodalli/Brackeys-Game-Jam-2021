using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public EnemyStateMachine StateMachine;
    public D_Entity entityData;

    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public GameObject AliveGO { get; private set; }
    public int FacingDirection { get; private set; }
    public AnimationToStateMachine atsm { get; private set; }

    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform playerCheck;

    private Vector2 velocityWorkspace;

    public virtual void Start() {

        FacingDirection = 1;

        AliveGO = transform.Find("Alive").gameObject;
        RB = AliveGO.GetComponent<Rigidbody2D>();
        Anim = AliveGO.GetComponent<Animator>();
        atsm = AliveGO.GetComponent<AnimationToStateMachine>();

        StateMachine = new EnemyStateMachine();
    }

    public virtual void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }
    public virtual void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity) {
        velocityWorkspace.Set(FacingDirection * velocity, RB.velocity.y);
        RB.velocity = velocityWorkspace;
    }
    public virtual bool CheckWall() => Physics2D.Raycast(wallCheck.position, AliveGO.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    public virtual bool CheckLedge() => Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);

    public virtual bool CheckPlayerInMinAgroRange() => Physics2D.Raycast(playerCheck.position, AliveGO.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    public virtual bool CheckPlayerInMaxAgroRange() => Physics2D.Raycast(playerCheck.position, AliveGO.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);

    public virtual bool CheckPlayerInCloseRangeAction() => Physics2D.Raycast(playerCheck.position, AliveGO.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);

    public virtual void Flip() {
        FacingDirection *= -1;
        AliveGO.transform.Rotate(0f, 180f, 0f);
    }

    public virtual void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * FacingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * FacingDirection * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * FacingDirection * entityData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * FacingDirection * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * FacingDirection * entityData.maxAgroDistance), 0.2f);
    }
}
