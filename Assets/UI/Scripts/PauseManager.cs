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
    public Image img;

    [HideInInspector]
    public bool isInGame = false;

    private PlayerMovement player;

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

        state = PauseStates.Pause;
        img.gameObject.SetActive(true);
        SetActivePanel(pause_bns, pause_txts, true);
    }

    public void TurnOnBackToMenuConfirm()
    {
        TurnOffPanel();

        //
        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(BackToMenu);

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
        img.gameObject.SetActive(false);
        TurnOffPanel();

        state = PauseStates.Null;

        Time.timeScale = 1f;
    }

    public void BackToMenu()
    {
        isInGame = false;

        SceneManager.LoadScene("Menu");

        Time.timeScale = 1f;
    }

    public override void OnEscapePressed()
    {
        if (state == PauseStates.Null && isInGame)
        {
            TurnOnPause();
        }
        else if (state == PauseStates.Pause)
            OnContinuePressed();
        else if (isInGame)
            TurnOnPause();
    }
}