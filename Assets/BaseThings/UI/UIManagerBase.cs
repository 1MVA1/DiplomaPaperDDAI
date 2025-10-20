using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManagerBase : MonoBehaviour
{
    protected PlayerInputActions inputActions;

    [Header("Base panels")]
    public GameObject[] settings;
    public GameObject[] confirm;

    [Header("Base localization")]
    public LocalizationTable generalTable;

    public TMP_Text[] txts;

    //General
    protected virtual void Start()
    {
        switch (LocalizationManager.language)
        {
            case Language.En:
                ChangeButton(settings, 1, false);
                break;

            case Language.Ru:
                ChangeButton(settings, 2, false);
                break;
        }

        ChangeLanguage(LocalizationManager.language);
    }

    protected void ChangeButton(GameObject[] gobjs, int index, bool newState)
    {
        var bn = gobjs[index].GetComponent<Button>();

        if (bn != null)
            bn.interactable = newState;
    }

    protected void SetActivePanel(GameObject[] objs, bool newActive)
    {
        foreach (var obj in objs)
        {
            obj.SetActive(newActive);
        }
    }

    //Localization
    public virtual void ChangeLanguage(Language newLanguage) { }

    public void OnEnglishPressed()
    {
        ChangeButton(settings, 1, false);
        ChangeButton(settings, 2, true);

        LocalizationManager.language = Language.En;

        ChangeLanguage(LocalizationManager.language);
    }

    public void OnRussianPressed()
    {
        ChangeButton(settings, 1, true);
        ChangeButton(settings, 2, false);

        LocalizationManager.language = Language.Ru;

        ChangeLanguage(LocalizationManager.language);
    }

    //Special
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(); // закрыть игру в билде
#endif
    }

    public virtual void OnEscapePressed() { }

    protected void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.Pause.performed += ctx => OnEscapePressed();
        }

        inputActions.Player.Enable();
    }    
    protected void OnDisable() => inputActions.Player.Disable();
}
