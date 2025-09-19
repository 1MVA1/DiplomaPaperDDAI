using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public bool canActionDoubleJump = true;
    public float jumpForce = 7f;
    private bool isGrounded = false;
    private bool isTouchingGround = false;
    private bool canDoubleJump = true;

    [Header("Dash Settings")]
    public bool canActionDash = true;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx =>
        {
            if (isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
            }
            else if (canActionDoubleJump && canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = false;
            }
        };

        inputActions.Player.Dash.performed += ctx =>
        {
            if (canActionDash && !isDashing && canDash) {
                StartCoroutine(Dash());
            }
        };
    }

    void Update()
    {
        if (isGrounded && rb.linearVelocity.y < 0.0) {
            isGrounded = false;
        }
        else if (!isGrounded && rb.linearVelocity.y == 0.0 && isTouchingGround)
        {
            isGrounded = true;
            canDoubleJump = true;
        }
    }
    void FixedUpdate()
    {
        if (!isDashing) {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private System.Collections.IEnumerator Dash()
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")) {
            isTouchingGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")) {
            isTouchingGround = false;
        }
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
}