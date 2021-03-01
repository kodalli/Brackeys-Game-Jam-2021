using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TimelineUtils;

public enum GameMode {
    Gameplay,
    DialogueMoment, //waiting for input
}

public class PlayerController : Singleton<PlayerController> {

    #region Variables
    public bool movementIsActive = true;
    public Vector3 DeltaPosition { get; private set; }
    public Dictionary<string, GameObject> npcSquad = new Dictionary<string, GameObject>();

    private bool isNear;
    private GameObject interactObj;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 lastPos;
    
    private readonly int moveX = Animator.StringToHash("moveX");
    private readonly int moveY = Animator.StringToHash("moveY");
    private readonly int isMoving = Animator.StringToHash("isMoving");
    #endregion

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastPos = transform.position;
    }
    private void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        switch (PlayerControlSave.Instance.localPlayerData.currentGameMode) {
            case GameMode.Gameplay:
                // Movement
                if (movement != Vector2.zero && movementIsActive) Move();
                else if (movement == Vector2.zero || !movementIsActive) anim.SetBool(isMoving, false);

                // Dialogue box
                if (Dialog.Instance != null && Input.GetKeyDown(KeyCode.Escape)) Dialog.Instance.SkipDialogue();

                // Interact
                if (Dialog.Instance != null && !Dialog.Instance.IsDialogueOver() && Input.GetKeyDown(KeyCode.E))
                    Dialog.Instance.NextSentence();
                else if (Input.GetKeyDown(KeyCode.E)) {
                    // Interact with sign or npc, or obj
                    if (interactObj == null) { }
                    else if (isNear && interactObj.CompareTag("Sign"))
                        interactObj.GetComponent<Sign>().OnInteractKey();
                    else if (isNear && interactObj.CompareTag("BattleNPC"))
                        interactObj.GetComponent<NPCBattleManager>().OnInteractKey();
                    else if (isNear && interactObj.CompareTag("ItemPickup"))
                        interactObj.GetComponent<ItemPickUp>().AddToInventory();
                }

                // Toggle menus
                if (Input.GetKeyDown(KeyCode.Q)) {
                    Dialog.Instance.ToggleItemMenu();
                }
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    Dialog.Instance.ToggleSquadMenu();
                }
                break;

            case GameMode.DialogueMoment:
                anim.SetBool(isMoving, false);
                if (Input.GetKeyDown(KeyCode.Space))
                    TimelineController.Instance.ResumeTimeline();
                break;
        }
    }

    private void FixedUpdate() {
        // npc follow player logic
        DeltaPosition = transform.position - lastPos;
        lastPos = transform.position;
        foreach(KeyValuePair<string, GameObject> npc in npcSquad) {
            npc.Value.GetComponent<NPCPath>().FollowLeader();
        }
    }

    public void PlayerSceneLoad(string scene) {
        // things to save
        PlayerControlSave.Instance.localPlayerData.playerPosition = transform.position;
        PlayerControlSave.Instance.SaveData();
        SceneManager.LoadScene(scene);
    }

    public bool CanBattle() {
        var squadDict = PlayerControlSave.Instance.localPlayerData.monstersDict;
        foreach (KeyValuePair<string, Monster> monster in squadDict)
            if (monster.Value.CurrentStatus != Status.Fainted)
                return true;
        return false;
    }

    private void Move() {
        anim.SetBool(isMoving, true);
        if (movement.x != 0) movement.y = 0;
        rb.MovePosition(rb.position + movement * PlayerControlSave.Instance.localPlayerData.playerSpeed * Time.deltaTime);
        anim.SetFloat(moveX, movement.x);
        anim.SetFloat(moveY, movement.y);
    }

    #region On Trigger Enter/Exit

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Enter" + collision.gameObject.name);

        switch(collision.gameObject.name) {
            case "Cutscene1":
                PlayerControlSave.Instance.localPlayerData.currentGameMode = GameMode.DialogueMoment;
                PlayerControlSave.Instance.localPlayerData.playerPosition.x = -3.5f;
                PlayerControlSave.Instance.localPlayerData.playerPosition.y = 4.03f;
                SceneManager.LoadScene(2);
                break;
        }

    }

    private void OnTriggerExit2D(Collider2D collision) {
        Debug.Log("Exit" + collision.gameObject.name);
    }

    #endregion

    #region On Collsion Enter/Exit
    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.collider.CompareTag("Sign")) {
            isNear = true;
            interactObj = collision.gameObject;
            collision.gameObject.GetComponent<Sign>().EnableKey();
        }
        else if (collision.collider.CompareTag("BattleNPC")) {
            isNear = true;
            interactObj = collision.gameObject;
            collision.gameObject.GetComponent<NPCBattleManager>().EnableKey();
        }
        else if (collision.collider.CompareTag("ItemPickup")) {
            isNear = true;
            interactObj = collision.gameObject;
            collision.gameObject.GetComponent<ItemPickUp>().EnableKey();
        }

    }

    private void OnCollisionExit2D(Collision2D collision) {

        if (collision.collider.CompareTag("Sign")) {
            isNear = false;
            interactObj = null;
            collision.gameObject.GetComponent<Sign>().DisableKey();
            Dialog.Instance.signBox.SetActive(false);
        }
        else if (collision.collider.CompareTag("BattleNPC")) {
            isNear = false;
            interactObj = null;
            collision.gameObject.GetComponent<NPCBattleManager>().DisableKey();
        }
        else if (collision.collider.CompareTag("ItemPickup")) {
            isNear = false;
            interactObj = collision.gameObject;
            collision.gameObject.GetComponent<ItemPickUp>().DisableKey();
        }
    }
    #endregion

}
