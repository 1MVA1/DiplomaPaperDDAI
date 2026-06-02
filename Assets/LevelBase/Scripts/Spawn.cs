
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Spawn : MonoBehaviour
{
    [Header("Edit")]
    public float respawnTime = 1f;
    [Range(0, 2)]
    public int idx;
    public bool isWithHead = true;

    [Header("Main")]
    public SpriteRenderer sr;
    public SpriteRenderer srHead;

    public GameObject playerPrefab;
    public Timer timer;

    private List<MonoBehaviour> GObjs = new List<MonoBehaviour>();

    private System.Action levelReadyAction;

    [Header("Sprite")]
    public Sprite[] sprites;
    public Sprite[] spritesHead;


    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            UpdateVisuals();
        };
#endif
    }
    private void UpdateVisuals()
    {
        sr.sprite = sprites[idx];

        if (isWithHead)
        {
            srHead.sprite = spritesHead[idx];
            srHead.enabled = true;
        }
        else
        {
            srHead.enabled = false;
        }
    }

    void Awake()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelReady += OnLevelReadyLevelManage;
        }
        else
        {
            OnLevelReady(true);
        }
    }

    public void OnLevelReadyLevelManage()
    {
        LevelManager.Instance.OnLevelReady -= OnLevelReadyLevelManage;

        OnLevelReady(false);
    }

    private void OnLevelReady(bool IsDebug)
    {
        MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var obj in allBehaviours)
        {
            if (obj == null)
            {
                continue;
            }

            if (obj is IRefreshable)
            {
                GObjs.Add(obj);
            }
        }

        if (IsDebug)
        {
            foreach (var obj in GObjs)
            {
                if (obj is IRefreshable prepable)
                {
                    prepable.PrepDiff();
                }
            }
        }

        StartCoroutine(ResreshAndRespawnPlayer(false));
    }
    private IEnumerator ResreshAndRespawnPlayer(bool shouldRefresh)
    {
        yield return new WaitForSeconds(respawnTime);

        if (shouldRefresh)
        {
            foreach (var obj in GObjs)
            {
                if (obj is IRefreshable refreshable)
                {
                    refreshable.Refresh();
                }
            }
        }

        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        player.GetComponentInChildren<PlayerCollision>().spawn = this;

        timer.TurnOn();

        TurnOnAll();
    }
    public void Restart()
    {
        timer.TurnOff();

        TurnOffAll();

        StartCoroutine(ResreshAndRespawnPlayer(true));
    }

    void TurnOnAll()
    {
        foreach (var obj in GObjs)
        {
            if (obj is IRefreshable turnonable)
            {
                turnonable.TurnOn();
            }
        }
    }
    public void TurnOffAll()
    {
        foreach (var obj in GObjs)
        {
            if (obj is IRefreshable turnoffable)
            {
                turnoffable.TurnOff();
            }
        }
    }
}
