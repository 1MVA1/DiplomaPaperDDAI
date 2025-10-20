using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Localization")]
    public LocalizationTable pauseTable;

    [Header("Panels")]
    public GameObject[] pause;

    private PauseStates state = PauseStates.Null;

    [Header("Debug")]
    public bool isInGame = false;

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

    //TurnOns
    public void TurnOnPause()
    {
        TurnOffPanel();

        state = PauseStates.Pause;
        SetActivePanel(pause, true);
    }

    public void TurnOnSettings()
    {
        TurnOffPanel();

        state = PauseStates.Settings;
        SetActivePanel(settings, true);
    }

    public void TurnOnBackToMenuConfirm()
    {
        TurnOffPanel();

        //
        var bn = confirm[1].GetComponent<Button>();

        if (bn != null)
        {
            bn.onClick.RemoveAllListeners();
            bn.onClick.AddListener(BackToMenu);
        }

        var txt = confirm[0].GetComponent<TMP_Text>();

        if (txt != null)
            txt.text = LocalizationManager.GetText(pauseTable, "back_text");
        //

        state = PauseStates.Confirm;
        SetActivePanel(confirm, true);
    }

    public void TurnOnExitConfirm()
    {
        TurnOffPanel();

        //
        var bn = confirm[1].GetComponent<Button>();

        if (bn != null)
        {
            bn.onClick.RemoveAllListeners();
            bn.onClick.AddListener(ExitGame);
        }

        var txt = confirm[0].GetComponent<TMP_Text>();

        if (txt != null)
            txt.text = LocalizationManager.GetText(generalTable, "exit_text");
        //

        state = PauseStates.Confirm;
        SetActivePanel(confirm, true);
    }

    public void TurnOffPanel()
    {
        switch (state)
        {
            case PauseStates.Pause:
                SetActivePanel(pause, false);
                break;

            case PauseStates.Settings:
                SetActivePanel(settings, false);
                break;

            case PauseStates.Confirm:
                SetActivePanel(confirm, false);
                break;
        }
    }

    //Localization
    public override void ChangeLanguage(Language newLanguage)
    {
        //Main
        txts[0].text = LocalizationManager.GetText(generalTable, "continue");
        txts[1].text = LocalizationManager.GetText(generalTable, "settings");
        txts[2].text = LocalizationManager.GetText(pauseTable, "back_menu");
        txts[3].text = LocalizationManager.GetText(generalTable, "exit");

        //Confirm
        txts[4].text = LocalizationManager.GetText(generalTable, "confirm");
        txts[5].text = LocalizationManager.GetText(generalTable, "back");

        //Settings
        txts[6].text = LocalizationManager.GetText(generalTable, "language_text");
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

        Destroy(gameObject);
    }

    public override void OnEscapePressed()
    {
        if (state == PauseStates.Null && isInGame)
        {
            Time.timeScale = 1f;

            TurnOnPause();
        }
        else if (state == PauseStates.Pause)
            OnContinuePressed();
        else
            TurnOnPause();
    }
}