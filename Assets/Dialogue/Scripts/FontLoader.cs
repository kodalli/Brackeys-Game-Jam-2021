using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FontLoader {
    //This is the order that the characters should be in the characterSheet
    private static char[] chars = "abcdefghijklmnopqrstuvwxyzæABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~><!?'\"#%&/\\()[]{}@£$*^+-.,:;_=".ToCharArray();
    private static List<Dictionary<char, CharData>> loadedFonts = new List<Dictionary<char, CharData>>(); //Stores all loaded fonts
    private static List<Texture2D> loadedFontResources = new List<Texture2D>(); //This is to keep track of all the loaded fonts and only load them once

    public static List<Dictionary<char, CharData>> LoadedFonts {
        get { return loadedFonts; }
    }

    public static List<Dictionary<char, CharData>> LoadFontResources(params Texture2D[] characterSheet) {
        return characterSheet != null && characterSheet.Any() ?
            characterSheet.Select(x => LoadFontResource(x)).ToList() :
            new List<Dictionary<char, CharData>>();
    }
    public static List<Dictionary<char, CharData>> ReloadFontResources(params Texture2D[] characterSheet) {
        return characterSheet != null && characterSheet.Any() ?
            characterSheet.Select(x => ReloadFontResource(x)).ToList() :
            new List<Dictionary<char, CharData>>();
    }


    public static Dictionary<char, CharData> LoadFontResource(Texture2D characterSheet) => LoadFontResource(characterSheet, true);

    private static Dictionary<char, CharData> LoadFontResource(Texture2D characterSheet, bool addToLoaded) {
        //If we already have this loaded then we just return the loaded one
        if (IsFontLoaded(characterSheet)) return loadedFonts[loadedFontResources.IndexOf(characterSheet)];

        Sprite[] subsprites = Resources.LoadAll<Sprite>(characterSheet.name);
        int spriteSize = (int)subsprites[0].rect.width; //characterSheet.width / subsprites.Length; //OLD

        Dictionary<char, CharData> loadedFontDictionary = GenerateCharFontDictionary(characterSheet, spriteSize, subsprites);

        if (addToLoaded) {
            loadedFonts.Add(loadedFontDictionary);
            loadedFontResources.Add(characterSheet);
        }

        return loadedFontDictionary;
    }

    public static Dictionary<char, CharData> ReloadFontResource(Texture2D characterSheet) {
        if (IsFontLoaded(characterSheet)) {
            Dictionary<char, CharData> loadedFontResource = LoadFontResource(characterSheet, false);
            loadedFonts[loadedFontResources.IndexOf(characterSheet)] = loadedFontResource;
            return loadedFontResource;
        } else {
            Debug.Log("Font in Texture2D: " + characterSheet.name + " hasn't previously been loaded, Loading normally. Please use LoadFontResource/LoadFontResources if this behaviour is not desired.");
            return LoadFontResource(characterSheet);
        }
    }

    public static bool IsFontLoaded(Texture2D characterSheet) => loadedFontResources.Contains(characterSheet);

    private static Dictionary<char, CharData> GenerateCharFontDictionary(Texture2D characterSheet, int spriteSize, Sprite[] characterSprites) {
        int height = characterSheet.height; // We might need this if we ever use a text image that is on more than one line
        int width = characterSheet.width;

        int charIndex = 0;

        Dictionary<char, CharData> charData = new Dictionary<char, CharData>();

        //Y Texture Coordinate
        for (int texCoordY = height - spriteSize; texCoordY >= 0 && charIndex < chars.Length; texCoordY -= spriteSize) {
            int minY = texCoordY;
            int maxY = texCoordY + spriteSize;

            //X Texture Coordinate
            for (int texCoordX = 0; texCoordX < width && charIndex < chars.Length; texCoordX += spriteSize) {
                int minX = texCoordX;
                int maxX = texCoordX + (spriteSize - 1);
                bool edgeFound = false;

                //right edge
                int rightEdge = 0;
                for (int currentX = maxX; currentX >= minX; currentX--) {
                    for (int currentY = minY; currentY < maxY; currentY++) {
                        edgeFound = characterSheet.GetPixel(currentX, currentY).a != 0;
                        if (edgeFound) break;
                    }
                    if (edgeFound) break;
                    rightEdge++;
                }

                edgeFound = false;


                //left edge
                int leftEdge = 0;
                for (int currentX = minX; currentX <= maxX; currentX++) {
                    //X
                    for (int currentY = minY; currentY < maxY; currentY++) {
                        edgeFound = characterSheet.GetPixel(currentX, currentY).a != 0;
                        if (edgeFound) break;
                    }
                    if (edgeFound) break;
                    leftEdge++;
                }

                //Store current sprite width
                int currentSpriteWidth = spriteSize - (leftEdge + rightEdge);

                //Determine center offsets
                int halfWidth = spriteSize / 2;
                int leftOffset = halfWidth - leftEdge;
                int rightOffset = halfWidth - rightEdge;

                charData.Add(chars[charIndex], new CharData(currentSpriteWidth, characterSprites[charIndex], leftOffset, rightOffset));

                charIndex++;
            }
        }
        return charData;
    }
}

public struct CharData {
    public int Width;
    public int LeftOffset;
    public int RightOffset;

    public Sprite Sprite;

    public CharData(int width, Sprite sprite, int leftOffset, int rightOffset) {
        Width = width;
        Sprite = sprite;
        LeftOffset = leftOffset;
        RightOffset = rightOffset;
    }
}