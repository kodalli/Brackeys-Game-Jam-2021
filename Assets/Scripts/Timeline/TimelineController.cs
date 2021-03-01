using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : Singleton<TimelineController>
{
    private PlayableDirector activeDirector;

    private void Start() {
        activeDirector = GetComponent<PlayableDirector>();
    }

    #region Timeline Functions
    public void PauseTimeline(PlayableDirector whichOne) {
        if (!activeDirector.playableGraph.IsValid()) {
            Dialog.Instance.ToggleTimelineSkip(true);
            PlayerControlSave.Instance.localPlayerData.currentGameMode = GameMode.DialogueMoment;
            return;
        }

        Dialog.Instance.ToggleTimelineSkip(true);
        PlayerControlSave.Instance.localPlayerData.currentGameMode = GameMode.DialogueMoment; //InputManager will be waiting for a spacebar to resume
        activeDirector = whichOne;
        activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
    }
    public void ResumeTimeline() {
        if (!activeDirector.playableGraph.IsValid()) {
            PlayerControlSave.Instance.localPlayerData.currentGameMode = GameMode.Gameplay;
            PlayerControlSave.Instance.SaveData();
            Dialog.Instance.ToggleTimelineSkip(false);
            Dialog.Instance.ToggleTimelineDialoguePanel(false);
            return;
        }

        //GameStateMachine.Instance.gameMode = GameStateMachine.GameMode.Gameplay;
        Dialog.Instance.ToggleTimelineSkip(false);
        Dialog.Instance.ToggleTimelineDialoguePanel(false);
        activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
    #endregion
}
