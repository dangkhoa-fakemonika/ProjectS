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
    public bool isHittingWall = false;
    public bool isHittingHead = false;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    Animator animator;

    float colliderSizeX;
    float colliderSizeY;
    float colliderOffsetX;
    float colliderOffsetY;

    // [Header("Ground Checking")]
    // public Vector2 groundBoxSize;
    // public float groundCastDistance;
    // [Header("Wall Checking")]
    // public Vector2 facingBoxSize;
    // public float faceCastDistance;

    [Header ("Identifiers")]
    public LayerMask groundLayer;
    public LayerMask moveableLayer;
    public LayerMask wallLayer;

    private Vector2 horizontalBoxSize;
    private Vector2 verticalBoxSize;
    private float horizontalCastDistance;
    private float verticalCastDistance;
    private float topCastDistance;

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
        // horizontalInput = (float) Math.Sign(horizontalInput) * Math.Max(Math.Abs(horizontalInput), 1f);

        verticalInput = Input.GetAxis("Vertical");

        FlipSprite();
        isGrounded = checkGround();
        isHittingWall = checkHittingWall();
        if (isCrouching)
            isHittingHead = checkHittingHead();
        else
            isHittingHead = false;
        
        if (isHittingHead){
            isCrouching = true;
        }
        else {
            bool isPressingCrouch = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            isCrouching = isGrounded && isPressingCrouch;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching){
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
        }

        if (transform.position.y < -15){
            transform.position = new Vector3(5f, 10f, transform.position.z);
        }

        if (!isFacingRight)
        characterPos = new Vector3(transform.position.x + boxCollider.offset.x * 4f, transform.position.y + boxCollider.offset.y * 4f, transform.position.z);
        else
        characterPos = new Vector3(transform.position.x - boxCollider.offset.x * 4f, transform.position.y + boxCollider.offset.y * 4f, transform.position.z);


        horizontalBoxSize = new Vector2(boxCollider.size.x * 4f, 0.25f);
        verticalBoxSize = new Vector2(0.25f, boxCollider.size.y * 4f);
        horizontalCastDistance = boxCollider.size.x * 2f + 0.125f;
        verticalCastDistance = boxCollider.size.y * 2f + 0.125f;
        topCastDistance = colliderSizeY * 2f + 0.125f - colliderOffsetY * 2f;

        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isJumping", !isGrounded);

    }

    private void FixedUpdate() {
        if (isHittingWall && ((isFacingRight && horizontalInput > 0f) || (!isFacingRight && horizontalInput < 0f))){
            animator.SetFloat("xVelocity", 0);
            animator.SetFloat("yVelocity", rb.velocity.y);
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (!isCrouching){
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
        if (Physics2D.BoxCast(characterPos, horizontalBoxSize, 0, -transform.up, verticalCastDistance, groundLayer)){
            return true;
        }
        else{
            return false;
        }
    }

    public bool checkHittingWall(){
        int wallDirection = 1;
        if (!isFacingRight) wallDirection = -1;

        Vector2 realFaceBox = verticalBoxSize;
        if (isCrouching) realFaceBox = new Vector2(verticalBoxSize.x, verticalBoxSize.y * 0.7f);

        if (Physics2D.BoxCast(characterPos, realFaceBox, 0, transform.right * wallDirection, horizontalCastDistance - verticalBoxSize.x + 0.1f, wallLayer)){
            return true;
        }
        else{
            return false;
        }
    }

    public bool checkHittingHead(){
        if (!isCrouching) return true;

        if (Physics2D.BoxCast(characterPos, horizontalBoxSize, 0, transform.up, topCastDistance, groundLayer)){
            return true;
        }
        else{
            return false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(characterPos - transform.up * verticalCastDistance, horizontalBoxSize);
        Gizmos.DrawWireCube(characterPos + transform.up * topCastDistance, horizontalBoxSize);
        
        int wallDirection = 1;
        if (!isFacingRight) wallDirection = -1;
        Vector2 realFaceBox = verticalBoxSize;

        if (isCrouching) {
            realFaceBox = new Vector2(verticalBoxSize.x, verticalBoxSize.y * 0.7f);
        }

        Gizmos.DrawWireCube(characterPos + wallDirection * transform.right * (horizontalCastDistance - verticalBoxSize.x + 0.1f), realFaceBox);
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
