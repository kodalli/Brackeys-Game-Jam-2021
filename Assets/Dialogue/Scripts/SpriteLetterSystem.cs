using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Attatch this to something, ideally something in a unity canvas
public class SpriteLetterSystem : MonoBehaviour {
    public Texture2D charSheet; //This should be the resource you want to load, Make sure that the image is in Assets/Resources ideally. Its a folder that unity can load things from using Resources.LoadAll etc
    public Dictionary<char, CharData> loadedFont;
    public GameObject letterObject;
    public float letterSpacing = 3.5f;
    public float lineSpacing = 100f;

    private List<GameObject> letterObjects = new List<GameObject>();
    private void Awake() {
        loadedFont = FontLoader.LoadFontResource(charSheet);
    }

    private void Start() {
        GenerateSpriteText("Put you're text in here and it should create it");
    }

    public void GenerateSpriteText(string textToGenerate) {
        if (letterObject == null) return;


        float xPosition = 0;
        float yPosition = 0;

        for (int i = 0; i < textToGenerate.Length; i++) {
            char currentCharacter = textToGenerate[i];
            if (currentCharacter == ' ') {
                xPosition += (letterSpacing * 10f);
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
}