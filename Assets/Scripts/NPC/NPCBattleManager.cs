using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBattleManager : MonoBehaviour {
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private List<string> prefightDialogue;
    [SerializeField] private List<string> postfightDialogue;

    private GameObject keyObj;

    private void Start() {
        StartCoroutine(CheckDefeated());
    }

    public void EnableKey() {
        Vector3 pos = transform.position;
        pos.y += 1f; // hover above
        keyObj = Instantiate(keyPrefab, pos, Quaternion.identity);
    }

    public void DisableKey() {
        Dialog.Instance.dialogBox.SetActive(false);
        Destroy(keyObj);
    }

    public void OnInteractKey() {
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

    IEnumerator StartBattle() {
        while (!Dialog.Instance.IsDialogueOver()) yield return default;
        var enemy = new NPCBattleWrapper(GetComponent<BattleNPC>());
        PlayerControlSave.Instance.localPlayerData.enemyData = enemy;
        PlayerController.Instance.PlayerSceneLoad("Battle Scene");
    }

    IEnumerator DisableMovementUntilDialogueEnds() {
        PlayerController.Instance.movementIsActive = false;
        while (!Dialog.Instance.IsDialogueOver()) yield return default;
        PlayerController.Instance.movementIsActive = true;
    }

    IEnumerator CheckDefeated() {
        var countDown = 1f;
        while (countDown > 0) {
            if (GetComponent<BattleNPC>().State.Equals(NPCStatus.Defeated)) {
                GetComponent<NPCPath>().WalkThePath();
                Dialog.Instance.SkipDialogue();
                yield break;
            }
            yield return default;
            countDown -= Time.deltaTime;
        }
    }
}
