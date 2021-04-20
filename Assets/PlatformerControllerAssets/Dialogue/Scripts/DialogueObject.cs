using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueObject", menuName = "ScriptableObjects/DialogueObject", order = 0)]
public class DialogueObject : ScriptableObject {
    public string dialogueID;

    [TextArea(5, 10)]
    public string[] dialogue;
    public Response[] responseOptions;

    [System.Serializable]
    public struct Response {
        [TextArea(5, 10)]
        public string text;
        public GameObject dialogueObject;
    }
}