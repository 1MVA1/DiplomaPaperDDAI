using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour, IAdjustableDifficulty
{
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    private PlatformEffector2D effector;

    private Coroutine coroutineFaR;

    [System.Serializable]
    public class PlatformDiff
    {
        public float disappearingDuration;
        public float respawnDelay;

        public PlatformDiff(float disappearingDuration, float respawnDelay)
        {
            this.disappearingDuration = disappearingDuration;
            this.respawnDelay = respawnDelay;
        }
    }

    [Header("Difficulty Settings")]
    public PlatformDiff diffEasy = new PlatformDiff(2f, 2f);
    public PlatformDiff diffMedium = new PlatformDiff(1.5f, 2.5f);
    public PlatformDiff diffHard = new PlatformDiff(1f, 3f);

    private PlatformDiff currentDiff;

    [Header("Platform logic")]
    public float apperingDuration = 0.25f;

    private bool isFading = false;

    public void ApplyDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                currentDiff = diffEasy;
                break;
            case Difficulty.Medium:
                currentDiff = diffMedium;
                break;
            case Difficulty.Hard:
                currentDiff = diffHard;
                break;
        }
    }
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
            if (collision.contacts[0].normal.y < -0.5f)
                coroutineFaR = StartCoroutine(FadeAndRespawn());
        }
    }

    private IEnumerator FadeAndRespawn()
    {
        isFading = true;

        // Step 1
        float timer = 0f;
        Color startColor = sr.color;

        while (timer < currentDiff.disappearingDuration)
        {
            timer += Time.deltaTime;
            sr.color = new Color(startColor.r, startColor.g, startColor.b,
                Mathf.Lerp(1f, 0f, timer / currentDiff.disappearingDuration));

            yield return null;
        }

        sr.enabled = false;

        boxCol.enabled = false;
        effector.enabled = false;

        // Step 2
        yield return new WaitForSeconds(currentDiff.respawnDelay);

        Collider2D[] hits;
        do
        {
            hits = Physics2D.OverlapBoxAll(boxCol.bounds.center, boxCol.bounds.size, 0f);
            yield return new WaitForSeconds(0.1f);
        }
        while (hits.Length > 0);

        // Step 3
        sr.enabled = true;

        float appearTimer = 0f;

        while (appearTimer < apperingDuration)
        {
            appearTimer += Time.deltaTime;
            sr.color = new Color(startColor.r, startColor.g, startColor.b,
                Mathf.Lerp(0f, 1f, appearTimer / apperingDuration));

            yield return null;
        }

        boxCol.enabled = true;
        effector.enabled = true;

        isFading = false;
        coroutineFaR = null;
    }

    public void Refresh()
    {
        if (coroutineFaR != null)
        {
            StopCoroutine(coroutineFaR);
            coroutineFaR = null;
        }

        isFading = false;

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        sr.enabled = true;

        boxCol.enabled = true;
        effector.enabled = true;
    }
}