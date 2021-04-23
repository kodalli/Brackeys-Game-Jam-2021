using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour {
    [SerializeField] private GameObject dialoguePanel;
    private SpriteLetterSystem SPL;

    private void Start() {
        SPL = dialoguePanel.GetComponentInChildren<SpriteLetterSystem>();
        PlayerPrefs.DeleteAll();
        PointOfInterest.OnPoiEntered += OnPoiEnteredNotification;
    }
    private void OnDestroy() => PointOfInterest.OnPoiEntered -= OnPoiEnteredNotification;

    private void OnPoiEnteredNotification(PointOfInterest poi) {
        string achievementKey = "achievement-" + poi.PoiName;

        if (PlayerPrefs.GetInt(achievementKey) == 1) return;
        PlayerPrefs.SetInt(achievementKey, 1);

        dialoguePanel.SetActive(true);
        SPL.LetterSize = 65;
        SPL.LetterSpacing = 15;
        SPL.GenerateSpriteText($"Unlocked: <c=(255,50,120)><w>{poi.PoiName}</w></c>");
        StartCoroutine(RemoveDialoguePanel());
    }

    IEnumerator RemoveDialoguePanel() {
        yield return new WaitForSeconds(2f);
        dialoguePanel.SetActive(false);
    }

}
