using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheckTransform;
    public LayerMask groundLayer;

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
    private bool isJumping;
    private bool isFalling;
    private bool isGrounded;


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
        //Move character
        MoveCaharacter();
        //Set Animation for Playing
        SetAnim();
        //Flip character on basis of input
        Flip();
    }

    private void FixedUpdate()
    {
        CharacterPhysics();
    }

    private void MoveCaharacter()
    {
        Vector3 pos = transform.position;
        //Horizontal Movement
        pos.x += horizontalMove * speedOfCharacter * Time.deltaTime;
        transform.position = pos;
    }

    public void CharacterPhysics()
    {
        //Check if grounded
        //isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);

        //Vertical Movement
        if (verticalMove > 0 && isGrounded && !isCrouching)
        {
            Debug.Log(isGrounded);
            _rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
        }

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
        isCrouching = Input.GetKey(KeyCode.LeftControl) && isGrounded;
    }

    public void SetAnim()
    {
        _animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        _animator.SetBool("isJumping", isJumping);
        _animator.SetBool("Crouch", isCrouching);
        _animator.SetBool("isFalling", isFalling);
        _animator.SetBool("isGrounded", isGrounded);
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

    private void OnFallEvent()
    {
        isFalling = true;
        isJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 31)
        {
            isGrounded = true;
            isFalling = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 31)
        {
            isGrounded = false;
        }
    }
}
