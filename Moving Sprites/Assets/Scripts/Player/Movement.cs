using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    public float moveSpeed = 6f;
    bool isFacingRight = false;
    public float jumpPower = 10f;
    bool isGrounded = false;
    bool isCrouching = false;
    bool isHittingWall = false;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    Animator animator;

    float colliderSizeX;
    float colliderSizeY;
    float colliderOffsetX;
    float colliderOffsetY;

    [Header("Ground Checking")]
    public Vector2 groundBoxSize;
    public float groundCastDistance;
    public LayerMask groundLayer;
    [Header("Wall Checking")]
    public Vector2 facingBoxSize;
    public float faceCastDistance;


    private Vector3 characterPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        colliderSizeX = boxCollider.size.x;
        colliderSizeY = boxCollider.size.y;
        colliderOffsetX = boxCollider.offset.x;
        colliderOffsetY = boxCollider.offset.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isCrouching = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        animator.SetBool("isCrouching", isCrouching);

        FlipSprite();
        isGrounded = checkGround();
        // isHittingWall = checkHittingWall();

        if (Input.GetButtonDown("Jump") && isGrounded){
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
        }

        if (transform.position.y < -15){
            transform.position = new Vector3(5f, 10f, transform.position.z);
        }

        if (!isFacingRight)
        characterPos = new Vector3(transform.position.x + colliderOffsetX * 4f, transform.position.y, transform.position.z);
        else
        characterPos = new Vector3(transform.position.x - colliderOffsetX * 4f, transform.position.y, transform.position.z);
        animator.SetBool("isJumping", !isGrounded);

    }

    private void FixedUpdate() {
        // if (isHittingWall && ((isFacingRight && horizontalInput > 0f) || (!isFacingRight && horizontalInput < 0f))){
        //     animator.SetFloat("xVelocity", 0);
        //     animator.SetFloat("yVelocity", 0);
        //     rb.velocity = new Vector2(0, 0);
        //     return;
        // }

        if (!isCrouching || !isGrounded){
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            boxCollider.size = new Vector2(colliderSizeX, colliderSizeY);
            boxCollider.offset = new Vector2(colliderOffsetX, colliderOffsetY);
        }
        else{
            rb.velocity = new Vector2(horizontalInput * 0.4f * moveSpeed, rb.velocity.y);
            boxCollider.size = new Vector2(colliderSizeX, colliderSizeY * 0.7f);
            boxCollider.offset = new Vector2(colliderOffsetX, colliderOffsetY - colliderSizeY * 0.15f);
        }
        
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    void FlipSprite(){
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f)){
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public bool checkGround(){
        if (Physics2D.BoxCast(characterPos, groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer)){
            return true;
        }
        else{
            return false;
        }
    }

    public bool checkHittingWall(){
        int wallDirection = 1;
        if (!isFacingRight) wallDirection = -1;

        Vector2 realFaceBox = facingBoxSize;
        if (isCrouching) realFaceBox = new Vector2(facingBoxSize.x, facingBoxSize.y * 0.7f);

        if (Physics2D.BoxCast(transform.position, realFaceBox, 0, transform.right * wallDirection, faceCastDistance, groundLayer)){
            return true;
        }
        else{
            return false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(characterPos - transform.up * groundCastDistance, groundBoxSize);

        // int wallDirection = 1;
        // if (!isFacingRight) wallDirection = -1;
        // Vector2 realFaceBox = facingBoxSize;
        // if (isCrouching) realFaceBox = new Vector2(facingBoxSize.x, facingBoxSize.y * 0.7f);

        // Gizmos.DrawWireCube(transform.position + wallDirection * transform.right * faceCastDistance, realFaceBox);
    }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     isGrounded = true;
    //     animator.SetBool("isJumping", !isGrounded);
    // }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     isGrounded = true;
    //     animator.SetBool("isJumping", !isGrounded);
    // }

}
