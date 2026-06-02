
using System.Linq;
using UnityEditor;
using UnityEngine;


public class VoltZone : MonoBehaviour, IApplyDiff_VoltZone, IRefreshable
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    public Diff diff;
    [Range(3, 12)]
    public int size = 3;
    public bool isHorizontal = true;

    private int idx;

    [Header("Main")]
    public GameObject prefab;
    public BoxCollider2D boxCol;
    public SpriteRenderer[] srBlocks;
    public SpriteRenderer srSign;
    public SpriteRenderer[] srStarts;
    public Transform[] blocks;
    public Transform[] starts;
    public Transform sign;

    public string[] noDelNames = { "Block1", "Block2", "Sign", "Start1", "Start2" };

    [Header("Diff")]
    public float[] intervals = { 6f, 5f, 4f };
    public float activeDuration = 1.5f;

    private float interval;

    private float blinkTimer = 0f;
    private bool blinkVisible = true;

    private float stateTimer = 0f;

    private bool isActive = false;
    private bool isOn = false;

    [Header("Sprites")]
    public Sprite[] sprites1;
    public Sprite[] sprites2;
    public Sprite[] spritesBlock;
    public Sprite[] spritesSignEasy;
    public Sprite spriteSignMedium;
    public Sprite[] spritesStart1;
    public Sprite[] spritesStart2;

    public float frequency = 0.2f;

    public float spacing = 0.42f;
    public float spacingBlock = 0.56f;
    public float spacingStart = 0.4f;
    public float blinkFrequency = 0.1f;

    private float timer;
    private bool isFirstSprite = true;

    private SpriteRenderer[] srs;


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

            ApplyDiff(diff, size, isHorizontal);

            queued = false;
        };
#endif
    }
    private void Generate()
    {
        if (!Application.isPlaying)
        {
            ClearChildren();
        }

        float helper = (float)(size - 2) / 2 * spacing;

        float newX = helper + spacingBlock;

        blocks[0].localPosition = new Vector3(-newX, blocks[0].localPosition.y, blocks[0].localPosition.z);
        blocks[1].localPosition = new Vector3(newX, blocks[1].localPosition.y, blocks[1].localPosition.z);

        newX = helper + spacingStart;

        starts[0].localPosition = new Vector3(-newX, starts[0].localPosition.y, starts[0].localPosition.z);
        starts[1].localPosition = new Vector3(newX, starts[1].localPosition.y, starts[1].localPosition.z);

        boxCol.size = new Vector3(size * spacing, boxCol.size.y, 1.0f);

        int count = Mathf.Max(1, Mathf.RoundToInt(size));

        float centerOffset = (count - 1) * 0.5f;

        if (diff == Diff.Medium)
        {
            srSign.sprite = spriteSignMedium;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj;

            if (!Application.isPlaying)
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            else
                obj = Instantiate(prefab, transform);

            float x = (i - centerOffset) * spacing;
            obj.transform.localPosition = new Vector3(x, 0f, 0f);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            sr.sprite = sprites1[idx];
            sr.enabled = false;
        }

#if UNITY_EDITOR
        queued = false;
#endif
    }
    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (noDelNames.Contains(child.name))
                continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
        Destroy(child.gameObject);
#endif
        }
    }

    public void PrepDiff()
    {
        idx = (int)diff;

        srBlocks[0].sprite = spritesBlock[idx];
        srBlocks[1].sprite = spritesBlock[idx];

        interval = intervals[idx];
    }
    public void ApplyDiff(Diff diff_, float size_, bool isHorizontal_)
    {
        diff = diff_;
        isHorizontal = isHorizontal_;

        size = (int)size_;

        if (isHorizontal)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            sign.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            sign.localRotation = Quaternion.Euler(0f, 0f, -90f);
        }

        PrepDiff();

        Generate();

        if (Application.isPlaying)
        {
            srs = GetComponentsInChildren<SpriteRenderer>(true)
                .Where(sr => !noDelNames.Contains(sr.gameObject.name)).ToArray();
        }
    }

    void Update()
    {
        if (isOn)
        {
            if (isActive)
            {
                timer += Time.deltaTime;

                if (timer >= frequency)
                {
                    isFirstSprite = !isFirstSprite;

                    Sprite newSprite = isFirstSprite ? sprites1[idx] : sprites2[idx];

                    foreach (SpriteRenderer sr in srs)
                    {
                        sr.sprite = newSprite;
                    }

                    timer = 0;
                }

                stateTimer += Time.deltaTime;

                if (stateTimer >= activeDuration)
                {
                    DeactivateZone();
                }
            }
            else
            {
                stateTimer += Time.deltaTime;

                if (stateTimer >= interval)
                {
                    ActivateZone();
                }

                float remainingTime = interval - stateTimer;

                UpdateStarts(remainingTime);

                if (diff == Diff.Easy)
                {
                    UpdateSign_Easy(remainingTime);
                }
                else if (diff == Diff.Medium)
                {
                    UpdateSign_Medium(remainingTime);
                }
            }
        }
    }

    private void ActivateZone()
    {
        if (!isOn)
        {
            return;
        }

        srSign.enabled = false;

        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = true;
        }

        foreach (SpriteRenderer sr in srStarts)
        {
            sr.enabled = false;
        }

        boxCol.enabled = true;
        isActive = true;

        stateTimer = 0f;

        isFirstSprite = true;
    }
    private void DeactivateZone()
    {
        if (!isOn)
        {
            return;
        }

        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = false;
        }

        boxCol.enabled = false;
        isActive = false;

        blinkVisible = true;
        blinkTimer = 0f;

        stateTimer = 0f;

        isFirstSprite = true;
    }

    private void UpdateStarts(float remainingTime)
    {
        if (remainingTime <= 1f)
        {
            Sprite newSprite = isFirstSprite ? spritesStart1[idx] : spritesStart2[idx];

            foreach (SpriteRenderer sr in srStarts)
            {
                sr.enabled = true;
                sr.sprite = newSprite;
            }

            isFirstSprite = !isFirstSprite;
        }
    }

    private void UpdateSign_Easy(float remainingTime)
    {
        if (remainingTime <= 5f)
        {
            srSign.sprite = spritesSignEasy[Mathf.Clamp((int)remainingTime, 0, spritesSignEasy.Length - 1)];

            if (remainingTime <= 2f)
            {
                blinkTimer += Time.deltaTime;

                if (blinkTimer >= blinkFrequency)
                {
                    blinkVisible = !blinkVisible;
                    srSign.enabled = blinkVisible;
                    blinkTimer = 0f;
                }
            }
            else
            {
                srSign.enabled = true;
            }
        }
    }
    private void UpdateSign_Medium(float remainingTime)
    {
        if (remainingTime <= 2f)
        {
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkFrequency)
            {
                srSign.enabled = blinkVisible;
                blinkTimer = 0f;
                blinkVisible = !blinkVisible;
            }
        }
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
        CancelInvoke();

        timer = 0f;
        stateTimer = 0f;

        blinkTimer = 0f;
        blinkVisible = true;

        isActive = false;
        isFirstSprite = true;

        boxCol.enabled = false;

        srSign.enabled = false;

        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = false;
            sr.sprite = sprites1[idx];
        }

        foreach (SpriteRenderer sr in srStarts)
        {
            sr.enabled = false;
            sr.sprite = spritesStart1[idx];
        }
    }
}
