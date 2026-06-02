
using UnityEngine;


public class ShooterBody : MonoBehaviour
{
    [Header("Edit")]
    public Shooter owner;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            owner.TakeDamage();
        }
    }
}
