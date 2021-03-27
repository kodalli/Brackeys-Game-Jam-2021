using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRBController : MonoBehaviour {

    PlayerActionControls playerActionControls;
    BoxCollider2D box2d;
    Animator animator;
    Rigidbody2D rb;
    Transform groundCheck;

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float jumpSpeed = 3f;

    private Vector2 previousVelocity;
    private Vector2 currentVelocity;

    Vector2 movementInput;
    int keyHorizontal;
    bool keyJump;
    // bool keyShoot;

    bool isGrounded;
    float groundCheckRadius = 0.3f;
    bool isShooting;
    bool isFacingRight;

    // Enables Input Actions Map
    void OnEnable() => playerActionControls.Enable();
    void OnDisable() => playerActionControls.Disable();
    void Awake() => playerActionControls = new PlayerActionControls();

    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        box2d = GetComponent<BoxCollider2D>();
        groundCheck = transform.Find("GroundCheck");

        isFacingRight = true; // sprite defaults to facing right
    }
    void FixedUpdate() {
        // CheckIfGrounded();
        isGrounded = CheckIfGroundedRaycast();
    }
    void Update() {
        movementInput = playerActionControls.Gameplay.Movement.ReadValue<Vector2>();
        keyHorizontal = (int)(movementInput * Vector2.right).normalized.x;
        playerActionControls.Gameplay.Jump.performed += _ => keyJump = true;
        playerActionControls.Gameplay.Jump.canceled += _ => keyJump = false;
        // playerActionControls.Gameplay.Shoot.performed += _ => keyShoot = true;
        // playerActionControls.Gameplay.Shoot.canceled += _ => keyShoot = false;

        keyHorizontal = (int)Input.GetAxisRaw("Horizontal");

        currentVelocity = rb.velocity;

        if (keyHorizontal < 0) {
            if (isFacingRight) Flip();
            if (isGrounded) {
                animator.Play("Player_Run");
            }
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            // SetMovementVelocity(-moveSpeed);
        } else if (keyHorizontal > 0) {
            if (!isFacingRight) Flip();
            if (isGrounded) {
                animator.Play("Player_Run");
            }
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        } else {
            if (isGrounded && currentVelocity == Vector2.zero) {
                animator.Play("Player_Idle");
            }
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (keyJump && isGrounded) {
            animator.Play("Player_Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        if (!isGrounded && keyHorizontal != 0 && currentVelocity.y > 0) {
            animator.Play("Player_Jump");
        }
        if (!isGrounded && currentVelocity.y < 0) {
            animator.Play("Player_Fall");
        }
    }
    void Flip() {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180.0f, 0f);
    }
    void CheckIfGroundedBoxCast() {
        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        // Ground Check
        Vector3 box_origin = box2d.bounds.center;
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        // See if box colliding with ground
        if (raycastHit.collider != null) {
            isGrounded = true;
        }
        // Draw debug lines
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
    }
    public bool CheckIfGroundedRaycast() {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMask);
    }



}
