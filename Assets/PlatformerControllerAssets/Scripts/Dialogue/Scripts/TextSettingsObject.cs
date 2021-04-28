using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TextSettingsObject", menuName = "ScriptableObjects/TextSettingsObject", order = 0)]
public class TextSettingsObject : ScriptableObject {
    public float letterSpacing = 6f;
    public float wordSpacing = 10f;
    public float lineSpacing = 110f;
    public float letterSize = 80f;
    public float indentLeft = 50f;
    public float indentRight = 50f;
    public float indentTop = -120f;
    public float indentBottom = 0f;
}

// letterSpacing = 4f;
// wordSpacing = 10f;
// lineSpacing = 70f;
// letterSize = 50f;
// indentLeft = 40f;
// indentRight = 40f;
// indentTop = -60f;
// indentBottom = 0f;