using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

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

        LevelManager.Instance.levelIndex = PlayerPrefs.GetInt("levelIndex", -1);
        LevelManager.Instance.enemyDifficulty = PlayerPrefs.GetInt("enemyDifficulty", 3);
        LevelManager.Instance.platformingDifficulty = PlayerPrefs.GetInt("platformingDifficulty", 3);

        AudioManager.Instance.musicLevel = PlayerPrefs.GetFloat("musicLevel", 0.5f);
        AudioManager.Instance.soundLevel = PlayerPrefs.GetFloat("soundLevel", 0.5f);

        LocalizationManager.language = (Language)PlayerPrefs.GetInt("language", (int)Language.En);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("levelIndex", LevelManager.Instance.levelIndex);
        PlayerPrefs.SetInt("enemyDifficulty", LevelManager.Instance.enemyDifficulty);
        PlayerPrefs.SetInt("platformingDifficulty", LevelManager.Instance.platformingDifficulty);

        PlayerPrefs.SetFloat("musicLevel", AudioManager.Instance.musicLevel);
        PlayerPrefs.SetFloat("soundLevel", AudioManager.Instance.soundLevel);
        PlayerPrefs.SetInt("language", (int)LocalizationManager.language);

        PlayerPrefs.Save();
    }
}
