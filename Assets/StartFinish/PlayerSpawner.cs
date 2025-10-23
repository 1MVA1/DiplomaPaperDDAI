using UnityEngine;
using TMPro;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject playerPrefab;

    public float respawnTime = 2f;
    public float levelDelay = 2f;

    [Header("UI")]
    public TMP_Text timerText;

    private GameObject currentPlayer;

    private float timer = 0f;
    private bool isTimerRunning = false;

    private List<MonoBehaviour> refreshObjects = new List<MonoBehaviour>();

    void Awake() {
        LevelManager.Instance.OnLevelReady += OnLevelReady;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            timerText.text = $"{timer:F2}";
        }
    }

    private void OnLevelReady()
    {
        refreshObjects.Clear();

        MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour == null)
                continue;

            MethodInfo refreshMethod = behaviour.GetType().GetMethod("Refresh", BindingFlags.Instance | BindingFlags.Public);

            if (refreshMethod != null)
                refreshObjects.Add(behaviour);
        }

        Debug.Log($"Found {refreshObjects.Count} objs with Refresh()");

        StartCoroutine(Respawn(true));
    }

    public void KillPlayer(GameObject player)
    {
        if (player != null)
        {
            Destroy(player);
        }

        isTimerRunning = false;

        StartCoroutine(Respawn(false));
    }

    private IEnumerator Respawn(bool isFirst)
    {
        yield return new WaitForSeconds(respawnTime);

        if (!isFirst)
        {
            foreach (var obj in refreshObjects)
            {
                if (obj == null)
                    continue;

                var refreshMethod = obj.GetType().GetMethod("Refresh", BindingFlags.Instance | BindingFlags.Public);

                if (refreshMethod != null)
                    refreshMethod.Invoke(obj, null);
            }
        }

        currentPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        timer = 0f;
        isTimerRunning = true;
    }

    public void FinishLevel()
    {
        isTimerRunning = false;

        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(levelDelay);

        LevelManager.Instance.LoadNextLevel();
    }
}