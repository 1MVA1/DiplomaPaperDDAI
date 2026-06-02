
using UnityEditor;
using UnityEngine;


public class Spike : MonoBehaviour, IApplyDiff_Spike
{
    [Header("Edit")]
    public Diff diff;
    public float size = 1;

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

            ApplyDiff(diff, size, transform.localRotation.z);
        };
#endif
    }
    public void ApplyDiff(Diff diff_, float size_, float rotation_)
    {
        transform.localScale = new Vector3(size_, size_, 1f);
        transform.localRotation = Quaternion.Euler(0f, 0f, rotation_);

        diff = diff_;
        sr.sprite = sprites[(int)diff];
    }
}
