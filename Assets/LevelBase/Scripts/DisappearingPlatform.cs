
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DisapperingPlatform : MonoBehaviour, IRefreshable
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    [Range(2, 5)]
    public int size = 2;

    private float spacing = 0.54f;

    [Header("Main")]
    public GameObject prefab;
    public BoxCollider2D boxCol;

    private SpriteRenderer[] srs;

    [Header("Logic")]
    public float disDuration = 1.5f;
    public float respDelay = 1.5f;
    public float apperingDuration = 0.35f;

    private bool isFading = false;
    private Coroutine coroutineFaR;

    private bool isOn = false;

    [Header("Sprites")]
    public Sprite[] sprites;


    private void OnValidate()
    {
#if UNITY_EDITOR
        if (queued)
        {
            return;
        }

        queued = true;

        EditorApplication.delayCall += () =>
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            Generate();

            queued = false;
        };
#endif
    }
    private void Generate()
    {
        ClearChildren();

        boxCol.size = new Vector3(size * spacing, boxCol.size.y, 1.0f);

        int count = Mathf.Max(1, Mathf.RoundToInt(size));
        float centerOffset = (count - 1) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            GameObject obj;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            }
            else
            {
                obj = Instantiate(prefab, transform);
            }
#else
        obj = Instantiate(prefab, transform);
#endif

            float x = (i - centerOffset) * spacing;
            obj.transform.localPosition = new Vector3(x, 0f, 0f);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (i == 0)
            {
                sr.sprite = sprites[0];
            }
            else if (i == count - 1)
            {
                sr.sprite = sprites[0];
                obj.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            }
            else
            {
                sr.sprite = sprites[1];
            }
        }

#if UNITY_EDITOR
        queued = false;
#endif
    }
    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
                Destroy(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }
    }

    public void PrepDiff()
    {
    }

    public void Start()
    {
        srs = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private IEnumerator FadeAndRespawn()
    {
        isFading = true;

        // Step 1
        float timer = 0f;
        Color startColor = srs[0].color;

        while (timer < disDuration)
        {
            if (!isOn)
            {
                yield break;
            }

            timer += Time.deltaTime;

            foreach (SpriteRenderer sr in srs)
            {
                sr.color = new Color(startColor.r, startColor.g, startColor.b, 
                    Mathf.Lerp(1f, 0f, timer / disDuration));
            }

            yield return null;
        }

        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = false;
        }

        boxCol.enabled = false;

        // Step 2
        yield return new WaitForSeconds(respDelay);

        Collider2D[] hits;
        do
        {
            if (!isOn)
            {
                yield break;
            }

            hits = Physics2D.OverlapBoxAll(boxCol.bounds.center, boxCol.bounds.size, 0f);

            yield return new WaitForSeconds(0.1f);
        }
        while (hits.Length > 0);

        // Step 3
        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = true;
        }

        float appearTimer = 0f;

        while (appearTimer < apperingDuration)
        {
            if (!isOn)
            {
                yield break;
            }

            appearTimer += Time.deltaTime;

            foreach (SpriteRenderer sr in srs)
            {
                sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(0f, 1f, appearTimer / apperingDuration));
            }

            yield return null;
        }

        boxCol.enabled = true;

        isFading = false;
        coroutineFaR = null;
    }

    public void TurnOn()
    {
        isOn = true;
    }
    public void TurnOff()
    {
        isOn = false;
    }
    public void Refresh()
    {
        if (coroutineFaR != null)
        {
            StopCoroutine(coroutineFaR);
            coroutineFaR = null;
        }

        isFading = false;

        foreach (SpriteRenderer sr in srs)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            sr.enabled = true;
        }

        boxCol.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFading && collision.gameObject.CompareTag("PlayerLegs"))
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                coroutineFaR = StartCoroutine(FadeAndRespawn());
            }
        }
    }
}
