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

    [Header("Panels")]
    public GameObject[] main;
    public GameObject[] newgame;

    private MenuStates state = MenuStates.Main;

    private int enemyDifficulty;
    private int platformDifficulty;

    protected override void Start() 
    {
        base.Start();

        if (LevelManager.Instance.levelIndex == -1)
            ChangeButton(main, 1, false);
        else
        {
            var bn = main[0].GetComponent<Button>();

            if (bn != null)
            {
                bn.onClick.RemoveAllListeners();
                bn.onClick.AddListener(TurnOnNewGameConfirm);
            }
        }

        SetActivePanel(main, true);
    }

    //TurnOns
    public void TurnOnMain()
    {
        TurnOffPanel();

        state = MenuStates.Main;
        SetActivePanel(main, true);
    }

    public void TurnOnNewGame()
    {
        TurnOffPanel();

        enemyDifficulty = 3;
        platformDifficulty = 3;

        //
        var bn = newgame[13].GetComponent<Button>();

        bn.onClick.RemoveAllListeners();
        bn.onClick.AddListener(StartGame);

        var text = newgame[0].GetComponent<TMP_Text>();

        if (text != null) {
            text.text = LocalizationManager.GetText(menuTable, "newgame_text");
        }
        //

        state = MenuStates.NewGame;
        SetActivePanel(newgame, true);
    }

    public void TurnOnSettings()
    {
        TurnOffPanel();

        state = MenuStates.Settings;
        SetActivePanel(settings, true);
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

        state = MenuStates.Confirm;
        SetActivePanel(confirm, true);
    }

    public void TurnOnNewGameConfirm()
    {
        TurnOffPanel();

        //
        var bn = confirm[1].GetComponent<Button>();

        if (bn != null)
        {
            bn.onClick.RemoveAllListeners();
            bn.onClick.AddListener(TurnOnNewGame);
        }

        var txt = confirm[0].GetComponent<TMP_Text>();

        if (txt != null)
            txt.text = LocalizationManager.GetText(menuTable, "ngconfirm_text");
        //

        state = MenuStates.Confirm;
        SetActivePanel(confirm, true);
    }

    public void TurnOffPanel()
    {
        switch (state)
        {
            case MenuStates.Main:
                SetActivePanel(main, false);
                break;

            case MenuStates.NewGame:
                SetActivePanel(newgame, false);
                break;

            case MenuStates.Settings:
                SetActivePanel(settings, false);
                break;

            case MenuStates.Confirm:
                SetActivePanel(confirm, false);
                break;
        }
    }

    //New game buttons
    public void SelectEnemyDifficulty(int difficulty)
    {
        ChangeButton(newgame, 1 + enemyDifficulty, true);

        enemyDifficulty = difficulty;

        ChangeButton(newgame, 1 + enemyDifficulty, false);
    }

    public void SelectPlatformDifficulty(int difficulty)
    {
        ChangeButton(newgame, 7 + platformDifficulty, true);

        platformDifficulty = difficulty;

        ChangeButton(newgame, 7 + platformDifficulty, false);
    }

    //Localization
    public override void ChangeLanguage(Language newLanguage)
    {
        //Main
        txts[0].text = LocalizationManager.GetText(menuTable, "newgame");
        txts[1].text = LocalizationManager.GetText(generalTable, "continue");
        txts[2].text = LocalizationManager.GetText(generalTable, "settings");
        txts[3].text = LocalizationManager.GetText(generalTable, "exit");

        //Confirm
        txts[4].text = LocalizationManager.GetText(generalTable, "confirm");
        txts[5].text = LocalizationManager.GetText(generalTable, "back");

        //Newgame
        txts[6].text = LocalizationManager.GetText(menuTable, "enemy_text");
        txts[7].text = LocalizationManager.GetText(menuTable, "platforming_text");

        //Settings
        txts[8].text = LocalizationManager.GetText(generalTable, "language_text");
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