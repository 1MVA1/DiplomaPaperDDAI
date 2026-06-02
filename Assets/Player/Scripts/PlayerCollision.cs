using UnityEngine;


public class PlayerCollision : MonoBehaviour
{
    [Header("Main")]
    public GameObject owner;

    public PlayerMovement pm;
    public Collider2D col;

    [Header("Do not touch")]
    public Spawn spawn;


    public void Die()
    {
        spawn.Restart();

        Destroy(owner);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.deathEnemyNow++;
            }

            Die();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlatDeath"))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.deathPlatNow++;
            }

            Die();
        }
        else if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.deathEnemyNow++;
            }

            Die();
        }
        else if (other.CompareTag("Finish"))
        {
            pm.DisableMovement();

            col.enabled = false;
        }
    }
}
