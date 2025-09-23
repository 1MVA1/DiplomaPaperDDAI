using UnityEngine;

public class VoltZone : MonoBehaviour
{
    private Collider2D zoneCollider;

    [System.Serializable]
    public class VoltZoneDiff
    {
        public float interval;
        public float prepareTime;
        public float activeDuration;

        public VoltZoneDiff(float interval, float prepareTime, float activeDuration)
        {
            this.interval = interval;
            this.prepareTime = prepareTime;
            this.activeDuration = activeDuration;
        }
    }

    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Easy;

    public VoltZoneDiff diffEasy = new VoltZoneDiff(3f, 1.25f, 1.5f);
    public VoltZoneDiff diffMedium = new VoltZoneDiff(2f, 1f, 2f);
    public VoltZoneDiff diffHard = new VoltZoneDiff(1.5f, 0.75f, 2.5f);

    private VoltZoneDiff currentDiff;

    private void Awake()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                currentDiff = diffEasy;
                break;
            case Difficulty.Medium:
                currentDiff = diffMedium;
                break;
            case Difficulty.Hard:
                currentDiff = diffHard;
                break;
        }
    }

    void Start()
    {
        zoneCollider = GetComponent<Collider2D>();
        zoneCollider.enabled = false; 

        Invoke(nameof(ActivateZone), currentDiff.interval); 
    }

    void ActivateZone()
    {
        zoneCollider.enabled = true;
        Debug.Log("On!");

        Invoke(nameof(DeactivateZone), currentDiff.activeDuration); 
    }

    void DeactivateZone()
    {
        zoneCollider.enabled = false;
        Debug.Log("Off!");

        Invoke(nameof(ActivateZone), currentDiff.interval);
    }
}