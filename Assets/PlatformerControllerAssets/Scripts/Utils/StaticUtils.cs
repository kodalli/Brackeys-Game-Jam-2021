using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
using System.Linq;
// using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.Animations;

namespace StaticUtils {
    public static class StaticUtils {
        public static GameObject FindInChildren(this GameObject go, string name) {
            return (from x in go.GetComponentsInChildren<Transform>()
                    where x.gameObject.name == name
                    select x.gameObject).First();
        }

        public static void SetActiveAllChildren<T>(this GameObject go, bool state) where T : UnityEngine.Component {
            go.GetComponentsInChildren<T>().ToList().ForEach(x => x.gameObject.SetActive(state));
            go.SetActive(true);
        }

        public static AnimatorState[] GetStateNames(Animator animator) {
            AnimatorController controller = animator ? animator.runtimeAnimatorController as AnimatorController : null;
            return controller == null ? null : controller.layers.SelectMany(l => l.stateMachine.states).Select(s => s.state).ToArray();
        }
    }

}
