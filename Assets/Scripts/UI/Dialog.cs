using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour {
    public static Dialog Instance;
    public TextMeshProUGUI textDisplay;
    public GameObject dialogBox;
    public GameObject continueButton;
    public GameObject skipButton;
    public List<string> sentences;
    public int index;
    [SerializeField] private int maxSentenceSize = 20;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        if (!GlobalControlSave.Instance.savedPlayerData.finishedTutorial) {
            DisplayTextInDialogueBox(sentences);
        }
    }

    IEnumerator Type() {
        skipButton.SetActive(true);

        foreach (char letter in sentences[index].ToCharArray()) {
            textDisplay.text += letter;
            yield return new WaitForEndOfFrame();
        }

        continueButton.SetActive(true);
    }

    public void NextSentence() {
        continueButton.SetActive(false);

        if (index < sentences.Count - 1) {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else {
            index++;
            textDisplay.text = "";
            continueButton.SetActive(false);
            skipButton.SetActive(false);
            dialogBox.SetActive(false);
            PlayerControlSave.Instance.localPlayerData.finishedTutorial = true;
            PlayerController.Instance.movementIsActive = true;
            //sentences.Clear();
        }
    }

    public void SkipDialogue() {
        index = sentences.Count;
        NextSentence();
    }

    public void DisplayTextInDialogueBox(List<string> dialogueList) {
        textDisplay.text = "";
        sentences = NormalizeDialogueList(dialogueList);
        index = 0;
        dialogBox.SetActive(true);
        StartCoroutine(Type());
    }

    private List<string> NormalizeDialogueList(List<string> dialogueList) {
        List<string> sentences = new List<string>();

        foreach (var str in dialogueList) {
            if (str.Length > maxSentenceSize) {
                // split up text
                List<string> splitUP = new List<string>();
                for (int i = 0; i < str.Length; i += maxSentenceSize) {
                    var text = str.Substring(i, Mathf.Min(maxSentenceSize, str.Length - i));
                    splitUP.Add(text);
                }
                sentences.AddRange(splitUP);
            }
            else {
                sentences.Add(str);
            }
        }

        return sentences;
    }

    public bool IsDialogueOver() => index > sentences.Count - 1;
}
