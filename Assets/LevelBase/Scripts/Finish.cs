
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Finish : MonoBehaviour
{
    [Header("Edit")]
    public float levelDelay = 1f;

    [Header("Main")]
    public Timer timer;
    public Spawn spawn;

    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite sprite1;
    public Sprite sprite2;

    public float frequency = 0.5f;
    public bool isWithStick = false;

    private float sprite_timer;
    private bool isFirstSprite = false;


    void Update()
    {
        sprite_timer += Time.deltaTime;

        if (sprite_timer >= frequency)
        {
            isFirstSprite = !isFirstSprite;
            spriteRenderer.sprite = isFirstSprite ? sprite1 : sprite2;

            sprite_timer = 0;
        }
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(levelDelay);

        if (LevelManager.Instance != null)
        {
            SceneManager.LoadScene("HandAdj");
            //LevelManager.Instance.LoadNextLevel(timer.timer);
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer.TurnOff();

            spawn.TurnOffAll();

            StartCoroutine(LoadNextLevel());
        }
    }
}
