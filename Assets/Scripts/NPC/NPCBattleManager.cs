using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBattleManager : MonoBehaviour {
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private List<string> prefightDialogue;
    [SerializeField] private List<string> postfightDialogue;

    public Transform postFightLocation;

    private bool isNear = false;
    private GameObject keyObj;

    private void Start() {
        if(GetComponent<BattleNPC>().State == NPCStatus.Defeated && postFightLocation != null) {
            gameObject.transform.position = postFightLocation.position;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && isNear) {

            if (Dialog.Instance.dialogBox.activeSelf) {
                return;
            }

            // check if npc is defeated, if defeated display postfight dialogue
            if (GetComponent<BattleNPC>().State == NPCStatus.Defeated) {
                Dialog.Instance.DisplayTextInDialogueBox(postfightDialogue);
                StartCoroutine(DisableMovementUntilDialogueEnds());

            }
            else if (!PlayerController.Instance.CanBattle()) {  // check if can battle
                List<string> textList = new List<string>() { "All your homies have fainted." };
                Dialog.Instance.DisplayTextInDialogueBox(textList);
                return;
            }
            else {  // display prefight dialogue, then start battle
                Dialog.Instance.DisplayTextInDialogueBox(prefightDialogue);
                StartCoroutine(DisableMovementUntilDialogueEnds());
                StartCoroutine(StartBattle());
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            isNear = true;
            Vector3 pos = transform.position;
            pos.y += 1f; // hover above
            keyObj = Instantiate(keyPrefab, pos, Quaternion.identity);

        }

    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            isNear = false;
            //dialogBox.SetActive(false);
            Destroy(keyObj);
        }
    }

    IEnumerator StartBattle() {

        while (!Dialog.Instance.IsDialogueOver()) {
            yield return default;
        }

        var enemy = new NPCBattleWrapper(GetComponent<BattleNPC>());

        PlayerControlSave.Instance.localPlayerData.enemyData = enemy;

        PlayerController.Instance.PlayerSceneLoad("Battle Scene");
    }

    IEnumerator DisableMovementUntilDialogueEnds() {
        PlayerController.Instance.movementIsActive = false;

        while (!Dialog.Instance.IsDialogueOver() && !PlayerController.Instance.movementIsActive) {
            //Debug.Log("");
            yield return default;
        }

        PlayerController.Instance.movementIsActive = true;
    }
}
