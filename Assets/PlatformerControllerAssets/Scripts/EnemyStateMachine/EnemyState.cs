using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState {

    protected EnemyStateMachine stateMachine;
    protected Entity entity;

    protected float startTime; // When the enemy entered this state

    /// <summary>
    /// Base constructor defining every state
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="stateMachine"></param>
    public EnemyState(Entity entity, EnemyStateMachine stateMachine){
        this.entity = entity;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter(){
        startTime = Time.time;
    }
    public virtual void Exit(){

    }
    public virtual void LogicUpdate(){

    }
    public virtual void PhysicsUpdate(){

    }




}
