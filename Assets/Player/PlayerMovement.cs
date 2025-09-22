using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    [Header("Movement")]
    public float moveSpeed = 5f;

    public bool isGrounded = false;

    private bool canMove = true;

    [Header("Jump")]
    public float jumpForce = 7f;
    public bool canActionDoubleJump = true;
    private bool canDoubleJump = true;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Wall")]
    public float wallSlideSpeed = 0.5f;
    public float wallJumpHorizontalForce = 2f;
    public float wallJumpVerticalForce = 2f;
    public float wallJumpLockDuration = 0.2f;

    private bool isTouchingGround = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private int wallDirection = 0; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => HandleJump();

        inputActions.Player.Dash.performed += ctx => {
            if (canDash && !isDashing) StartCoroutine(Dash());
        };
    }

    void HandleJump()
    {
        if (isGrounded)
        {
            isGrounded = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (isWallSliding && wallDirection != 0)
        {
            isWallSliding = false;
            canDoubleJump = true;
            canMove = false;

            rb.linearVelocity = new Vector2(-wallDirection * wallJumpHorizontalForce, wallJumpVerticalForce);

            StartCoroutine(EnableMoveAfterDelay(wallJumpLockDuration));
        }
        else if (canActionDoubleJump && canDoubleJump)
        {
            canDoubleJump = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (isTouchingWall && !isTouchingGround && Mathf.Abs(moveInput.x) > 0.01f && Mathf.Sign(moveInput.x) == wallDirection)
        {
            isWallSliding = true;

            if (rb.linearVelocity.y < -wallSlideSpeed) {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            }
        }
        else {
            isWallSliding = false;
        }

        if (!isDashing && canMove) {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(moveInput.x * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private IEnumerator EnableMoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        canMove = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isTouchingGround = true;
            isGrounded = true;
            canDoubleJump = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;

            ContactPoint2D best = collision.contacts[0];

            foreach (var c in collision.contacts) {
                if (Mathf.Abs(c.normal.x) > Mathf.Abs(best.normal.x)) best = c;
            }

            wallDirection = (best.point.x > transform.position.x) ? 1 : -1;

            if (!isTouchingGround && Mathf.Abs(moveInput.x) > 0.01f && Mathf.Sign(moveInput.x) == wallDirection) {
                isWallSliding = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isTouchingGround = false;
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
            wallDirection = 0;
            isWallSliding = false;
        }
    }

    void OnEnable() => inputActions.Enable();

    void OnDisable() => inputActions.Disable();
}
