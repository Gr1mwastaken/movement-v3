using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{

    public Rigidbody2D rb;
    public PlayerInput playerInput;

    [Header("Movement Variables")]
    public float jumpForce;
    public float walkSpeed;
    public float runSpeed = 8;
    public int facingDirection = 1;
    public float jumpCutMultiplier = .5f;
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;

    //move inputs
    private Vector2 moveInput;
    private bool runPressed;
    private bool jumpPressed;
    private bool jumpReleased;


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Slide Settings")]
    public float slideDuration = .6f;
    private bool isSliding;
    private float slideTimer;


    private void Start()
    {
        rb.gravityScale = normalGravity;
    }
    
    
        

    void Update()
    {
        HandleSlide();
        Flip();
    }



    void FixedUpdate()
    {
        ApplyVariableGravity();
        CheckGrounded();

        if (!isSliding)
            HandleMovement();
            
        HandleJump();
    }


    private void HandleMovement()
    {
        float currentSpeed = runPressed ? runSpeed : walkSpeed;
        float targetSpeed = moveInput.x * currentSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }



    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpReleased = false;
        }
        if (jumpReleased)
        {
            if (rb.linearVelocity.y > 0) //if still going up
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
            jumpReleased = false;
        }
    }
    

    private void HandleSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0)
            {
                isSliding = false;
            }
        }
        
        if(isGrounded && runPressed && moveInput.y < -.1f && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;
        }
    }

    void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y < -0.1f)
        {
            rb.gravityScale = fallGravity;
        }
        else if (rb.linearVelocity.y > 0.1)
        {
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }
    
    


    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


    void Flip()
    {
        if(moveInput.x > 0.1f)
        {
            facingDirection = 1;
        }
        else if(moveInput.x < -0.1f)
        {
            facingDirection = -1;
        }
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    public void OnMove (InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    public void OnRun (InputValue value)
    {
        runPressed = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else //button is released
        {
            jumpReleased = true;
        }
    }



   private void OnDrawGizmosSelected()
   {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
   }
}
