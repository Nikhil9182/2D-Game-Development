using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speedOfCharacter;
    public float jumpForce;
    public float crouchHeight;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;

    private float horizontalMove; //to take input for the horizontal movement of the player
    private float verticalMove; //to take input for the vertical movement of the player
    private float normalColHeight; //box collider's heaight
    private float normalCOLOffset;
    private bool isCrouching; //bool variable for crouching of the character and its animation
    private bool canJump;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();

        normalColHeight = _capsuleCollider.size.y;
        normalCOLOffset = _capsuleCollider.offset.y;
    }

    private void Update()
    {
        //Take Input First
        TakeInput();
        //Set Animation for Playing
        SetAnim();
        //Flip character on basis of input
        Flip();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    public void MoveCharacter()
    {
        Vector3 pos = transform.position;
        //Horizontal Movement
        pos.x += horizontalMove * speedOfCharacter * Time.fixedDeltaTime;
        //Vertical Movement
        if(verticalMove > 0 && canJump)
        {
            _rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }
        transform.position = pos;

        if (isCrouching)
        {
            _capsuleCollider.offset = new Vector2(_capsuleCollider.offset.x, normalCOLOffset/2 + 0.1f);
            _capsuleCollider.size = new Vector2(_capsuleCollider.size.x, crouchHeight);
        }
        else if (!isCrouching)
        {
            _capsuleCollider.offset = new Vector2(_capsuleCollider.offset.x, normalCOLOffset);
            _capsuleCollider.size = new Vector2(_capsuleCollider.size.x, normalColHeight);
        }
    }

    public void TakeInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Jump");
        isCrouching = Input.GetKey(KeyCode.LeftControl);
    }

    public void SetAnim()
    {
        _animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        _animator.SetBool("Jump", (verticalMove > 0 && canJump) ? true : false);
        _animator.SetBool("Crouch", isCrouching);
    }

    public void Flip()
    {
        Vector3 scale = transform.localScale;
        if (horizontalMove < 0) 
        {
            scale.x = -1f;
        }
        else if (horizontalMove > 0) 
        {
            scale.x = 1f;
        }
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }
}
