// using System;
// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEngine.Timeline;

// namespace TimelineUtils {
// 	[Serializable]
// 	public class DialogueBehaviour : PlayableBehaviour {
// 		[SerializeField] private string characterName;
// 		[SerializeField] private string dialogueLine;
// 		[SerializeField] private int dialogueSize;
// 		[SerializeField] private float typeSpeed = 0.05f;
// 		[SerializeField] private bool hasToPause = false;

// 		private bool clipPlayed = false;
// 		private bool pauseScheduled = false;
// 		private PlayableDirector director;

// 		public override void OnPlayableCreate(Playable playable) {
// 			director = (playable.GetGraph().GetResolver() as PlayableDirector);
// 		}

// 		public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
// 			if (!clipPlayed && info.weight > 0f) {
// 				//UIManager.Instance.SetDialogue(characterName, dialogueLine, dialogueSize);
// 				Dialog.Instance.SetTimelineDialogue(characterName, dialogueLine, dialogueSize, typeSpeed);

// 				if (Application.isPlaying) {
// 					if (hasToPause) {
// 						pauseScheduled = true;
// 					}
// 				}

// 				clipPlayed = true;
// 			}
// 		}

// 		public override void OnBehaviourPause(Playable playable, FrameData info) {
// 			if (pauseScheduled) {
// 				pauseScheduled = false;
// 				TimelineController.Instance.PauseTimeline(director);
// 			}
// 			else {
// 				Dialog.Instance.ToggleTimelineDialoguePanel(false);
// 			}

// 			clipPlayed = false;
// 		}
// 	}
// }
