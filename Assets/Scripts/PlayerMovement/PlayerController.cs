using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 movement;
    private Rigidbody2D rb;
    [SerializeField] private PlayerData playerData;
    private Animator anim;
    private bool isMoving;
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
        rb.MovePosition(rb.position + movement * playerData.movementVelocity * Time.fixedDeltaTime);
       
    }
    void SwitchAnimationState()
    {
        anim.SetBool("idleLeft", false); anim.SetBool("idleRight", false); anim.SetBool("idleUp", false); anim.SetBool("idleDown", false); 
        anim.SetBool("moveRight", false); anim.SetBool("moveLeft", false); anim.SetBool("moveUp", false); anim.SetBool("moveDown", false);

        if (movement.x > 0)
            anim.SetBool("moveRight", true);
        else if (movement.x < 0)
            anim.SetBool("moveLeft", true);
        else if(movement.y > 0)
            anim.SetBool("moveUp", true);
        else if (movement.y < 0)
            anim.SetBool("moveDown", true);

    }
}
