using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace TimelineUtils {
    public class SubtitleTrackMixer : PlayableBehaviour {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            TextMeshProUGUI text = playerData as TextMeshProUGUI;
            string currentText = "";
            float currentAlpha = 0f;
            Color finalColor = Color.black;

            if (!text) { return; }

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++) {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0f) {
                    ScriptPlayable<SubtitleBehavior> inputPlayable = (ScriptPlayable<SubtitleBehavior>)playable.GetInput(i);
                    SubtitleBehavior input = inputPlayable.GetBehaviour();
                    currentText = input.subtitleText;
                    currentAlpha = inputWeight;
                    finalColor += input.color * inputWeight;
                }
            }

            text.text = currentText;
            text.color = new Color(finalColor.r, finalColor.g, finalColor.b, currentAlpha);
        }
    }
}
