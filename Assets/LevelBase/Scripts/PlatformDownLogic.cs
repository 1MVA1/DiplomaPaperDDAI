
using UnityEngine;


public class PlatformDownLogic : MonoBehaviour
{
    [Header("Main")]
    public BoxCollider2D boxCol;
    public PlatformEffector2D platCol;

    private PlayerInputActions inpAct;
    private BoxCollider2D playerCol;
    private BoxCollider2D playerBotCol;


    void Awake()
    {
        inpAct = new PlayerInputActions();

        inpAct.Player.Move.performed += ctx =>
        {
            TryToGetPlayerColiders();

            if (ctx.ReadValue<Vector2>().y < 0)
            {
                if (playerCol != null)
                {
                    Physics2D.IgnoreCollision(playerCol, boxCol, true);
                    Physics2D.IgnoreCollision(playerBotCol, boxCol, true);
                }
            }
        };

        inpAct.Player.Move.canceled += ctx =>
        {
            TryToGetPlayerColiders();

            StartCoroutine(TryToTurnOnColider());
        };
    }

    private void TryToGetPlayerColiders()
    {
        if (playerCol == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("PlayerLegs");

            if (player != null)
            {
                playerBotCol = player.GetComponent<BoxCollider2D>();

                Transform bodyTransform = player.transform.Find("Body");

                if (bodyTransform != null)
                {
                    playerCol = bodyTransform.GetComponent<BoxCollider2D>();
                }
            }
        }
    }

    private System.Collections.IEnumerator TryToTurnOnColider()
    {
        Collider2D[] hits;

        do
        {
            if (playerCol == null)
                yield break;

            hits = Physics2D.OverlapBoxAll(boxCol.bounds.center, boxCol.bounds.size, 0f);

            bool playerInside = false;

            foreach (var hit in hits)
            {
                if (hit == playerCol)
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

        if (playerCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, boxCol, false);
            Physics2D.IgnoreCollision(playerBotCol, boxCol, false);
        }
    }

    void OnEnable() => inpAct.Enable();
    void OnDisable() => inpAct.Disable();
}