using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class UIManagerBase : MonoBehaviour
{
    protected PlayerInputActions inputActions;

    [Header("Confirm")]
    public Button[] confirm_bns;
    public TMP_Text[] confirm_txts;


    protected void SetActivePanel(Button[] bns, TMP_Text[] txts, bool newActive)
    {
        foreach (var bn in bns)
            bn.gameObject.SetActive(newActive);

        foreach (var txt in txts)
            txt.gameObject.SetActive(newActive);
    }

    //Special
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
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
