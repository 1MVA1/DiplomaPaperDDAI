using UnityEngine;
using TMPro;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject playerPrefab;
    public float respawnTime = 3f;

    [Header("UI")]
    public TMP_Text timerText;

    private GameObject currentPlayer;
    private float timer = 0f;
    private bool isTimerRunning = false;

    private List<MonoBehaviour> refreshObjects = new List<MonoBehaviour>();

    void Start()
    {
        FindAllRefreshables();

        SpawnPlayer();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            timerText.text = $"{timer:F2}";
        }
    }

    public void SpawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        RefreshAllObjects();

        currentPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        timer = 0f;
        isTimerRunning = true;
    }

    public void KillPlayer(GameObject player)
    {
        if (player != null)
        {
            Destroy(player);
        }

        isTimerRunning = false;

        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnPlayer();
    }

    public void FinishLevel()
    {
        isTimerRunning = false;
    }

    private void FindAllRefreshables()
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

        Debug.Log($"Найдено {refreshObjects.Count} объектов с методом Refresh()");
    }

    private void RefreshAllObjects()
    {
        foreach (var obj in refreshObjects)
        {
            if (obj == null) 
                continue;

            MethodInfo refreshMethod = obj.GetType().GetMethod("Refresh", BindingFlags.Instance | BindingFlags.Public);

            if (refreshMethod != null)
                refreshMethod.Invoke(obj, null);
        }
    }
}