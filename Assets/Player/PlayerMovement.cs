using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashSpeed = 12f;       // скорость рывка
    public float dashDuration = 0.2f;   // длительность рывка в секундах

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        // движение
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // прыжок
        inputActions.Player.Jump.performed += ctx =>
        {
            if (isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        };

        // рывок
        inputActions.Player.Dash.performed += ctx =>
        {
            if (!isDashing)
                StartCoroutine(Dash());
        };
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;

        // сохраняем текущую гравитацию и отключаем её
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // задаём мгновенную скорость рывка
        rb.linearVelocity = new Vector2(moveInput.x * dashSpeed, 0f);

        // ждём конец рывка
        yield return new WaitForSeconds(dashDuration);

        // возвращаем гравитацию
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("StaticPlatform"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("StaticPlatform"))
            isGrounded = false;
    }
}