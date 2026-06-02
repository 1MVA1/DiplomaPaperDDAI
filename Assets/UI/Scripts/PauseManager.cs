using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum PauseStates
{
    Pause,
    Settings,
    Confirm,
    Null
}


public class PauseManager: UIManagerBase
{
    public static PauseManager Instance;

    [Header("Pause")]
    public Button[] pause_bns;
    public TMP_Text[] pause_txts;

    [Header("Debug")]
    public bool isInGame = false;

    private PauseStates state = PauseStates.Null;
    // ¬ы действительно хотите вернутьс€ в меню?

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

    protected void Start()
    {
        //LevelManager.Instance.OnLevelReady += ChangeInGame;
    }

    private void ChangeInGame()
    {
        isInGame = true;
    }

    //TurnOns
    public void TurnOnPause()
    {
        TurnOffPanel();

        //
        //settings_txts[0].text = LocalizationManager.GetText(pauseTable, "pause_text");
        //

        state = PauseStates.Pause;
        SetActivePanel(pause_bns, pause_txts, true);
    }

    public void TurnOnBackToMenuConfirm()
    {
        TurnOffPanel();

        //
        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(BackToMenu);

        //confirm_txts[0].text = LocalizationManager.GetText(pauseTable, "back_text");
        //

        state = PauseStates.Confirm;
        SetActivePanel(confirm_bns, confirm_txts, true);
    }

    public void TurnOnExitConfirm()
    {
        TurnOffPanel();

        //
        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(ExitGame);

        //confirm_txts[0].text = LocalizationManager.GetText(generalTable, "exit_text");
        //

        state = PauseStates.Confirm;
        SetActivePanel(confirm_bns, confirm_txts, true);
    }

    public void TurnOffPanel()
    {
        switch (state)
        {
            case PauseStates.Pause:
                SetActivePanel(pause_bns, pause_txts, false);
            break;

            case PauseStates.Confirm:
                SetActivePanel(confirm_bns, confirm_txts, false);
            break;
        }
    }

    //Special
    public void OnContinuePressed()
    {
        TurnOffPanel();

        state = PauseStates.Null;

        Time.timeScale = 1f;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");

        Time.timeScale = 1f;

        Destroy(gameObject);
    }

    public override void OnEscapePressed()
    {
        if (state == PauseStates.Null && isInGame)
        {
            Time.timeScale = 0f;

            TurnOnPause();
        }
        else if (state == PauseStates.Pause)
            OnContinuePressed();
        else if (isInGame)
            TurnOnPause();
    }
}