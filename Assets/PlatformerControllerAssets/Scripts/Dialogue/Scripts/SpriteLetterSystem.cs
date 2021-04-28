using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using StaticUtils;

//Attatch this to something, ideally something in a unity canvas
public class SpriteLetterSystem : MonoBehaviour {
    private enum TextEffect { None, Wavy, Shaky }

    #region Variables
    // public
    [SerializeField] private Texture2D charSheet; //This should be the resource you want to load, Make sure that the image is in Assets/Resources ideally. Its a folder that unity can load things from using Resources.LoadAll etc
    [SerializeField] private GameObject letterObject;
    [SerializeField] private DialogueObject dObj;
    [SerializeField] private RectTransform dialogueBoxRT;
    // 
    [Header("Text Settings")]
    [SerializeField] private float letterSpacing = 3.5f;
    [SerializeField] private float wordSpacing = 10f;
    [SerializeField] private float lineSpacing = 100f;
    [SerializeField] private float letterSize = 10f;

    public float LetterSize { set { letterSize = value; } }
    public float LetterSpacing { set { letterSpacing = value; } }

    //
    [Header("Text Position In Dialogue Box")]
    [SerializeField] private float indentLeft = 20f;
    [SerializeField] private float indentRight = 20f;
    [SerializeField] private float indentTop = -20f;
    [SerializeField] private float indentBottom = 0f;
    // 
    [Header("Text Effects")]
    [SerializeField] private float wavyStrength = 0.5f;
    [SerializeField] private float shakyStrength = 0.5f;
    // private
    private TextEffect activeEffect;
    private Color activeColor = Color.black;
    private List<GameObject> letterObjects = new List<GameObject>();
    private Dictionary<CharSpriteData, TextEffect> fxChars = new Dictionary<CharSpriteData, TextEffect>();
    private Dictionary<char, CharData> loadedFont;
    #endregion

    public void GenerateBigText(string text) {
        letterSpacing = 6f;
        wordSpacing = 10f;
        lineSpacing = 110f;
        letterSize = 80f;
        indentLeft = 50f;
        indentRight = 50f;
        indentTop = -120f;
        indentBottom = 0f;
        GenerateSpriteText(text);
    }

    public void GenerateSmallText(string text) {
        letterSpacing = 4f;
        wordSpacing = 10f;
        lineSpacing = 70f;
        letterSize = 50f;
        indentLeft = 40f;
        indentRight = 40f;
        indentTop = -60f;
        indentBottom = 0f;
        GenerateSpriteText(text);
    }


    /// <summary>
    /// Container for character sprite data
    /// </summary>
    private struct CharSpriteData {
        public Transform transform; // gameobject component
        public Vector2 position { get { return this.transform.position; } }
        public Image image; // gameobject component
        public Color color { set { this.image.color = value; } }
        public RectTransform rectTransform; // gameobject component
        public float rightOffset;
        public float leftOffset;

    }

#if UNITY_EDITOR
    private void OnGUI() {
        if (GUI.Button(new Rect(0, 0, 200, 50), "Generate Big Text")) {
            GenerateBigText(dObj.dialogue[0]);
        } else if (GUI.Button(new Rect(0, 50, 200, 50), "Generate Small Text")) {
            GenerateSmallText(dObj.dialogue[0]);
        } else if (GUI.Button(new Rect(0, 300, 200, 50), "Generate Text")) {
            GenerateSpriteText(dObj.dialogue[0]);
        }
    }
#endif

    private void Awake() {
        loadedFont = FontLoader.LoadFontResource(charSheet);
    }

    // private void Start() {
    //     // string textToGenerate = "<c=(255,50,120)><w>I am a top level Chungoloist</w></c>, and I have concluded with <c=(255,0,0)>absolute</c> <c=(0,255,0)>certainty </c>that Big Chungus himself shall enter into existence at 2:31 PM this April 9th.";
    //     string textToGenerate = dObj.dialogue[0];
    //     GenerateSpriteText(textToGenerate);
    //     // transform.localScale = new Vector3(letterSize / 100f, letterSize / 100f, 1f);
    //     // dialogueBox.SetActive(false);

    // }



    private void FixedUpdate() {
        DoTextEffects();
    }

    /// <summary>
    /// Generates characters sprites from string and applies text effects
    /// </summary>
    public void GenerateSpriteText(string textToGenerate) {
        // GetComponentsInChildren<RectTransform>().ToList().ForEach(x => x.gameObject.SetActive(false));
        // gameObject.SetActive(true);

        gameObject.SetActiveAllChildren<RectTransform>(false);

        // textToGenerate = textToGenerate.ToLower();

        if (letterObject == null) return;

        // The sprite text generator object should be place at the top left corner of the text box

        float scale = letterSize / 100f;

        Rect dBoxRect = dialogueBoxRT.rect;

        float xPosition = indentLeft;
        float yPosition = indentTop;

        bool inTag = false;

        int wordCount = 0;

        List<CharSpriteData> wordList = new List<CharSpriteData>();

        for (int i = 0; i < textToGenerate.Length; i++) {

            CheckTag(textToGenerate, textToGenerate[i], i, ref inTag);

            // Debug.Log($"{inTag} {activeEffect} {textToGenerate[i]}");

            // Out of dialogue box bounds Y
            if (yPosition < -dBoxRect.height + indentBottom) {
                continue;
            }

            if (!inTag) {
                char currentCharacter = textToGenerate[i];
                if (currentCharacter == ' ') {
                    xPosition += (letterSpacing * wordSpacing);
                    wordCount++;
                    wordList.Clear();
                    continue;
                }
                CharData currentCharacterData = loadedFont[currentCharacter];

                xPosition += currentCharacterData.LeftOffset * letterSpacing;

                // Create new game object

                // Debug.Log("Letter Number: " + i + ", xpos: " + xPosition + ", ypos: " + yPosition);
                GameObject newLetterSprite = CreateNewLetter(currentCharacterData, xPosition, yPosition, i);
                newLetterSprite.transform.localScale = new Vector3(scale, scale, 1f);

                letterObjects.Add(newLetterSprite);

                CharSpriteData charData = new CharSpriteData();

                charData.transform = newLetterSprite.transform;
                charData.rectTransform = newLetterSprite.GetComponent<RectTransform>();
                charData.image = newLetterSprite.GetComponent<Image>();
                charData.rightOffset = currentCharacterData.RightOffset;
                charData.leftOffset = currentCharacterData.LeftOffset;

                wordList.Add(charData);

                // set active color here so we can wrap other effects in color tags
                charData.color = activeColor;

                fxChars.Add(charData, activeEffect);

                xPosition += currentCharacterData.RightOffset * letterSpacing * scale;

                // if (xPosition >= dBoxRect.width - indentRight) {
                //     yPosition -= lineSpacing;
                //     xPosition = indentLeft;
                // }

                if (xPosition >= dBoxRect.width - indentRight) {
                    yPosition -= lineSpacing; // go to next line
                    xPosition = indentLeft; // reset x position

                    // puts characters of unfinished word on the next line
                    if (wordList.Count > 0) {
                        for (int j = 0; j < wordList.Count; j++) {
                            var letterData = wordList[j];
                            // letterData.transform = transform;
                            xPosition += letterData.leftOffset * letterSpacing;
                            letterData.transform.localPosition = new Vector3(xPosition, yPosition, 1f);
                            xPosition += letterData.rightOffset * letterSpacing * scale;

                            if (yPosition < -dBoxRect.height) letterData.transform.gameObject.SetActive(false);
                        }
                    }
                }
                // Debug.Log(charData.transform.localPosition);
            }
        }
        // Debug.Log(wordCount);
        // Debug.Log($"width: {dBoxRect.width}, height: {dBoxRect.height}");
    }

    /// <summary>
    /// Creates new letter sprite and displays on canvas
    /// </summary>
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
                activeColor = Color.black;
            }
        } else if (j > 0 && fullText[j - 1] == '>') {
            inTag = false;
        }
    }

    /// <summary>
    /// Sets active color to value specified in element tag
    /// </summary>
    private void SetColorFromText(string fullText, int start) {
        // c=( 256, 256, 256)

        int end = start;
        int length = 0;

        while (end < fullText.Length && fullText[end] != ')') {
            end++;
            length++;
        }

        // 256, 256, 256
        string str = fullText.Substring(start, length);

        // [256, 256, 256]
        string[] strSplit = str.Split(',');

        // Debug.Log(strSplit.Length + " " + fullText + " " + str);

        if (strSplit.Length != 3 && end != fullText.Length) Debug.LogError("color must follow <c=(0-256,0-256,0-256)></c> format");

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
                    rectTransform.anchoredPosition += Vector2.up * Mathf.Sin(position.x * 0.1f + 10 * Time.time) * wavyStrength;
                    break;
                case TextEffect.Shaky:
                    rectTransform.anchoredPosition = position + Random.insideUnitCircle * shakyStrength;
                    break;
            }
        }
    }

}