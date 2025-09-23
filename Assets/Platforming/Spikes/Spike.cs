using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Easy;

    public Vector3 scaleEasy = new Vector3(1f, 1f, 1f);
    public Vector3 scaleMedium = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 scaleHard = new Vector3(1.4f, 1.4f, 1f);

    private void Awake()
    {
        Vector3 scale = Vector3.one;

        switch (difficulty)
        {
            case Difficulty.Easy:
                scale = scaleEasy;
                break;
            case Difficulty.Medium:
                scale = scaleMedium;
                break;
            case Difficulty.Hard:
                scale = scaleHard;
                break;
        }

        transform.localScale = scale;
    }
}