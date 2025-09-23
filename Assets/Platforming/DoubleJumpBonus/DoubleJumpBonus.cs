using Unity.VisualScripting;
using UnityEngine;

public class DoubleJumpBonus : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        Destroy(gameObject);
    }
}
