using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//Attatch this to something, ideally something in a unity canvas
public class SpriteLetterSystem : MonoBehaviour {
    private enum TextEffect { None, Wavy, Shaky }

    #region Variables
    // public
    public Texture2D charSheet; //This should be the resource you want to load, Make sure that the image is in Assets/Resources ideally. Its a folder that unity can load things from using Resources.LoadAll etc
    public Dictionary<char, CharData> loadedFont;
    public GameObject letterObject;
    public float letterSpacing = 3.5f;
    public float wordSpacing = 10f;
    public float lineSpacing = 100f;
    public float letterSize = 10f;
    // private
    private TextEffect activeEffect;
    private Color activeColor = Color.white;
    private List<GameObject> letterObjects = new List<GameObject>();
    private Dictionary<CharSpriteData, TextEffect> fxChars = new Dictionary<CharSpriteData, TextEffect>();
    #endregion

    /// <summary>
    /// Container for character sprite data
    /// </summary>
    private struct CharSpriteData {
        public Transform transform; // gameobject component
        public Vector2 position { get { return this.transform.position; } }
        public Image image; // gameobject component
        public Color color { set { this.image.color = value; } }
        public RectTransform rectTransform; // gameobject component

    }

    private void Awake() {
        loadedFont = FontLoader.LoadFontResource(charSheet);
    }

    private void Start() {
        string textToGenerate = "<c=(255,50,120)><w>I am a top level Chungoloist</w></c>, and I have concluded with <c=(255,0,0)>absolute</c> <c=(0,255,0)>certainty </c>that Big Chungus himself shall enter into existence at 2:31 PM this April 9th.";
        GenerateSpriteText(textToGenerate);
        transform.localScale = new Vector3(letterSize / 100f, letterSize / 100f, 1f);
    }

    private void FixedUpdate() {
        DoTextEffects();
    }

    /// <summary>
    /// Generates characters sprites from string and applies text effects
    /// </summary>
    /// <param name="textToGenerate"></param>
    public void GenerateSpriteText(string textToGenerate) {
        if (letterObject == null) return;


        float xPosition = 0;
        float yPosition = 0;

        bool inTag = false;

        for (int i = 0; i < textToGenerate.Length; i++) {

            CheckTag(textToGenerate, textToGenerate[i], i, ref inTag);

            // Debug.Log($"{inTag} {activeEffect} {textToGenerate[i]}");

            if (!inTag) {
                char currentCharacter = textToGenerate[i];
                if (currentCharacter == ' ') {
                    xPosition += (letterSpacing * wordSpacing);
                    continue;
                }
                CharData currentCharacterData = loadedFont[currentCharacter];

                xPosition += currentCharacterData.LeftOffset * letterSpacing;

                // Create new game object

                // Debug.Log("Letter Number: " + i + ", xpos: " + xPosition + ", ypos: " + yPosition);
                GameObject newLetterSprite = CreateNewLetter(currentCharacterData, xPosition, yPosition, i);
                letterObjects.Add(newLetterSprite);

                CharSpriteData charData = new CharSpriteData();

                charData.transform = newLetterSprite.transform;
                charData.rectTransform = newLetterSprite.GetComponent<RectTransform>();
                charData.image = newLetterSprite.GetComponent<Image>();

                charData.color = activeColor;

                fxChars.Add(charData, activeEffect);

                xPosition += currentCharacterData.RightOffset * letterSpacing;
            }
        }
    }

    /// <summary>
    /// Creates new letter sprite and displays on canvas
    /// </summary>
    /// <param name="newCharacter"></param>
    /// <param name="positionX"></param>
    /// <param name="positionY"></param>
    /// <param name="letterNumber"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Applies tag effects and checks if current char is in tagged element
    /// </summary>
    /// <param name="fullText"></param>
    /// <param name="c"></param>
    /// <param name="j"></param>
    /// <param name="inTag"></param>
    private void CheckTag(string fullText, char c, int j, ref bool inTag) {
        if (c == '<') {
            inTag = true;

            char next = fullText[j + 1];

            if (next != '/') {
                switch (next) {
                    case 'w': activeEffect = TextEffect.Wavy; break;
                    case 's': activeEffect = TextEffect.Shaky; break;
                    case 'c': SetColorFromText(fullText, j + 4); break;
                }
            } else {
                activeEffect = TextEffect.None;
                activeColor = Color.white;
            }
        } else if (j > 0 && fullText[j - 1] == '>') {
            inTag = false;
        }
    }

    /// <summary>
    /// Sets active color to value specified in element tag
    /// </summary>
    /// <param name="fullText"></param>
    /// <param name="start"></param>
    private void SetColorFromText(string fullText, int start) {
        // c=( 256, 256, 256)

        int end = start;
        int length = 0;

        while (fullText[end] != ')' && end < fullText.Length) {
            end++;
            length++;
        }

        // 256, 256, 256
        string str = fullText.Substring(start, length);

        // [256, 256, 256]
        string[] strSplit = str.Split(',');
        if (strSplit.Length != 3) Debug.LogError("color must follow c=(0-256,0-256,0-256) format");

        int[] colorParams = strSplit.Select(int.Parse).ToArray();

        activeColor.r = colorParams[0] / 255f;
        activeColor.g = colorParams[1] / 255f;
        activeColor.b = colorParams[2] / 255f;
        activeColor.a = 1;
    }

    /// <summary>
    /// Applies text effects to each text object in the ui canvas
    /// </summary>
    private void DoTextEffects() {
        foreach (CharSpriteData charData in fxChars.Keys) {
            var effect = fxChars[charData];
            var rectTransform = charData.rectTransform;
            var position = charData.position;

            switch (effect) {
                case TextEffect.Wavy:
                    rectTransform.anchoredPosition += Vector2.up * Mathf.Sin(position.x * 0.1f + 10 * Time.time) * 1f;
                    break;
                case TextEffect.Shaky:
                    rectTransform.anchoredPosition = position + Random.insideUnitCircle * 0.8f;
                    break;
            }
        }
    }
}