using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator anim;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement == Vector2.zero) anim.SetBool("isMoving", false);
        if (movement != Vector2.zero) {
            anim.SetBool("isMoving", true);
            if (movement.x != 0) movement.y = 0;
            rb.MovePosition(rb.position + movement * PlayerControlSave.Instance.localPlayerData.playerSpeed * Time.deltaTime);
            anim.SetFloat("moveX", movement.x);
            anim.SetFloat("moveY", movement.y);
        }
    }

}
