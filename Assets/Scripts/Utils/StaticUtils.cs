using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
using System.Linq;
// using UnityEngine.EventSystems;

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
}
