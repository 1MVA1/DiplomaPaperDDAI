
using UnityEditor;
using UnityEngine;


public class Decor : MonoBehaviour
{
    [Header("Edit")]
    [Range(0, 5)]
    public int idx = 0;

    [Header("Main")]
    public SpriteRenderer sr;

    [Header("Sprites")]
    public Sprite[] sprites;


    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            sr.sprite = sprites[idx];
        };
#endif
    }
}
