using UnityEngine;
using TMPro;
using System.Collections;

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

    void Start() {
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

    public void SpawnPlayer() {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        currentPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        timer = 0f;
        isTimerRunning = true;
    }

    public void KillPlayer(GameObject player)
    {
        if (player != null) {
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
}