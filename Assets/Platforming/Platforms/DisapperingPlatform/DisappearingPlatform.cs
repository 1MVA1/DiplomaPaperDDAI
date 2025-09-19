using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class FallingPlatform : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    private PlatformEffector2D effector;

    public Difficulty trapDifficulty = Difficulty.Easy;

    public float disapperingDuration = 2f;
    public float apperingDuration = 0.5f;
    public float respawnDelay = 3f;  

    private bool isFading = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        effector = GetComponent<PlatformEffector2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFading && collision.gameObject.CompareTag("Player"))
        {
            // check the direction of impact (up)
            if (collision.contacts[0].normal.y < -0.5f) {
                StartCoroutine(FadeAndRespawn());
            }
        }
    }

    private IEnumerator FadeAndRespawn()
    {
        isFading = true;

        // Step 1
        float timer = 0f;
        Color startColor = sr.color;

        while (timer < disapperingDuration)
        {
            timer += Time.deltaTime;
            sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, timer / disapperingDuration));

            yield return null;
        }

        boxCol.enabled = false;
        effector.enabled = false;

        // Step 2
        yield return new WaitForSeconds(respawnDelay);

        Collider2D[] hits;

        do
        {
            hits = Physics2D.OverlapBoxAll(boxCol.bounds.center, boxCol.bounds.size, 0f);
            yield return new WaitForSeconds(0.1f);
        }
        while (hits.Length > 0);

        // Step 3
        timer = 0f;

        while (timer < apperingDuration)
        {
            timer += Time.deltaTime;
            sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(0f, 1f, timer / apperingDuration));

            yield return null;
        }

        boxCol.enabled = true;
        effector.enabled = true;

        isFading = false;
    }
}