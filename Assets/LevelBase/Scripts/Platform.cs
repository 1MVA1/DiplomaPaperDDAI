
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Platform : MonoBehaviour
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    [Range(2, 10)]
    public int size = 2;

    [Header("Main")]
    public GameObject prefab;
    public BoxCollider2D boxCol;

    [Header("Sprites")]
    public Sprite[] sprites;

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
}
