using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum MenuStates
{
    Main,
    NewGame,
    Settings,
    Confirm
}

public class MenuManager: UIManagerBase
{
    [Header("Pause")]
    public GameObject pauseManagerPrefab;

    [Header("Localization")]
    public LocalizationTable menuTable;

    [Header("Buttons")]
    public Button[] main_bns;
    public Button[] newgame_bns;

    [Header("Texts")]
    public TMP_Text[] main_txts;
    public TMP_Text[] newgame_txts;

    [Header("Button texts")]
    public TMP_Text[] main_bn_txts;

    private MenuStates state = MenuStates.Main;

    private int enemyDifficulty;
    private int platformDifficulty;

    protected override void Start() 
    {
        base.Start();

        if (LevelManager.Instance.levelIndex == -1)
            main_bns[1].interactable = false;
        else
        {
            main_bns[0].onClick.RemoveAllListeners();
            main_bns[0].onClick.AddListener(TurnOnNewGameConfirm);
        }

        SetActivePanel(main_bns, main_txts, true);
    }

    //TurnOns
    public void TurnOnMain()
    {
        TurnOffPanel();

        state = MenuStates.Main;
        SetActivePanel(main_bns, main_txts, true);
    }

    public void TurnOnNewGame()
    {
        TurnOffPanel();

        enemyDifficulty = 3;
        platformDifficulty = 3;

        for (int i = 0; i <= 9; i++)
        {
            if (i == 2 || i == 7)
                newgame_bns[i].interactable = false;
            else
                newgame_bns[i].interactable = true;
        }

        //
        newgame_bns[10].onClick.RemoveAllListeners();
        newgame_bns[10].onClick.AddListener(StartGame);

        newgame_txts[0].text = LocalizationManager.GetText(menuTable, "newgame_text");
 
        //

        state = MenuStates.NewGame;
        SetActivePanel(newgame_bns, newgame_txts, true);
    }

    public void TurnOnSettings()
    {
        TurnOffPanel();

        state = MenuStates.Settings;
        SetActivePanel(settings_bns, settings_txts, true);
    }

    public void TurnOnExitConfirm()
    {
        TurnOffPanel();

        //
        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(ExitGame);

        confirm_txts[0].text = LocalizationManager.GetText(generalTable, "exit_text");
        //

        state = MenuStates.Confirm;
        SetActivePanel(confirm_bns, confirm_txts, true);
    }

    public void TurnOnNewGameConfirm()
    {
        TurnOffPanel();

        //
        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(TurnOnNewGame);

        confirm_txts[0].text = LocalizationManager.GetText(menuTable, "ngconfirm_text");
        //

        state = MenuStates.Confirm;
        SetActivePanel(confirm_bns, confirm_txts, true);
    }

    public void TurnOffPanel()
    {
        switch (state)
        {
            case MenuStates.Main:
                SetActivePanel(main_bns, main_txts, false);
                break;

            case MenuStates.NewGame:
                SetActivePanel(newgame_bns, newgame_txts, false);
                break;

            case MenuStates.Settings:
                SetActivePanel(settings_bns, settings_txts, false);
                break;

            case MenuStates.Confirm:
                SetActivePanel(confirm_bns, confirm_txts, false);
                break;
        }
    }

    //New game buttons
    public void SelectEnemyDifficulty(int difficulty)
    {
        newgame_bns[enemyDifficulty - 1].interactable = true;

        enemyDifficulty = difficulty;

        newgame_bns[enemyDifficulty - 1].interactable = false;
    }

    public void SelectPlatformDifficulty(int difficulty)
    {
        newgame_bns[platformDifficulty + 4].interactable = true;

        platformDifficulty = difficulty;

        newgame_bns[platformDifficulty + 4].interactable = false;
    }

    //Localization
    public override void ChangeLanguage(Language newLanguage)
    {
        base.ChangeLanguage(newLanguage);

        //Main
        main_bn_txts[0].text = LocalizationManager.GetText(menuTable, "newgame");
        main_bn_txts[1].text = LocalizationManager.GetText(generalTable, "continue");
        main_bn_txts[2].text = LocalizationManager.GetText(generalTable, "settings");
        main_bn_txts[3].text = LocalizationManager.GetText(generalTable, "exit");

        //Confirm
        confirm_bn_txts[0].text = LocalizationManager.GetText(generalTable, "confirm");
        confirm_bn_txts[1].text = LocalizationManager.GetText(generalTable, "back");

        //Newgame
        newgame_txts[1].text = LocalizationManager.GetText(menuTable, "enemy_text");
        newgame_txts[2].text = LocalizationManager.GetText(menuTable, "platforming_text");
    }

    //Special 
    public void StartGame()
    {
        if (LevelManager.Instance != null)
        {
            if (PauseManager.Instance == null)
                Instantiate(pauseManagerPrefab, Vector3.zero, Quaternion.identity);

            LevelManager.Instance.enemyDifficulty = enemyDifficulty;
            LevelManager.Instance.platformingDifficulty = platformDifficulty;

            LevelManager.Instance.LoadLevel(0);
        }
        else
            Debug.Log($"LevelManager is missing");
    }

    public override void OnEscapePressed()
    {
        if (state == MenuStates.Main)
            TurnOnExitConfirm();
        else
            TurnOnMain();
    }
}