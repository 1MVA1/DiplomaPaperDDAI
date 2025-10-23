using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public delegate void LevelReadyHandler();
    public event LevelReadyHandler OnLevelReady;

    [Header("Database")]
    public LevelDataBase levelDataBase;

    private GameObjectConfig enemyConfig;
    private GameObjectConfig platformingConfig;

    public bool wasInMenu = false;

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
        wasInMenu = true;

        if (levelDataBase == null)
        {
            Debug.LogError("Database missing!");
            return;
        }

        if (levelIndex <= -1 || levelIndex >= levelDataBase.levels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        var level = levelDataBase.levels[levelIndex];

        if (level.config == null)
            Debug.LogWarning("Ñonfig not found!");

        if (level.config.enemyConfig.Count != 5)
            Debug.LogWarning("EnemyConfigs has not enough members!");
        else
            enemyConfig = level.config.enemyConfig[SaveManager.Instance.enemyDifficulty];

        if (level.config.platformingConfig.Count != 5)
            Debug.LogWarning("PlatformingConfigs has not enough members!");
        else
            platformingConfig = level.config.platformingConfig[SaveManager.Instance.platformingDifficulty];

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

    void SpawnObjects(GameObjectConfig config)
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
        SaveManager.Instance.levelIndex++;

        LoadLevel(SaveManager.Instance.levelIndex);
    }
}