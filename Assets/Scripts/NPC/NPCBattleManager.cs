using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NPCBattleManager : MonoBehaviour {
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private List<string> prefightDialogue;
    [SerializeField] private List<string> postfightDialogue;
    [SerializeField] private Transform target;
    NavMeshAgent agent;

    private GameObject keyObj;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // StartCoroutine(CheckDefeated());
    }

    private void Update() {

        if (target != null)
            agent.SetDestination(target.position);

        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(CheckDefeated());
        }
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
        var npcPath = GetComponent<NPCPath>();

        // if no NPCPath script then return
        if (npcPath == null) yield break;

        var countDown = 1f;
        while (countDown > 0) {
            var dict = PlayerControlSave.Instance.localPlayerData.enemyPathCounter;
            var counter = 0;
            var pos = transform.position;
            var name = GetComponent<BattleNPC>().Name;
            // check if the npc has walked any path yet
            if (dict.ContainsKey(name)) {
                // get the next path to walk
                var pair = dict[name];
                counter = pair.Item1;
                pos = pair.Item2;
                transform.position = pos;
            } else {
                // add npc to dictionary and start counter for paths to walk at 0
                Tuple<int, Vector3> pair = new Tuple<int, Vector3>(0, pos);
                dict.Add(name, pair);
            }
            if (GetComponent<BattleNPC>().State.Equals(NPCStatus.Defeated)) {
                //***TODO handle list of npc paths***
                Debug.Log(counter);
                npcPath.WalkThePath(counter);
                Dialog.Instance.SkipDialogue();
                //increment to next path for the npc

               yield break;
            }
            yield return default;
            countDown -= Time.deltaTime;
        }
    }

    private void WalkNextPath() {
        var npcPath = GetComponent<NPCPath>();

        // if no NPCPath script then return
        if (npcPath == null) return;

        var dict = PlayerControlSave.Instance.localPlayerData.enemyPathCounter;
        var counter = 0;
        var pos = transform.position;
        var name = GetComponent<BattleNPC>().Name;

        // check if the npc has walked any path yet
        if (dict.ContainsKey(name)) {
            // get the next path to walk
            var pair = dict[name];
            counter = pair.Item1;
            pos = pair.Item2;
            transform.position = pos;
        }
        else {
            // add npc to dictionary and start counter for paths to walk at 0
            Tuple<int, Vector3> pair = new Tuple<int, Vector3>(0, pos);
            dict.Add(name, pair);
        }

        Debug.Log(counter);
        npcPath.WalkThePath(counter);
        Dialog.Instance.SkipDialogue();

        //increment to next path for the npc
    }
} 
