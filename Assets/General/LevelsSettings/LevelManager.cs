using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public delegate void LevelReadyHandler();
    public event LevelReadyHandler OnLevelReady;

    [Header("Database")]
    public LevelDatabase levelDatabase;

    [Header("Level info")]
    public int levelIndex;

    public int enemyDifficulty;
    public int platformingDifficulty;

    private LevelConfig enemyConfig;
    private LevelConfig platformingConfig;

    //General and saves
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //Manage levels
    public void LoadLevel(int levelIndex)
    {
        if (levelDatabase == null)
        {
            Debug.LogError("Database missing!");
            return;
        }

        if (levelIndex >= levelDatabase.levels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        var level = levelDatabase.levels[levelIndex];

        if (level.enemyConfigs == null)
            Debug.LogWarning("Enemy config not found!");
        else if (level.enemyConfigs.Count != 5)
             Debug.LogWarning("Enemy configsData has not enough members!");
        else
            enemyConfig = level.enemyConfigs[enemyDifficulty];

        if (level.platformingConfigs == null)
            Debug.LogWarning("Platforming config not found!");
        else if (level.platformingConfigs.Count != 5)
            Debug.LogWarning("Platforming configsData has not enough members!");
        else
            platformingConfig = level.platformingConfigs[platformingDifficulty];

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(level.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (enemyConfig != null)
            SpawnObjects(enemyConfig);

        if (platformingConfig != null)
            SpawnObjects(platformingConfig);

        OnLevelReady?.Invoke();
    }

    void SpawnObjects(LevelConfig config)
    {
        foreach (var data in config.data)
        {
            var obj = Instantiate(data.prefab, data.position, Quaternion.identity);

            if (obj.TryGetComponent<IAdjustableDifficulty>(out var adjustable))
                adjustable.ApplyDifficulty(data.difficulty);
        }
    }

    public void LoadNextLevel() 
    {
        levelIndex++;

        LoadLevel(levelIndex);
    }
}