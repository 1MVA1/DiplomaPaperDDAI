
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Background : MonoBehaviour
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    [Range(0, 2)]
    public int idx = 0;

    [Header("Edit Other")]
    public float startX = -8.5f;
    public float finishX = 8.5f;

    public float startY = 5f;
    public float finishY = -5f;

    public float spacingX = 1.2f;
    public float spacingY = 1.2f;

    public int unusual_row = 5;

    private int idx_unusual = 0;

    [Header("Main")]
    public GameObject prefab;

    [Header("Sprites")]
    public Sprite[] sprites1;
    public Sprite[] unusual_sprites1;
    public Sprite[] sprites2;
    public Sprite[] unusual_sprites2;
    public Sprite[] sprites3;
    public Sprite[] unusual_sprites3;


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
    public void Generate()
    {
        ClearChildren();

        Sprite[] sprites = sprites2;
        Sprite[] unusual_sprites = unusual_sprites2;
        idx_unusual = 0;

        switch (idx)
        {
            case 0:
                sprites = sprites1;
                unusual_sprites = unusual_sprites1;
            break;

            case 1:
                sprites = sprites2;
                unusual_sprites = unusual_sprites2;
            break;

            case 2:
                sprites = sprites3;
                unusual_sprites = unusual_sprites3;
            break;
        }

        int row = 0;

        for (float y = startY; y >= finishY; y -= spacingY)
        {
            for (float x = startX; x <= finishX; x += spacingX)
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

                obj.transform.localPosition = new Vector3(x, y, 0f);

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

                if (row == unusual_row)
                {
                    sr.sprite = unusual_sprites[idx_unusual];
                    idx_unusual++;

                    if (idx_unusual == unusual_sprites.Length)
                    {
                        idx_unusual = 0;
                    }
                }
                else if (row < unusual_row)
                {
                    sr.sprite = sprites[0];
                }
                else
                {
                    sr.sprite = sprites[1];
                }
            }

            row++;
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
