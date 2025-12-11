using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 36f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;

//if(keycode left shift && down)change sprite or whatever
    [SerializeField] private TrailRenderer tr;
    
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
    public float slideSpeed = 12;
    public float slideStopDuration = .15f;
    private bool isSliding;
    private bool slideInputLocked;
    private float slideTimer;
    private float slideStopTimer;


    private void Start()
    {
        rb.gravityScale = normalGravity;
    }
    
    
        

    private void Update()
    {   
        

        if (isDashing)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            StartCoroutine(Dash());
        }
        
        HandleSlide();
        if(!isSliding)
            Flip();


    }



    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
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
            rb.linearVelocity = new Vector2(slideSpeed * facingDirection, rb.linearVelocity.y);

            if (slideTimer <= 0)
            {
                isSliding = false;
                slideStopTimer = slideStopDuration;
            }


        }


        if(slideStopTimer > 0)
        {
            slideStopTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }


        if(isGrounded && runPressed && moveInput.y < -.1f && !isSliding && !slideInputLocked)
        {
            isSliding = true;
            slideInputLocked = true;
            slideTimer = slideDuration;
        }

        if(slideStopTimer < 0 && moveInput.y >= -.1f)
        {
            slideInputLocked = false;
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


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    

}
