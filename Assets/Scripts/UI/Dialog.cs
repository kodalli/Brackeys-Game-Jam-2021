using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject dialogBox;
    public GameObject continueButton;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    void Start()
    {

        if (!PlayerControlSave.Instance.localPlayerData.finishedTutorial) {
            dialogBox.SetActive(true);
            StartCoroutine(Type());
        }
    }
    void Update()
    {
        if(textDisplay.text == sentences[index]){
            continueButton.SetActive(true);
        }
    }
    IEnumerator Type()
    {
        foreach(char letter in sentences[index].ToCharArray()){
            textDisplay.text += letter;
            yield return new WaitForEndOfFrame();
        }
    }
    public void NextSentence()
    {
        continueButton.SetActive(false);

        if(index < sentences.Length - 1){
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        } else{
            textDisplay.text = "";
            continueButton.SetActive(false);
            dialogBox.SetActive(false);
            PlayerControlSave.Instance.localPlayerData.finishedTutorial = true;
        }
    }
}
