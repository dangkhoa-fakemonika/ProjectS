using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoulMovement : MonoBehaviour
{
    [Header ("Soul Attributes")]
    public float catchingSpeed;
    public float flyingSpeed;
    public float maxActiveRange;
    [Header ("Player References")]
    [SerializeField] PlayerMovement playerData;
    public Transform player;
    private float facingOriginal;
    public GameObject soulSprite;
    private bool isFacingRight = false;
    Animator animator;

    Rigidbody2D rb;
    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        facingOriginal = transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        animator = soulSprite.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Alt Horizontal");
        verticalInput = Input.GetAxis("Alt Vertical");

        bool barrage = Input.GetKey(KeyCode.Q);
        bool punch = Input.GetKey(KeyCode.E) && !barrage;

        animator.SetBool("barrage", barrage);
        animator.SetBool("punch", punch);
    }

    void FixedUpdate(){
        Vector3 travelPosition;
        switch (playerData.soulMode){
            case 0:
                soulSprite.SetActive(false);
                if (!playerData.isFacingRight)
                    travelPosition = new Vector3(player.position.x + 1.5f, player.position.y + 0.5f, transform.position.z);
                else travelPosition = new Vector3(player.position.x - 1.5f, player.position.y + 0.5f, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, travelPosition, catchingSpeed);
                break;
            case 1:
                soulSprite.SetActive(true);
                FlipSpriteFollow();
                if (!playerData.isFacingRight)
                    travelPosition = new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, transform.position.z);
                else travelPosition = new Vector3(player.position.x - 0.5f, player.position.y + 0.5f, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, travelPosition, catchingSpeed);
                break;
            case 2:
                soulSprite.SetActive(true);
                if (OutOfActiveRange()){
                    FlipSpriteFollow();
                    travelPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
                    rb.velocity = new Vector2(0, 0);
                    transform.position = Vector3.MoveTowards(transform.position, travelPosition, catchingSpeed * 2);
                }
                else {
                    rb.velocity = new Vector2(horizontalInput * flyingSpeed, verticalInput * flyingSpeed);
                    animator.SetFloat("moving", Math.Abs(rb.velocity.x + rb.velocity.y) / 2f);
                    FlipSpriteMoving();
                }
                // transform.position = new Vector3(horizontalInput * flyingSpeed + transform.position.x, verticalInput * flyingSpeed + transform.position.y, transform.position.z);
                break;
            default:
                break;
        }

    }


    void FlipSpriteFollow(){
        Vector3 localScale = transform.localScale;
        if (playerData.isFacingRight)
            localScale.x = -facingOriginal;
        else localScale.x = facingOriginal;
        transform.localScale = localScale;
        isFacingRight = playerData.isFacingRight;
    }

    void FlipSpriteMoving(){
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f)){
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    bool OutOfActiveRange(){
        if ((transform.position.x - player.position.x) * (transform.position.x - player.position.x) + (transform.position.y - player.position.y) * (transform.position.y - player.position.y) > maxActiveRange * maxActiveRange){
            return true;
        }
        else
            return false;
    }
}
