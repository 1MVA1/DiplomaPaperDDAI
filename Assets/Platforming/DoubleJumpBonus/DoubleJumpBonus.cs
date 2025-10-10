using UnityEngine;

public class DoubleJumpBonus : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.enabled = false;
            col.enabled = false;
        }
    }

    public void Refresh()
    {
        spriteRenderer.enabled = true; 
        col.enabled = true; 
    }
}
