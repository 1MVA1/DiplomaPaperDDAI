using UnityEditor.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformDownLogic : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private BoxCollider2D platformCollider;
    private BoxCollider2D playerCollider;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        platformCollider = GetComponent<BoxCollider2D>();

        inputActions.Player.Move.performed += ctx =>
        {
            TryToGetPlayerColider();

            if (ctx.ReadValue<Vector2>().y < 0) {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
            }
        };

        inputActions.Player.Move.canceled += ctx =>
        {
            TryToGetPlayerColider();

            StartCoroutine(TryToTurnOnColider());
        };
    }

    private void TryToGetPlayerColider()
    {
        if (playerCollider == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerCollider = player.GetComponent<BoxCollider2D>();
        }
    }

    private System.Collections.IEnumerator TryToTurnOnColider()
    {
        Collider2D[] hits;

        do
        {
            hits = Physics2D.OverlapBoxAll(platformCollider.bounds.center, platformCollider.bounds.size, 0f);

            bool playerInside = false;

            foreach (var hit in hits)
            {
                if (hit == playerCollider)
                {
                    playerInside = true;
                    break;
                }
            }

            if (playerInside)
                yield return null;
            else
                break;
        }
        while (true);

        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
}