using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Attatch this to something, ideally something in a unity canvas
public class SpriteLetterSystem : MonoBehaviour {
    private enum TextEffect { None, Wavy, Shaky }

    #region Variables
    public Texture2D charSheet; //This should be the resource you want to load, Make sure that the image is in Assets/Resources ideally. Its a folder that unity can load things from using Resources.LoadAll etc
    public Dictionary<char, CharData> loadedFont;
    public GameObject letterObject;
    public float letterSpacing = 3.5f;
    public float wordSpacing = 10f;
    public float lineSpacing = 100f;
    public float letterSize = 10f;
    private TextEffect activeEffect;
    private List<GameObject> letterObjects = new List<GameObject>();
    #endregion


    private void Awake() {
        loadedFont = FontLoader.LoadFontResource(charSheet);
    }

    private void Start() {
        GenerateSpriteText("I am a top level Chungoloist, and I have concluded with absolute certainty that Big Chungus himself shall enter into existence at 2:31 PM this April 9th. I have found this out using MATH.");
        transform.localScale = new Vector3(letterSize / 100f, letterSize / 100f, 1f);
        //StartCoroutine(TextFade(3f));
    }

    private void FixedUpdate() {
        //TextFade();
        TextWavy();
        //TextShaky();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // TextFade();
            TextWavy();
            // TextShaky();
        }
    }

    public void GenerateSpriteText(string textToGenerate) {
        if (letterObject == null) return;


        float xPosition = 0;
        float yPosition = 0;

        for (int i = 0; i < textToGenerate.Length; i++) {
            char currentCharacter = textToGenerate[i];
            if (currentCharacter == ' ') {
                xPosition += (letterSpacing * wordSpacing);
                continue;
            }
            CharData currentCharacterData = loadedFont[currentCharacter];

            xPosition += currentCharacterData.LeftOffset * letterSpacing;

            //Create new game object

            Debug.Log("Letter Number: " + i + ", xpos: " + xPosition + ", ypos: " + yPosition);
            GameObject newLetterSprite = CreateNewLetter(currentCharacterData, xPosition, yPosition, i);
            letterObjects.Add(newLetterSprite);


            xPosition += currentCharacterData.RightOffset * letterSpacing;
        }
    }

    private GameObject CreateNewLetter(CharData newCharacter, float positionX, float positionY, int letterNumber) {

        //Create new game object
        GameObject newLetterSprite = Instantiate(letterObject, transform);
        RectTransform newLetterTransform = newLetterSprite.GetComponent<RectTransform>();
        newLetterSprite.name = "lettersprite_" + letterNumber;
        newLetterTransform.localPosition += new Vector3(positionX, positionY, 0f);
        Image newLetterImage = newLetterSprite.GetComponent<Image>();
        newLetterImage.sprite = newCharacter.Sprite;

        return newLetterSprite;
    }

    private void CheckTag(string fullText, char c, int j, ref bool inTag) {
        if (c == '<') {
            inTag = true;

            char next = fullText[j + 1];

            if (next != '/') {
                switch (next) {
                    case 'w': activeEffect = TextEffect.Wavy; break;
                    case 's': activeEffect = TextEffect.Shaky; break;
                }
            } else {
                activeEffect = TextEffect.None;
            }
        } else if (j > 0 && fullText[j - 1] == '>') {
            inTag = false;
        }
    }

    #region Text Patterns
    IEnumerator TextFade(float timer) {
        var count = 0f;

        while (timer > 0) {
            for (int i = 0; i < letterObjects.Count; i++) {

                var obj = letterObjects[i].transform;
                var pos = obj.position;

                Color tmp = obj.GetComponent<Image>().color;
                tmp.a = Mathf.Sin(count);
                obj.GetComponent<Image>().color = tmp;

                obj.position = pos;
                yield return default;
                timer -= Time.deltaTime;
                Debug.Log(timer);
                count += 0.1f;
            }
        }
    }

    void TextFade() {
        var count = 0f;
        for (int i = 0; i < letterObjects.Count; i++) {
            var obj = letterObjects[i].transform;
            var pos = obj.position;

            Color tmp = obj.GetComponent<Image>().color;
            tmp.a = Mathf.Sin(count + Time.time);
            obj.GetComponent<Image>().color = tmp;

            obj.position = pos;
            count += 1f / letterObjects.Count;
        }
    }

    void TextShaky() {
        foreach (var letterObj in letterObjects) {
            RectTransform rectTransform = letterObj.GetComponent<RectTransform>();
            Vector2 position = rectTransform.position;
            rectTransform.anchoredPosition = position + Random.insideUnitCircle * 0.8f;
        }
    }

    void TextWavy() {
        foreach (var letterObj in letterObjects) {
            RectTransform rectTransform = letterObj.GetComponent<RectTransform>();
            Vector2 position = rectTransform.position;
            rectTransform.anchoredPosition += Vector2.up * Mathf.Sin(position.x * 0.1f + 10 * Time.time) * 0.4f;
        }
    }
    #endregion
}