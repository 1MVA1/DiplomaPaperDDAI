using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerSpawner spawner;

    void Start() {
        spawner = FindFirstObjectByType<PlayerSpawner>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
            spawner.KillPlayer(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathTrigger"))
            spawner.KillPlayer(gameObject);
        else if (other.CompareTag("LevelFinish"))
            spawner.FinishLevel();
    }
}