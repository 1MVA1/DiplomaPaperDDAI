
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum MenuStates
{
    Main,
    NewGame,
    Confirm
}


public class MenuManager: UIManagerBase
{
    [Header("Prefabs")]
    public GameObject pauseManagerPrefab;
    public GameObject eventSystemPrefab;

    [Header("Main")]
    public Button[] main_bns;
    public TMP_Text[] main_txts;

    [Header("NewGame")]
    public Button[] newgame_bns;
    public TMP_Text[] newgame_txts;

    private MenuStates state = MenuStates.Main;

    private int enemyDifficulty;
    private int platformDifficulty;

    private DDA_Connection dda;

    private bool canPress = true;


    protected void Start()
    {
        //if (LevelManager.Instance.wasInMenu)
        //{
        //    Init();
        //}
        //else
        //{
        //    if (eventSystemPrefab != null)
        //    {
        //        GameObject es = GameObject.Instantiate(eventSystemPrefab);
        //        UnityEngine.Object.DontDestroyOnLoad(es);
        //    }
        //}

        dda = FindFirstObjectByType<DDA_Connection>();
    }

    public void Init() 
    {
        if (SaveManager.Instance.levelIndex == -1)
        {
            main_bns[1].interactable = false;
        }
        else
        {
            main_bns[2].onClick.RemoveAllListeners();
            main_bns[2].onClick.AddListener(TurnOnNewGameConfirm);
        }

        SetActivePanel(main_bns, main_txts, true);
    }

    //TurnOns
    public void TurnOnMain()
    {
        TurnOffPanel();

        main_txts[0].text = "ƒ»œÀŒÃÕ¿ﬂ –¿¡Œ“¿";

        state = MenuStates.Main;
        SetActivePanel(main_bns, main_txts, true);
    }

    public void TurnOnNewGame()
    {
        if (!canPress)
            return;

        TurnOffPanel();

        enemyDifficulty = 2;
        platformDifficulty = 2;

        for (int i = 0; i <= 9; i++)
        {
            if (i == 2 || i == 7)
                newgame_bns[i].interactable = false;
            else
                newgame_bns[i].interactable = true;
        }

        newgame_bns[10].onClick.RemoveAllListeners();
        newgame_bns[10].onClick.AddListener(StartGame);

        newgame_txts[0].text = "Œ÷≈Õ»“≈ —¬Œ» Õ¿¬€ ».\n≈—À» ¬€ Õ≈ ”¬≈–≈Õ€, À”◊ÿ≈ ¬€¡–¿“‹ Ã≈Õ‹ÿ”ﬁ";
        newgame_txts[1].text = "—ÀŒ∆ÕŒ—“‹ ¬–¿√Œ¬";
        newgame_txts[2].text = "—ÀŒ∆ÕŒ—“‹ œÀ¿“‘Œ–Ã»Õ√¿";
        newgame_txts[3].text = "Õ¿◊¿“‹";
        newgame_txts[4].text = "¬≈–Õ”“‹—ﬂ ¬ Ã≈Õﬁ";

        state = MenuStates.NewGame;
        SetActivePanel(newgame_bns, newgame_txts, true);
    }

    public void TurnOnExitConfirm()
    {
        if (!canPress)
            return;

        TurnOffPanel();

        confirm_bns[0].onClick.RemoveAllListeners();
        confirm_bns[0].onClick.AddListener(ExitGame);

        confirm_txts[0].text = "¬€ ƒ≈…—“¬»“≈À‹ÕŒ ’Œ“»“≈ ¬€…“» »« »√–€?";
        confirm_txts[1].text = "¬€…“» »« »√–€";
        confirm_txts[2].text = "¬≈–Õ”“‹—ﬂ ¬ »√–”";

        state = MenuStates.Confirm;
        SetActivePanel(confirm_bns, confirm_txts, true);
    }

    public void TurnOnNewGameConfirm()
    {
        if (!canPress)
            return;

        TurnOffPanel();

        confirm_txts[0].text = "¬€ “Œ◊ÕŒ ’Œ“»“≈ Õ¿◊¿“‹ ÕŒ¬”ﬁ »√–”?\n¬€ ”ƒ¿À»“≈ —“¿–Œ≈ —Œ’–¿Õ≈Õ»≈";
        confirm_txts[1].text = "œ–ŒƒŒÀ∆»“‹";
        confirm_txts[2].text = "¬≈–Õ”“‹—ﬂ ¬ Ã≈Õﬁ";

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

            case MenuStates.Confirm:
                SetActivePanel(confirm_bns, confirm_txts, false);
            break;
        }
    }


    //New game buttons
    public void SelectEnemyDifficulty(int difficulty)
    {
        newgame_bns[enemyDifficulty].interactable = true;

        enemyDifficulty = difficulty;

        newgame_bns[enemyDifficulty].interactable = false;
    }

    public void SelectPlatformDifficulty(int difficulty)
    {
        newgame_bns[platformDifficulty + 5].interactable = true;

        platformDifficulty = difficulty;

        newgame_bns[platformDifficulty + 5].interactable = false;
    }


    //Special 
    public void PressedDDA()
    {
        canPress = false;

        StartCoroutine(PressedDDACoroutine());
    }

    private IEnumerator PressedDDACoroutine()
    {
        main_txts[2].color = Color.yellow;
        main_txts[2].text = "Œ∆»ƒ¿Õ»≈";

        yield return new WaitForSeconds(0.5f);

        if (dda.isConnected)
        {
            if (dda.Disconnect())
            {
                main_txts[2].color = Color.red;
                main_txts[2].text = "Œ“ Àﬁ◊≈Õ";
            }
            else
            {
                main_txts[2].color = Color.green;
                main_txts[2].text = "œŒƒ Àﬁ◊≈Õ";
            }
        }
        else
        {
            if (dda.Connect())
            {
                main_txts[2].color = Color.green;
                main_txts[2].text = "œŒƒ Àﬁ◊≈Õ";
            }
            else
            {
                main_txts[2].color = Color.red;
                main_txts[2].text = "Œ“ Àﬁ◊≈Õ";
            }
        }

        canPress = true;
    }

    public void StartGame()
    {
        if (!canPress)
            return;

        if (LevelManager.Instance != null)
        {
            if (PauseManager.Instance == null)
                Instantiate(pauseManagerPrefab, Vector3.zero, Quaternion.identity);

            var sm = SaveManager.Instance;

            sm.enemyDiff = enemyDifficulty;
            sm.platDiff = platformDifficulty;
            sm.levelIndex++;

            LevelManager.Instance.LoadLevel(0);
        }
    }
}
