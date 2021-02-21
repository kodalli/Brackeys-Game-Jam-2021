using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;

    public bool movementIsActive = true;

    private bool isNear;
    private GameObject interactObj;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator anim;
    private int avgFrameRate;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        if (Instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update() {

        FPSCounter();

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Movement
        if (movement == Vector2.zero || !movementIsActive) anim.SetBool("isMoving", false);
        if (movement != Vector2.zero && movementIsActive) Move();

        // Interact and Dialogue box
        if (Input.GetKeyDown(KeyCode.Escape)) Dialog.Instance.SkipDialogue();
        if (!Dialog.Instance.IsDialogueOver() && Input.GetKeyDown(KeyCode.E))
            Dialog.Instance.NextSentence();
        else if (Input.GetKeyDown(KeyCode.E)) {
            // Interact with sign or npc
            if (interactObj == null) { }
            else if (isNear && interactObj.CompareTag("Sign"))
                interactObj.GetComponent<Sign>().OnInteractKey();
            else if (isNear && interactObj.CompareTag("BattleNPC"))
                interactObj.GetComponent<NPCBattleManager>().OnInteractKey();
        } 
    }

    public void PlayerSceneLoad(string scene) {
        // things to save
        PlayerControlSave.Instance.localPlayerData.playerPosition = transform.position;
        PlayerControlSave.Instance.SaveData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public bool CanBattle() {
        var squadDict = PlayerControlSave.Instance.localPlayerData.monstersDict;
        foreach (KeyValuePair<string, Monster> monster in squadDict)
            if (monster.Value.CurrentStatus != Status.Fainted)
                return true;
        return false;
    }

    private void Move() {
        anim.SetBool("isMoving", true);
        if (movement.x != 0) movement.y = 0;
        rb.MovePosition(rb.position + movement * PlayerControlSave.Instance.localPlayerData.playerSpeed * Time.deltaTime);
        anim.SetFloat("moveX", movement.x);
        anim.SetFloat("moveY", movement.y);
    }

    private void FPSCounter() {
        float current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        Debug.Log(avgFrameRate.ToString() + " FPS");
        //display_Text.text = avgFrameRate.ToString() + " FPS";
    }

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
    }
}
