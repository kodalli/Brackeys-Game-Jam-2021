using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FontLoader {
    //This is the order that the characters should be in the characterSheet
    // private static char[] chars = "abcdefghijklmnopqrstuvwxyzæABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~><!?'\"#%&/\\()[]{}@£$*^+-.,:;_=".ToCharArray();
    // private static char[] chars = " !\"#$%&'()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[\\]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~".ToCharArray();
    private static char[] chars = "!\"#$%&'()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[\\]^_ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~".ToCharArray();
    private static List<Dictionary<char, CharData>> loadedFonts = new List<Dictionary<char, CharData>>(); //Stores all loaded fonts
    private static List<Texture2D> loadedFontResources = new List<Texture2D>(); //This is to keep track of all the loaded fonts and only load them once

    /// <summary>
    /// Property with dictonary of font characters and char data
    /// </summary>
    public static List<Dictionary<char, CharData>> LoadedFonts {
        get { return loadedFonts; }
    }

    /// <summary>
    /// Loads font resources from character sheet array into dictionary
    /// </summary>
    public static List<Dictionary<char, CharData>> LoadFontResources(params Texture2D[] characterSheet) {
        return characterSheet != null && characterSheet.Any() ?
            characterSheet.Select(x => LoadFontResource(x)).ToList() :
            new List<Dictionary<char, CharData>>();
    }

    /// <summary>
    /// Reload font resources from character sheet array into dictionary
    /// </summary>
    public static List<Dictionary<char, CharData>> ReloadFontResources(params Texture2D[] characterSheet) {
        return characterSheet != null && characterSheet.Any() ?
            characterSheet.Select(x => ReloadFontResource(x)).ToList() :
            new List<Dictionary<char, CharData>>();
    }

    /// <summary>
    /// Loads font resources from character sheet into dictionary
    /// </summary>
    public static Dictionary<char, CharData> LoadFontResource(Texture2D characterSheet) => LoadFontResource(characterSheet, true);

    /// <summary>
    /// Loads font resource from character sheet and adds it to existing loaded font if applicable
    /// </summary>
    private static Dictionary<char, CharData> LoadFontResource(Texture2D characterSheet, bool addToLoaded) {
        //If we already have this loaded then we just return the loaded one
        if (IsFontLoaded(characterSheet)) return loadedFonts[loadedFontResources.IndexOf(characterSheet)];

        Sprite[] subsprites = Resources.LoadAll<Sprite>(characterSheet.name);
        int spriteSize = (int)subsprites.Max(x => x.rect.width);
        // int spriteSize = (int)subsprites[0].rect.width; //characterSheet.width / subsprites.Length; //OLD

        // Debug.Log(subsprites.Length);
        // Debug.Log(chars.Length);

        Dictionary<char, CharData> loadedFontDictionary = GenerateCharFontDictionary(characterSheet, spriteSize, subsprites);


        if (addToLoaded) {
            loadedFonts.Add(loadedFontDictionary);
            loadedFontResources.Add(characterSheet);
        }

        return loadedFontDictionary;
    }

    /// <summary>
    /// Reloads font resource from character sheet into dictionary
    /// </summary>
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

    /// <summary>
    /// Checks if loaded font resources contains the character sheet
    /// </summary>
    public static bool IsFontLoaded(Texture2D characterSheet) => loadedFontResources.Contains(characterSheet);

    /// <summary>
    /// Generates font dictionary by performing a vertical scan on each font sprite and stores character width
    /// </summary>
    private static Dictionary<char, CharData> GenerateCharFontDictionary(Texture2D characterSheet, int spriteSize, Sprite[] characterSprites) {
        int height = characterSheet.height; // We might need this if we ever use a text image that is on more than one line
        int width = characterSheet.width;

        int charIndex = 0;

        Dictionary<char, CharData> charData = new Dictionary<char, CharData>();

        // Perform vertical scan on each sprite to find the widths

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
                // int currentSpriteWidth = Mathf.Max(spriteSize - (leftEdge + rightEdge), 1);
                int currentSpriteWidth = spriteSize - (leftEdge + rightEdge);

                if (currentSpriteWidth < 0) {
                    Debug.Log($"{chars[charIndex]} width {currentSpriteWidth} {spriteSize} {leftEdge} {rightEdge}");
                    // vape fix, just manually set width for "!" and "B" because its setting the width to < 0
                    // CharData temp = charData['A'];
                    charData.Add(chars[charIndex], new CharData(14, characterSprites[charIndex], 3, 3));
                } else {
                    //Determine center offsets
                    int halfWidth = spriteSize / 2;
                    int leftOffset = halfWidth - leftEdge;
                    int rightOffset = halfWidth - rightEdge;

                    charData.Add(chars[charIndex], new CharData(currentSpriteWidth, characterSprites[charIndex], leftOffset, rightOffset));
                }

                Debug.Log($"{chars[charIndex]} width {currentSpriteWidth} {spriteSize} {leftEdge} {rightEdge}");

                // //Determine center offsets
                // int halfWidth = spriteSize / 2;
                // int leftOffset = halfWidth - leftEdge;
                // int rightOffset = halfWidth - rightEdge;

                // charData.Add(chars[charIndex], new CharData(currentSpriteWidth, characterSprites[charIndex], leftOffset, rightOffset));

                // fix for first sprite being empty in sprite sheet
                // if (charIndex > 0 && charIndex < chars.Length)
                //     charData.Add(chars[charIndex], new CharData(currentSpriteWidth, characterSprites[charIndex - 1], leftOffset, rightOffset));

                charIndex++;
            }
        }
        return charData;
    }
}

/// <summary>
/// Struct holding char data of width, left offset, right offset, and sprite data
/// </summary>
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