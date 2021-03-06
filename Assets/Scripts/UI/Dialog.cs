﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : Singleton<Dialog>{

    #region Variables
    [Header("Opening Dialogue Variables")]

    [Space(10)]
    public TextMeshProUGUI textDisplay;
    public GameObject dialogBox;
    public GameObject continueButton;
    public GameObject skipButton;
    public List<string> sentences;
    [SerializeField] private int maxSentenceSize = 20;
    public int index;

    [Header("Menu Variables")]
    public GameObject squadMenu;
    public GameObject itemMenu;
    
    [Header("Timeline Variables")]
    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI dialogueLineText;
    public GameObject toggleSpacebarMessage;
    public GameObject dialoguePanel;

    [Header("Other Variables")]
    public GameObject signBox;

    #endregion

    void Start() {
        //if (!GlobalControlSave.Instance.savedPlayerData.finishedTutorial) {
        //    DisplayTextInDialogueBox(sentences);
        //}
    }
    #region Starting Village Dialogue Functions
    IEnumerator Type() {
        skipButton.SetActive(true);
        var prevIndex = index;
        foreach (char letter in sentences[index].ToCharArray()) {
            if (prevIndex != index) yield break;
            textDisplay.text += letter;
            yield return new WaitForEndOfFrame();
        }

        continueButton.SetActive(true);

    }

    public void NextSentence() {
        continueButton.SetActive(false);
        textDisplay.text = "";

        if (index < sentences.Count - 1) {
            index++;
            StartCoroutine(Type());
        }
        else {
            index++;
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
    #endregion

    #region Menus and Sign Dialogue Functions
    public void DisplaySignText(string text) {
        signBox.SetActive(true);
        signBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void ToggleSquadMenu() {
        if (squadMenu.activeSelf) {
            squadMenu.SetActive(false);
        }
        else {
            itemMenu.SetActive(false);
            squadMenu.SetActive(true);
            StartCoroutine(DisableScrollRect());
        }
    }

    public void ToggleItemMenu() {
        if (itemMenu.activeSelf) {
            itemMenu.SetActive(false);
        }
        else {
            squadMenu.SetActive(false);
            itemMenu.SetActive(true);
            itemMenu.GetComponent<ScrollRect>().horizontal = false;
        }
    }

    IEnumerator DisableScrollRect() {
        yield return new WaitForSeconds(0.5f);
        squadMenu.GetComponent<ScrollRect>().horizontal = false;

    }
    #endregion

    #region Timeline Toggle Functions
    public void SetTimelineDialogue(string charName, string lineOfDialogue, int sizeOfDialogue, float typeSpeed) {
        charNameText.SetText(charName);
        //dialogueLineText.SetText(lineOfDialogue);
        StartCoroutine(TimelineTypeText(lineOfDialogue, typeSpeed));
        dialogueLineText.fontSize = sizeOfDialogue;

        ToggleTimelineDialoguePanel(true);
    }

    public void ToggleTimelineSkip(bool active) {
        toggleSpacebarMessage.SetActive(active);
    }

    public void ToggleTimelineDialoguePanel(bool active) {
        dialoguePanel.SetActive(active);
    }

    IEnumerator TimelineTypeText(string dialogue, float typeSpeed) {
        for(int i = 0; i < dialogue.Length+1; i++) {
            dialogueLineText.SetText(dialogue.Substring(0, i));
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    #endregion

}

