using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio settings")]
    public float fadeDuration = 1f;

    public float musicLevel = 1f;
    public float soundLevel = 1f;

    [Header("Audio settings")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music")]
    public AudioClip menuMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        audioMixer.SetFloat("musicLevel", CalculateDB(musicLevel));
    }

    public void ChangeMusicLevel(float newLevel)
    {
        musicLevel = newLevel;

        audioMixer.SetFloat("musicLevel", CalculateDB(musicLevel));
    }

    public void ChangeSoundLevel(float newLevel)
    {
        soundLevel = newLevel;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Menu"))
            PlayMusic(menuMusic);
    }

    private float CalculateDB(float level)
    {
        if (level <= 0f)
            return -80f;
        else
            return Mathf.Log10(musicLevel) * 20f;
    }

    //Music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) 
            return;

        if (musicSource.clip == null)
            StartCoroutine(StartNewMusic(clip));
        else
            StartCoroutine(FadeMusic(clip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        StartCoroutine(StartNewMusic(newClip));
    }

    private IEnumerator StartNewMusic(AudioClip newClip)
    {
        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
    }
}