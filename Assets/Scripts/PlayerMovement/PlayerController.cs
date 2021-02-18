using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum Direction { UP, DOWN, LEFT, RIGHT, IDLE }
    private string UP = "UP", DOWN = "DOWN", LEFT = "LEFT", RIGHT = "RIGHT";
    private Direction PreviousDirection;
    private Vector2 movement;
    private Rigidbody2D rb;
    [SerializeField] private PlayerData playerData;
    private Animator anim;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        Move();
    }

    void Move()
    {
        //anim.SetBool("isMoving", movement.Equals(Vector2.zero));

        movement = Mathf.Abs(movement.x) > Mathf.Abs(movement.y) ? new Vector2(movement.x, 0f) : new Vector2(0f, movement.y);

        rb.MovePosition(rb.position + movement * playerData.movementVelocity * Time.fixedDeltaTime);

        Direction CurrentDirection;

        if (movement.x > 0)
            CurrentDirection = Direction.RIGHT;
        else if (movement.x < 0)
            CurrentDirection = Direction.LEFT;
        else if (movement.y > 0)
            CurrentDirection = Direction.UP;
        else if (movement.y < 0)
            CurrentDirection = Direction.DOWN;
        else
            CurrentDirection = Direction.IDLE;

        UpdateAnimation(CurrentDirection);

        Debug.Log(CurrentDirection);
    }

    void UpdateAnimation(Direction dir)
    {
        if (dir.Equals(PreviousDirection))
            return;

        ToggleAnimation(PreviousDirection, false);

        ToggleAnimation(dir, true);
    }

    void ToggleAnimation(Direction dir, bool state)
    {
        switch(dir)
        {
            case Direction.UP:
                anim.SetBool(UP, state);
                break;
            case Direction.DOWN:
                anim.SetBool(DOWN, state);
                break;
            case Direction.LEFT:
                anim.SetBool(LEFT, state);
                break;
            case Direction.RIGHT:
                anim.SetBool(RIGHT, state);
                break;
            default:
                break;
        }
    }
}
