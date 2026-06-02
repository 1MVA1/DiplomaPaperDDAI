
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public SpriteRenderer sr_body;
    public SpriteRenderer sr_legs;

    public Sprite[] sprites_body;
    public Sprite[] sprites_legs;

    private PlayerInputActions inputActions;

    [Header("Animation Settings")]
    public float animationSpeed = 0.1f;
    private float animationTimer;
    private int currentLegStep = 0;

    [Header("Movement")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private bool canMove = true;

    [Header("Ground")]
    public bool isGrounded = false;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    public float jumpCutMultiplier = 0.5f;

    private float jumpBufferCounter;
    private int groundContacts = 0;
    private float coyoteTimeCounter;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool canDash = true;
    private bool isDashing = false;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform shootTransform;
    public float fireRate = 1.5f;
    public float shootAnimTime = 0.5f;

    private float lastTimeShoot;
    private bool isShooting = false;
    private Coroutine shootAnimCoroutine;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpBufferCounter = jumpBufferTime;
        inputActions.Player.Jump.canceled += ctx => OnJumpReleased();

        inputActions.Player.Dash.performed += ctx =>
        {
            if (canDash && !isDashing)
            {
                StartCoroutine(Dash());
            }
        };

        inputActions.Player.Shoot.performed += ctx => isShooting = true;
        inputActions.Player.Shoot.canceled += ctx => isShooting = false;

        inputActions.Player.Teleport.performed += ctx =>
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
                UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            worldPosition.z = transform.position.z;

            transform.position = worldPosition;
        };
    }
    void Update()
    {
        if (!isGrounded)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        else
        {
            coyoteTimeCounter = coyoteTime;
        }

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isShooting)
        {
            if (Time.time >= lastTimeShoot + fireRate)
            {
                lastTimeShoot = Time.time;

                shootAnimCoroutine = StartCoroutine(ShootAnimationRoutine());

                var bullet = Instantiate(bulletPrefab, shootTransform.position, shootTransform.rotation);

                Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
                mouseScreenPos.z = 0f;

                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreenPos);

                Vector2 dir = (mouseWorld - shootTransform.position).normalized;

                bullet.GetComponent<PlayerBullet>().Init(dir);
            }
        }

        HandleJump();

        HandleAnimations();
    }
    void FixedUpdate()
    {
        if (!isDashing && canMove)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    void Move(Vector2 moveDir)
    {
        moveInput = moveDir;

        if (moveDir.x > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (moveDir.x < 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    void HandleJump()
    {
        if (jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || isGrounded))
        {
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            isGrounded = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    void OnJumpReleased()
    {
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }


    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;

        rb.gravityScale = 0f;

        float direction;

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            direction = Mathf.Sign(moveInput.x);
        }
        else
        {
            direction = transform.eulerAngles.y == 0f ? 1f : -1f;
        }

        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void HandleAnimations()
    {
        if (!isGrounded)
        {
            sr_legs.sprite = sprites_legs[1];
            return; 
        }

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                currentLegStep = (currentLegStep + 1) % sprites_legs.Length;
                sr_legs.sprite = sprites_legs[currentLegStep];
            }
        }
        else
        {
            sr_legs.sprite = sprites_legs[0];
            animationTimer = 0f;
        }
    }
    private IEnumerator ShootAnimationRoutine()
    {
        sr_body.sprite = sprites_body[1];

        yield return new WaitForSeconds(shootAnimTime);

        sr_body.sprite = sprites_body[0]; 
        shootAnimCoroutine = null;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform") || other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            groundContacts++;
            isGrounded = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Platform") || other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            groundContacts--;

            if (groundContacts <= 0)
            {
                isGrounded = false;
                groundContacts = 0;
            }
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        canDash = false;
        isDashing = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
}
