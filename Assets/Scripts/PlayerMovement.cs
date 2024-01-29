using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]private float speed;
    [SerializeField]private float jumpSpeed;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private LayerMask wallLayer;
    

    //grab reference for animator and rigidbody
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown = 0;
    float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update(){
        horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        //flip player when moving left/right
        if (horizontalInput > .01)             
            transform.localScale = new Vector2(3, 3);   //7 is the scale, changes size. Negative numbers flip sprite
        if (horizontalInput < -.01)
            transform.localScale = new Vector2(-3, 3);


        if (onWall())
        {
            StartCoroutine(KickOffWallAfterDelay(.5f));
        }

        if (Input.GetKey(KeyCode.Space))
            Jump();

        if (Input.GetKey(KeyCode.E) && isGrounded())
            anim.SetTrigger("roll");


        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        
        wallJumpCooldown += Time.deltaTime;

        


    }

    private void Jump(){
        if (onWall() && !isGrounded() && wallJumpCooldown > 1)
        {
            float direction = -Mathf.Sign(transform.localScale.x);
            body.velocity = new Vector2(direction * 30, 34);  //flip the direction and push up
            wallJumpCooldown = 0;
        }
        else if(isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            anim.SetTrigger("jump");
        }

    }


    private IEnumerator KickOffWallAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);

        // Code to execute after the delay
        if (!isGrounded() && onWall())
        {
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 0);
        }
    }
    
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.01f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;

    }

    public bool canAttack()
    {
        return isGrounded() && !onWall();
    }
}
