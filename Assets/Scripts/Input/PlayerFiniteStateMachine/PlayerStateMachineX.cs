using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachineX
{
    public PlayerStateX CurrentState { get; private set; }

    public void Initialize(PlayerStateX startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }
    public void ChangeState(PlayerStateX newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
