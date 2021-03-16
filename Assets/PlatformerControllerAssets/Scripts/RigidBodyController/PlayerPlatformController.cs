using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformController : MonoBehaviour {

    // Components
    private Rigidbody2D rb;
    private Animator anim;

    // Movement variables
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float groundCheckRadius = 0f;
    [SerializeField] private int amountOfJumps = 1;
    [SerializeField] private LayerMask whatIsGround;

    private float movementInputDirection;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool canJump;
    private int amountOfJumpsLeft;

    // Animator Parameters
    private bool isMoving;

    // Check Variables
    [SerializeField] private Transform groundCheck;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
    }
    void Update() {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
    }


    private void FixedUpdate() {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckInput() {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) {
            Jump();
        }
    }
    private void CheckMovementDirection() {
        if (isFacingRight && movementInputDirection < 0) {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0) {
            Flip();
        }

        isMoving = rb.velocity.x != 0; // Simplified if statement to set isMoving based on movement
    }
    private void ApplyMovement() {
        rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
    }
    private void Jump() {
        if (canJump) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
    }
    private void UpdateAnimations() {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }
    private void CheckSurroundings() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }
    private void CheckIfCanJump() {
        if (isGrounded && rb.velocity.y <= 0f) {
            amountOfJumpsLeft = amountOfJumps;
        }
        if(amountOfJumpsLeft <= 0) {
            canJump = false;
        }
        else {
            canJump = true;
        }
    }
    private void Flip() {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, -180f, 0f);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}

