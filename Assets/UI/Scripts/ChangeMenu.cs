
using UnityEngine;
using UnityEngine.UI;


public class ChangeMenu : MonoBehaviour
{
    public Button[] bnsEnemy;
    public Button[] bnsPlat;

    private int actPlat = 1;
    private int actEnemy = 1;

    public void SelectEnemyAct(int act)
    {
        bnsEnemy[actEnemy].interactable = true;

        actEnemy = act;

        bnsEnemy[actEnemy].interactable = false;
    }

    public void SelectPlatAct(int act)
    {
        bnsPlat[actPlat].interactable = true;

        actPlat = act;

        bnsPlat[actPlat].interactable = false;
    }

    public void Continue()
    {
        SaveManager.Instance.SetNewDiffs(actPlat - 1, actEnemy - 1);

        LevelManager.Instance.LoadNextLevel();
    }
}
