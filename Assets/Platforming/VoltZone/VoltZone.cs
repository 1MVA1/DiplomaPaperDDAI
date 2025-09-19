using UnityEngine;

public class VoltZone : MonoBehaviour
{
    public Difficulty trapDifficulty = Difficulty.Easy;

    public float interval = 3f; 
    public float activeDuration = 1f; 

    private Collider2D zoneCollider;

    void Start()
    {
        zoneCollider = GetComponent<Collider2D>();
        zoneCollider.enabled = false; 

        Invoke(nameof(ActivateZone), interval); 
    }

    void ActivateZone()
    {
        zoneCollider.enabled = true;
        Debug.Log("On!");

        Invoke(nameof(DeactivateZone), activeDuration); 
    }

    void DeactivateZone()
    {
        zoneCollider.enabled = false;
        Debug.Log("Off!");

        Invoke(nameof(ActivateZone), interval);
    }
}