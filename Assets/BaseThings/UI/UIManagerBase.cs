using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManagerBase : MonoBehaviour
{
    protected PlayerInputActions inputActions;

    [Header("Base buttons")]
    public Button[] settings_bns;
    public Button[] confirm_bns;

    [Header("Base texts")]
    public TMP_Text[] settings_txts;
    public TMP_Text[] confirm_txts;

    [Header("Base button texts")]
    public TMP_Text[] settings_bn_txts;
    public TMP_Text[] confirm_bn_txts;

    [Header("Base localization")]
    public LocalizationTable generalTable;

    //General
    protected virtual void Start()
    {
        switch (LocalizationManager.language)
        {
            case Language.En:
                settings_bns[0].interactable = false;
                break;

            case Language.Ru:
                settings_bns[1].interactable = false;
                break;
        }

        ChangeLanguage(LocalizationManager.language);
    }

    protected void SetActivePanel(Button[] bns, TMP_Text[] txts, bool newActive)
    {
        foreach (var bn in bns)
            bn.gameObject.SetActive(newActive);

        foreach (var txt in txts)
            txt.gameObject.SetActive(newActive);
    }

    //Localization
    public virtual void ChangeLanguage(Language newLanguage) 
    {
        //Settings
        settings_txts[0].text = LocalizationManager.GetText(generalTable, "language_text");
    }

    public void OnEnglishPressed()
    {
        settings_bns[0].interactable = false;
        settings_bns[1].interactable = true;

        LocalizationManager.language = Language.En;

        ChangeLanguage(LocalizationManager.language);
    }

    public void OnRussianPressed()
    {
        settings_bns[0].interactable = true;
        settings_bns[1].interactable = false;

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
