
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class PrefabLineSpawner : MonoBehaviour
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    [Range(0, 1)]
    public int idx = 0;

    [Header("Main")]
    public GameObject prefab;

    [Header("Sprites")]
    public Sprite[] spritesA;
    public Sprite[] spritesB;

    public float start = -9f;
    public float finish = 9f;

    public float spacing = 0.54f;

    public float switchInterval = 0.7f;

    private float timer;
    private bool state = false;

    private readonly List<SpriteRenderer> renderers = new();


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
        if (prefab == null || spacing <= 0f)
            return;

        ClearChildren();

        int index = 0;

        for (float x = start; x <= finish; x += spacing)
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

            obj.transform.localPosition = new Vector3(x, 0f, 0f);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            sr.sprite = spritesA[idx];

            index++;
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

    public void Start()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();

            renderers.Add(sr);
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer = 0f;

            state = !state;

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                {
                    continue;
                }

                bool even = i % 2 == 0;

                Sprite sprite;

                if (state)
                    sprite = spritesB[idx];
                else
                    sprite = spritesA[idx];

                renderers[i].sprite = sprite;
            }
        }
    }
}
