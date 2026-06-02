
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Wall : MonoBehaviour
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    [Range(2, 34)]
    public int size = 2;
    [Range(0, 2)]
    public int idx = 0;

    [Header("Main")]
    public GameObject prefab;
    public BoxCollider2D boxCol;

    [Header("Sprites")]
    public Sprite[] sprites1;
    public Sprite[] sprites2;
    public Sprite[] sprites3;

    public float spacing = 0.54f;


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

        boxCol.size = new Vector3(boxCol.size.x, size * spacing, 1.0f);

        Sprite[] sprites = { };

        switch (idx)
        {
            case 0: sprites = sprites1; break;
            case 1: sprites = sprites2; break;
            case 2: sprites = sprites3; break;
        }

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

            float y = (i - centerOffset) * spacing;
            obj.transform.localPosition = new Vector3(0f, y, 0f);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (i == 0)
            {
                sr.sprite = sprites[0];
            }
            else if (i == count - 1)
            {
                sr.sprite = sprites[0];
                obj.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
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
}
