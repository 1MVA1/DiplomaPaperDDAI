
using UnityEngine;


public class StalkerBody : MonoBehaviour
{
    [Header("Edit")]
    public Stalker owner;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            owner.TakeDamage();
        }
    }
}
