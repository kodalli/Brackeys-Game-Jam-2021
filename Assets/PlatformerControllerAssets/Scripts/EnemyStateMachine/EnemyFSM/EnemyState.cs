using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState {

    protected EnemyStateMachine stateMachine;
    protected Entity entity;

    protected string animBoolName;

    protected float startTime; // When the enemy entered this state

    /// <summary>
    /// Base constructor defining every state
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="stateMachine"></param>
    public EnemyState(Entity entity, EnemyStateMachine stateMachine, string animBoolName) {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() {
        startTime = Time.time;
        entity.Anim.SetBool(animBoolName, true);
        DoChecks();
    }
    public virtual void Exit() {
        entity.Anim.SetBool(animBoolName, false);
    }
    public virtual void LogicUpdate() {

    }
    public virtual void PhysicsUpdate() {
        DoChecks();
    }
    public virtual void DoChecks() {

    }




}
