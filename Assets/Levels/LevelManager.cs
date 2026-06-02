using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [Header("Debug")]
    public bool wasInMenu = false;

    [Header("Config")]
    public LevelsConfig config;

    private int levelIndex = 0;

    // 
    public delegate void LevelReadyHandler();
    public event LevelReadyHandler OnLevelReady;


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

    public void LoadLevel(int levelIndex_)
    {
        wasInMenu = true;
        levelIndex = levelIndex_;

        if (SaveManager.Instance.levelIndex > 18)
        {
            SceneManager.LoadScene("Menu");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(config.levels[levelIndex].sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        var level = config.levels[levelIndex];

        if (level.spikeConfigs != null)
        {
            foreach (var data in level.spikeConfigs[SaveManager.Instance.platDiff].data)
            {
                var obj = Instantiate(config.spike, data.position, Quaternion.identity);

                if (obj.TryGetComponent<IApplyDiff_Spike>(out var adjustable))
                {
                    adjustable.ApplyDiff(data.difficulty, data.size, data.rotation);
                }
            }
        }

        if (level.sawConfigs != null)
        {
            foreach (var data in level.sawConfigs[SaveManager.Instance.platDiff].data)
            {
                var obj = Instantiate(config.saw, data.position, Quaternion.identity);

                if (obj.TryGetComponent<IApplyDiff_Saw>(out var adjustable))
                {
                    adjustable.ApplyDiff(data.difficulty, data.size, data.speed, data.timeStop, data.acceleration,
                        data.maxSpeed, data.points, data.isCycle);
                }
            }
        }

        if (level.voltZoneConfigs != null)
        {
            foreach (var data in level.voltZoneConfigs[SaveManager.Instance.platDiff].data)
            {
                var obj = Instantiate(config.voltZone, data.position, Quaternion.identity);

                if (obj.TryGetComponent<IApplyDiff_VoltZone>(out var adjustable))
                {
                    adjustable.ApplyDiff(data.difficulty, data.size, data.isHorizontal);
                }
            }
        }

        if (level.enemyConfigs != null)
        {
            if (level.enemyConfigs[SaveManager.Instance.enemyDiff].dataCrab != null)
            {
                foreach (var data in level.enemyConfigs[SaveManager.Instance.enemyDiff].dataCrab)
                {
                    var obj = Instantiate(config.crab, data.position, Quaternion.identity);

                    if (obj.TryGetComponent<IApplyDiff_Enemy>(out var adjustable))
                    {
                        adjustable.ApplyDiff(data.difficulty, data.isMovingRight);
                    }
                }
            }

            if (level.enemyConfigs[SaveManager.Instance.enemyDiff].dataShooter != null)
            {
                foreach (var data in level.enemyConfigs[SaveManager.Instance.enemyDiff].dataShooter)
                {
                    var obj = Instantiate(config.shooter, data.position, Quaternion.identity);

                    if (obj.TryGetComponent<IApplyDiff_Enemy>(out var adjustable))
                    {
                        adjustable.ApplyDiff(data.difficulty, data.isMovingRight);
                    }
                }
            }

            if (level.enemyConfigs[SaveManager.Instance.enemyDiff].dataStalker != null)
            {
                foreach (var data in level.enemyConfigs[SaveManager.Instance.enemyDiff].dataStalker)
                {
                    var obj = Instantiate(config.stalker, data.position, Quaternion.identity);

                    if (obj.TryGetComponent<IApplyDiff_Enemy>(out var adjustable))
                    {
                        adjustable.ApplyDiff(data.difficulty, data.isMovingRight);
                    }
                }
            }
        }

        OnLevelReady?.Invoke();
    }

    public void LoadNextLevel(float time) 
    {
        var sm = SaveManager.Instance;

        if (DDA_Connection.Instance.isConnected)
        {
            if (sm.levelIndex < 3)
            {
                sm.levelIndex++;

                LoadLevel(sm.levelIndex);

                return;
            }

            float timeEma = time * 0.4f + 0.6f * sm.prevEmaTime;
            float deltaTime = time - sm.prevTime;

            float deathEmaPlat = sm.deathPlatNow * 0.4f + 0.6f * sm.prevEmaPlatDeath;
            int deltaDeathPlat = sm.deathPlatNow - sm.prevPlatDeath;

            float deathEmaEnemy = sm.deathEnemyNow * 0.4f + 0.6f * sm.prevEmaEnemyDeath;
            int deltaDeathEnemy = sm.deathEnemyNow - sm.prevEnemyDeath;

            float done = 0.0f;

            if (sm.levelIndex == 18)
            {
                done = 1.0f;
            }

            int actIdx = DDA_Connection.Instance.SendState(time, timeEma, deltaTime, sm.deathPlatNow, deathEmaPlat,
                deltaDeathPlat, sm.actionPlat, sm.platDiff, sm.deathEnemyNow, deathEmaEnemy, deltaDeathEnemy,
                sm.actionEnemy, sm.enemyDiff, done, sm.prevActIdx, sm.actIdx, sm.prevTime, sm.prevEmaTime, 
                sm.prevDeltaTime, sm.prevPlatDeath, sm.prevEmaPlatDeath, sm.prevDeltaPlatDeath, sm.prevActionPlat, 
                sm.prevDiffPlat, sm.prevEnemyDeath, sm.prevEmaEnemyDeath, sm.prevDeltaEnemyDeath, sm.prevActionEnemy,
                sm.prevDiffEnemy);

            sm.prevTime = time;
            sm.prevEmaTime = timeEma;
            sm.prevDeltaTime = deltaTime;

            sm.prevPlatDeath = sm.deathPlatNow;
            sm.prevEmaPlatDeath = deathEmaPlat;
            sm.prevDeltaPlatDeath = deltaDeathPlat;
            sm.prevActionPlat = sm.actionPlat;
            sm.prevDiffPlat = sm.platDiff;

            sm.prevEnemyDeath = sm.deathEnemyNow;
            sm.prevEmaEnemyDeath = deathEmaEnemy;
            sm.prevDeltaEnemyDeath = deltaDeathEnemy;
            sm.prevActionEnemy = sm.actionEnemy;
            sm.prevDiffEnemy = sm.enemyDiff;

            sm.prevActIdx = sm.actIdx;

            sm.actIdx = actIdx;

            sm.platDiff = actIdx / 3;
            sm.enemyDiff = actIdx % 3;

            sm.levelIndex++;

            sm.deathPlatNow = 0;
            sm.deathEnemyNow = 0;

            LoadLevel(sm.levelIndex);
        }
        else
        {
            sm.levelIndex++;

            SceneManager.LoadScene("HandAdj");
        }
    }

    public void LoadNextLevel()
    {
        var sm = SaveManager.Instance;

        sm.levelIndex++;

        Debug.Log(sm.levelIndex);

        LoadLevel(sm.levelIndex);
    }
}
