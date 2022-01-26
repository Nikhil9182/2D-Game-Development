using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    public float speedOfCharacter;
    public float jumpForce;
    public float crouchHeight;
    public float groundCheckRadius;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;

    private float horizontalMove; //to take input for the horizontal movement of the player
    private float verticalMove; //to take input for the vertical movement of the player
    private float normalColHeight; //box collider's heaight
    private float normalCOLOffset; //box collider's offset
    private bool isCrouching; //bool variable for crouching of the character and its animation
    private bool hasLanded;
    private bool canJump;
    private bool isGrounded;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();

        normalColHeight = _capsuleCollider.size.y;
        normalCOLOffset = _capsuleCollider.offset.y;
        canJump = true;
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
        CheckGround();
        MoveCharacter();
    }

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                if (!wasGrounded)
                {
                    hasLanded = true;
                    canJump = true;
                }
            }
        }

        if (!isGrounded)
        {
            hasLanded = false;
        }
    }

    public void MoveCharacter()
    {
        Vector3 pos = transform.position;
        //Horizontal Movement
        pos.x += horizontalMove * speedOfCharacter * Time.deltaTime;
        //Vertical Movement
        if(verticalMove > 0 && canJump && !isCrouching)
        {
            _rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            canJump = false;
            hasLanded = false;
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
        _animator.SetBool("Land", hasLanded);
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
            hasLanded = true;
        }
    }
}
