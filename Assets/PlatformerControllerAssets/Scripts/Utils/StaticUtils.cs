using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
using System.Linq;
// using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Useful static functions
/// </summary>
namespace StaticUtils {
    public static class StaticUtils {


        /// <summary>
        /// Returns the first gameabject in children with a specific name.
        /// </summary>
        public static GameObject FindInChildren(this GameObject go, string name) {
            return (from x in go.GetComponentsInChildren<Transform>()
                    where Animator.StringToHash(x.gameObject.name) == Animator.StringToHash(name)
                    select x.gameObject).First();
        }


        /// <summary>
        /// Sets all the children gameobjects with a specific game component to true or false.
        /// </summary>
        public static void SetActiveAllChildren<T>(this GameObject go, bool state) where T : UnityEngine.Component {
            go.GetComponentsInChildren<T>().ToList().ForEach(x => x.gameObject.SetActive(state));
            go.SetActive(true);
        }


        public static void DestroyAllChildren<T>(this GameObject go) where T : UnityEngine.Component {
            go.GetComponentsInChildren<T>()
                .ToList().ForEach(x => { if (Animator.StringToHash(x.name) != Animator.StringToHash(go.name)) { UnityEngine.Object.Destroy(x.gameObject); } });
        }


        /// <summary>
        /// Allows a loop with the item and index. <para />
        /// <example>
        /// This sample shows how to call the <see cref="WithIndex"/> method.
        /// <code>
        /// foreach (var (item, index) in collection.WithIndex()) {
        ///     DoSomething(item, index) 
        /// }
        /// </code>
        /// </example>
        /// </summary>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
            return source.Select((item, index) => (item, index));
        }

        /// <summary>
        /// Returns all the state names from an animator as a string array.
        /// </summary>
        public static AnimatorState[] GetStateNames(Animator animator) {
            AnimatorController controller = animator ? animator.runtimeAnimatorController as AnimatorController : null;
            return controller == null ? null : controller.layers.SelectMany(l => l.stateMachine.states).Select(s => s.state).ToArray();
        }



        public static bool IsPlayingOnLayer(this Animator animator, int fullPathHash, int layer) {
            return animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == fullPathHash;
        }

        public static float NormalizedTime(this Animator animator, int layer) {
            float time = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
            return time > 1 ? 1 : time;
        }


    }
}
