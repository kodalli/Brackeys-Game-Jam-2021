using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineUtils {
    public class PlayTimeline : MonoBehaviour {
        private PlayableDirector pd;

        private void Start() {
            pd = GetComponent<PlayableDirector>();
        }

        public void Play() => pd.Play();

    }
}