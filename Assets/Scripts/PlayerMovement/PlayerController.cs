using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator anim;
    public bool movementIsActive = true;

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
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement == Vector2.zero || !movementIsActive) anim.SetBool("isMoving", false);
        if (movement != Vector2.zero && movementIsActive) {
            anim.SetBool("isMoving", true);
            if (movement.x != 0) movement.y = 0;
            rb.MovePosition(rb.position + movement * PlayerControlSave.Instance.localPlayerData.playerSpeed * Time.deltaTime);
            anim.SetFloat("moveX", movement.x);
            anim.SetFloat("moveY", movement.y);
        }

        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    PlayerSceneLoad("Battle Scene");
        //}
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
}
