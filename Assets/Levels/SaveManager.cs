using UnityEngine;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public MenuManager menu;
    public Background bg;

    [Header("Debug")]
    public bool deleteOldSave = false;
    public int levelsInPart = 4;

    [HideInInspector]
    public int levelIndex;

    [HideInInspector]
    public int actIdx;
    [HideInInspector]
    public int prevActIdx;

    [HideInInspector]
    public int enemyDiff;
    [HideInInspector]
    public int platDiff;

    [HideInInspector]
    public int actionPlat;
    [HideInInspector]
    public int actionEnemy;

    [HideInInspector]
    public int prevActionPlat;
    [HideInInspector]
    public int prevActionEnemy;

    [HideInInspector]
    public float prevTime;
    [HideInInspector]
    public float prevEmaTime;
    [HideInInspector]
    public float prevDeltaTime;

    [HideInInspector]
    public int prevPlatDeath;
    [HideInInspector]
    public float prevEmaPlatDeath;
    [HideInInspector]
    public int prevDeltaPlatDeath;
    [HideInInspector]
    public int prevDiffPlat;

    [HideInInspector]
    public int prevEnemyDeath;
    [HideInInspector]
    public float prevEmaEnemyDeath;
    [HideInInspector]
    public int prevDeltaEnemyDeath;
    [HideInInspector]
    public int prevDiffEnemy;

    [HideInInspector]
    public int deathPlatNow = 0;
    [HideInInspector]
    public int deathEnemyNow = 0;


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
        if (deleteOldSave)
        {
            PlayerPrefs.DeleteAll();
        }

        levelIndex = PlayerPrefs.GetInt("levelIndex", -1);
        actIdx = PlayerPrefs.GetInt("actIdx", 4);
        prevActIdx = PlayerPrefs.GetInt("prevActIdx", 4);

        enemyDiff = PlayerPrefs.GetInt("enemyDiff", 2);
        platDiff = PlayerPrefs.GetInt("platDiff", 2);

        actionPlat = PlayerPrefs.GetInt("actionPlat", 4);
        actionEnemy = PlayerPrefs.GetInt("actionEnemy", 4);
        prevActionPlat = PlayerPrefs.GetInt("prevActionPlat", 4);
        prevActionEnemy = PlayerPrefs.GetInt("prevActionEnemy", 4);

        prevTime = PlayerPrefs.GetFloat("prevTime", 0f);
        prevEmaTime = PlayerPrefs.GetFloat("prevEmaTime", 0f);
        prevDeltaTime = PlayerPrefs.GetFloat("prevDeltaTime", 0f);

        prevPlatDeath = PlayerPrefs.GetInt("prevPlatDeath", 0);
        prevEmaPlatDeath = PlayerPrefs.GetFloat("prevEmaPlatDeath", 0f);
        prevDeltaPlatDeath = PlayerPrefs.GetInt("prevDeltaPlatDeath", 0);
        prevDiffPlat = PlayerPrefs.GetInt("prevDiffPlat", 0);

        prevEnemyDeath = PlayerPrefs.GetInt("prevEnemyDeath", 0);
        prevEmaEnemyDeath = PlayerPrefs.GetFloat("prevEmaEnemyDeath", 0f);
        prevDeltaEnemyDeath = PlayerPrefs.GetInt("prevDeltaEnemyDeath", 0);
        prevDiffEnemy = PlayerPrefs.GetInt("prevDiffEnemy", 0);

        deathPlatNow = PlayerPrefs.GetInt("deathPlatNow", 0);
        deathEnemyNow = PlayerPrefs.GetInt("deathEnemyNow", 0);

        int helper = levelIndex / levelsInPart;

        if (levelIndex != -1 && helper != 0)
        {
            bg.idx = helper;
            bg.Generate();
        }

        menu.Init();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("levelIndex", levelIndex);
        PlayerPrefs.SetInt("actIdx", actIdx);
        PlayerPrefs.SetInt("prevActIdx", prevActIdx);

        PlayerPrefs.SetInt("enemyDiff", enemyDiff);
        PlayerPrefs.SetInt("platDiff", platDiff);

        PlayerPrefs.SetInt("actionPlat", actionPlat);
        PlayerPrefs.SetInt("actionEnemy", actionEnemy);
        PlayerPrefs.SetInt("prevActionPlat", prevActionPlat);
        PlayerPrefs.SetInt("prevActionEnemy", prevActionEnemy);

        PlayerPrefs.SetFloat("prevTime", prevTime);
        PlayerPrefs.SetFloat("prevEmaTime", prevEmaTime);
        PlayerPrefs.SetFloat("prevDeltaTime", prevDeltaTime);

        PlayerPrefs.SetInt("prevPlatDeath", prevPlatDeath);
        PlayerPrefs.SetFloat("prevEmaPlatDeath", prevEmaPlatDeath);
        PlayerPrefs.SetInt("prevDeltaPlatDeath", prevDeltaPlatDeath);
        PlayerPrefs.SetInt("prevDiffPlat", prevDiffPlat);

        PlayerPrefs.SetInt("prevEnemyDeath", prevEnemyDeath);
        PlayerPrefs.SetFloat("prevEmaEnemyDeath", prevEmaEnemyDeath);
        PlayerPrefs.SetInt("prevDeltaEnemyDeath", prevDeltaEnemyDeath);
        PlayerPrefs.SetInt("prevDiffEnemy", prevDiffEnemy);

        PlayerPrefs.SetInt("deathPlatNow", deathPlatNow);
        PlayerPrefs.SetInt("deathEnemyNow", deathEnemyNow);

        PlayerPrefs.Save();
    }

    public void SetNewDiffs(int platAct, int enemyAct)
    {
        enemyDiff = Mathf.Clamp(enemyDiff + enemyAct, 0, 4);

        platDiff = Mathf.Clamp(platDiff + platAct, 0, 4);
    }
}
