using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public delegate void SaveLoadedHandler();
    public event SaveLoadedHandler OnSaveLoaded;

    [Header("Variables to save")]
    public int levelIndex;

    public int enemyDifficulty;
    public int platformingDifficulty;

    public float musicLevel;
    public float soundLevel;

    public Language language;

    [Header("Debug")]
    public bool isDebug = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    } 

    void Start()
    {
        if (isDebug)
            PlayerPrefs.DeleteAll();

        levelIndex = PlayerPrefs.GetInt("levelIndex", -1);
        enemyDifficulty = PlayerPrefs.GetInt("enemyDifficulty", 2);
        platformingDifficulty = PlayerPrefs.GetInt("platformingDifficulty", 2);

        musicLevel = PlayerPrefs.GetFloat("musicLevel", 0.5f);
        soundLevel = PlayerPrefs.GetFloat("soundLevel", 0.5f);

        language = (Language)PlayerPrefs.GetInt("language", (int)Language.En);

        OnSaveLoaded?.Invoke();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("levelIndex", levelIndex);
        PlayerPrefs.SetInt("enemyDifficulty", enemyDifficulty);
        PlayerPrefs.SetInt("platformingDifficulty", platformingDifficulty);

        PlayerPrefs.SetFloat("musicLevel", musicLevel);
        PlayerPrefs.SetFloat("soundLevel", soundLevel);
        PlayerPrefs.SetInt("language", (int)language);

        PlayerPrefs.Save();
    }
}
