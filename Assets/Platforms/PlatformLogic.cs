using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlatformController : MonoBehaviour
{
    private PlatformEffector2D effector;
    private PlayerInputActions inputActions;

    private bool isPlayerOn = false;

    void Awake()
    {
        effector = GetComponent<PlatformEffector2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            if (input.y < 0) {
                effector.surfaceArc = 0f;
            }
        };

        inputActions.Player.Move.canceled += ctx => {
            StartCoroutine(RestoreSurfaceArc());
        };
    }

    private IEnumerator RestoreSurfaceArc()
    {
        while (isPlayerOn)
            yield return null; 

        effector.surfaceArc = 180f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            isPlayerOn = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            isPlayerOn = false;
        }
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
}