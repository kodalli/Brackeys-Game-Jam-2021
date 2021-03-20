using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState{

    protected PlayerX player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float stateEntryTime;

    private string animBoolName;
   
    public PlayerState(PlayerX player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter() {
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        stateEntryTime = Time.time;
        isAnimationFinished = false;
        isExitingState = false;
    }
    public virtual void Exit() {
        player.Anim.SetBool(animBoolName, false);
        isExitingState = true;
        // stateEntryTime = 0f;
    }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() {
        DoChecks();
    }
    public virtual void DoChecks() { }
    public virtual void AnimationTrigger() { }
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;


}
