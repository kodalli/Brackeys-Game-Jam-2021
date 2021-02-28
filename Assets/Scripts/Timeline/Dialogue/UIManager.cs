using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class UIManager : Singleton<UIManager>
{
	public GameMode gameMode = GameMode.Gameplay;
	public enum GameMode {
		Gameplay,
		DialogueMoment, //waiting for input
	}
    private void Update() {
		if (Input.GetKeyDown(KeyCode.Z)) {
			ResumeTimeline();
		}
	}

    public TextMeshProUGUI charNameText, dialogueLineText;
    public GameObject toggleSpacebarMessage, dialoguePanel;

	[SerializeField] private PlayableDirector activeDirector;

	public void SetDialogue(string charName, string lineOfDialogue, int sizeOfDialogue) {
		charNameText.SetText(charName);
		dialogueLineText.SetText(lineOfDialogue);
		dialogueLineText.fontSize = sizeOfDialogue;

		ToggleDialoguePanel(true);
	}

	public void TogglePressSpacebarMessage(bool active) {
		toggleSpacebarMessage.SetActive(active);
	}

	public void ToggleDialoguePanel(bool active) {
		dialoguePanel.SetActive(active);
	}

	public void PauseTimeline(PlayableDirector whichOne) {
		gameMode = GameMode.DialogueMoment; //InputManager will be waiting for a spacebar to resume
		activeDirector = whichOne;
		activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
		TogglePressSpacebarMessage(true);
	}
	public void ResumeTimeline() {
		gameMode = GameMode.Gameplay;
		TogglePressSpacebarMessage(false);
		ToggleDialoguePanel(false);
		activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
	}
}
