
using UnityEngine;


public class CrabBody : MonoBehaviour
{
    [Header("Edit")]
    public Crab owner;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            owner.TakeDamage();
        }
    }
}
