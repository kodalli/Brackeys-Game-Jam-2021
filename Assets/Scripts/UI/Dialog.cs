using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public static Dialog Instance;
    public TextMeshProUGUI textDisplay;
    public GameObject dialogBox;
    public GameObject continueButton;
    public GameObject skipButton;
    public List<string> sentences;
    public int index;
    public float typingSpeed;
    public int maxSentenceSize = 25;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!PlayerControlSave.Instance.localPlayerData.finishedTutorial) {
            dialogBox.SetActive(true);
            StartCoroutine(Type());
        }
    }

    IEnumerator Type()
    {
        skipButton.SetActive(true);

        foreach (char letter in sentences[index].ToCharArray()){
            textDisplay.text += letter;
            yield return new WaitForEndOfFrame();
        }

        continueButton.SetActive(true);
    }

    public void NextSentence()
    {
        continueButton.SetActive(false);

        if(index < sentences.Count - 1){
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        } else{
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
        //string totalText = "";
        //dialogueList.ForEach(text => totalText += text);
        //sentences.Clear();
        //for (int i = 1; i < totalText.Length/maxSentenceSize; i++) {
        //    sentences.Add(totalText.Substring())
        //}

        sentences = dialogueList;
        index = 0;
        dialogBox.SetActive(true);
        StartCoroutine(Type());
    }

    public bool IsDialogueOver() => index > sentences.Count - 1;
}
