using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : Singleton<GameStateMachine> {
    public GameMode gameMode = GameMode.Gameplay;

	public enum GameMode {
		Gameplay,
		DialogueMoment, //waiting for input
	}
}
