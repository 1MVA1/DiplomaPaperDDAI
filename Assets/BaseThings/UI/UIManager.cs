using UnityEngine;


public enum UIStates
{
    MainMenu,
    NewGame,
    Settings,
    Pause,
    Null
}

public class UIManager: MonoBehaviour
{
    public static UIManager Instance;

    private PlayerInputActions inputActions;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject newgamePanel;
    public GameObject settingsPanel;
    public GameObject pausePanel;

    private UIStates state = UIStates.MainMenu;

    private int enemyDifficulty;
    private int platformDifficulty;

    private bool isInGame = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
        inputActions.Player.Pause.performed += ctx => OnPausePressed();
    }

    void Start() {
        SetNewActivePanel(true);
    }

    public void TurnOnMainMenu() => ChangeState(UIStates.MainMenu);
    public void TurnOnNewGame()
    {
        enemyDifficulty = 3;
        platformDifficulty = 3;

        ChangeState(UIStates.NewGame);
    }
    public void TurnOnSettings() => ChangeState(UIStates.Settings);

    public void ChangeState(UIStates newState)
    {
        SetNewActivePanel(false);

        state = newState;

        SetNewActivePanel(true);
    }

    public void SetNewActivePanel(bool isActive)
    {
        switch (state)
        {
            case UIStates.MainMenu:
                mainMenuPanel.SetActive(isActive);
                break;

            case UIStates.NewGame:
                newgamePanel.SetActive(isActive);
                break;

            case UIStates.Settings:
                settingsPanel.SetActive(isActive);
                break;

            case UIStates.Pause:
                pausePanel.SetActive(isActive);
                break;
        }
    }

    public void OnContinuePressed()
    {
        ChangeState(UIStates.Null);
    }

    public void OnExitPressed() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
    Application.Quit(); // закрыть игру в билде
#endif
    }

    public void SelectEnemyDifficulty(int difficulty)
    {
        enemyDifficulty = difficulty;
        Debug.Log($"Enemy difficulty: {difficulty}");
    }

    public void SelectPlatformDifficulty(int difficulty)
    {
        platformDifficulty = difficulty;
        Debug.Log($"Platforming difficulty: {difficulty}");
    }

    public void OnConfirmPressed()
    {
        ChangeState(UIStates.Null);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.enemyDifficulty = enemyDifficulty;
            LevelManager.Instance.platformingDifficulty = platformDifficulty;

            LevelManager.Instance.LoadLevel(0);
        }
        else
            Debug.Log($"LevelManager is missing");
    }

    private void OnPausePressed()
    {
        if (isInGame)
        {
            if (state == UIStates.Pause) 
            {
                ChangeState(UIStates.Null);

                Time.timeScale = 1f;
            }
            else
            {
                ChangeState(UIStates.Pause);

                Time.timeScale = 0f;
            }
        }
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();
}