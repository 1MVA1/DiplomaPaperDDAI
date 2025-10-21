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

    [Header("Buttons")]
    public Button[] pause_bns;

    [Header("Texts")]
    public TMP_Text[] pause_txts;

    [Header("Button texts")]
    public TMP_Text[] pause_bn_txts;

    [Header("Debug")]
    public bool isInGame = false;

    private PauseStates state = PauseStates.Null;

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

        //
        settings_txts[0].text = LocalizationManager.GetText(pauseTable, "pause_text");
        //

        state = PauseStates.Pause;
        SetActivePanel(pause_bns, pause_txts, true);
    }

    public void TurnOnSettings()
    {
        TurnOffPanel();

        //
        settings_txts[0].text = LocalizationManager.GetText(generalTable, "settings_text");

        musicSlider.gameObject.SetActive(true);
        soundSlider.gameObject.SetActive(true);
        //

        state = PauseStates.Settings;
        SetActivePanel(settings_bns, settings_txts, true);
    }

    public void TurnOnBackToMenuConfirm()
    {
        TurnOffPanel();

        //
        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(BackToMenu);

        confirm_txts[0].text = LocalizationManager.GetText(pauseTable, "back_text");
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

        confirm_txts[0].text = LocalizationManager.GetText(generalTable, "exit_text");
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

            case PauseStates.Settings:
                SetActivePanel(settings_bns, settings_txts, false);

                musicSlider.gameObject.SetActive(false);
                soundSlider.gameObject.SetActive(false);
                break;

            case PauseStates.Confirm:
                SetActivePanel(confirm_bns, confirm_txts, false);
                break;
        }
    }

    //Localization
    public override void ChangeLanguage(Language newLanguage)
    {
        base.ChangeLanguage(newLanguage);

        //Main
        pause_bn_txts[0].text = LocalizationManager.GetText(generalTable, "continue");
        pause_bn_txts[1].text = LocalizationManager.GetText(generalTable, "settings");
        pause_bn_txts[2].text = LocalizationManager.GetText(pauseTable, "back_menu");
        pause_bn_txts[3].text = LocalizationManager.GetText(generalTable, "exit");

        //Confirm
        confirm_bn_txts[0].text = LocalizationManager.GetText(generalTable, "confirm");
        confirm_bn_txts[1].text = LocalizationManager.GetText(generalTable, "back");
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
        else
            TurnOnPause();
    }
}