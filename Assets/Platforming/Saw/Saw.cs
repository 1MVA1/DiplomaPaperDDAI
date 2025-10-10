using UnityEngine;
using System.Collections;

public class Saw : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector3 point;
        public float stopTime = 1f;
    }

    [System.Serializable]
    public class SawDiff
    {
        public float speed;
        public float acceleration;
        public float maxSpeed;

        public SawDiff(float speed, float acceleration, float maxSpeed)
        {
            this.speed = speed;
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
        }
    }

    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Easy;

    public SawDiff diffEasy = new SawDiff(2f, 0f, 0f);
    public SawDiff diffMedium = new SawDiff(3f, 0f, 0f);
    public SawDiff diffHard = new SawDiff(4f, 0.5f, 6f);

    private SawDiff currentDiff;

    [Header("Saw logic")]
    public Waypoint[] waypoints;
    public float rotationSpeed = 360f;

    public int startIndex = 0;
    public bool baseIsGoingForward = true;

    private float currentSpeed;

    private int currentIndex;
    private bool isGoingForward;
    private bool isWaiting = false;

    private Vector3 startPosition;
    private Coroutine coroutineWait;

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

        currentSpeed = currentDiff.speed;
    }

    void Start()
    {
        startPosition = transform.position;

        currentIndex = startIndex;
        isGoingForward = baseIsGoingForward;
    }

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting)
            return;

        currentSpeed = Mathf.MoveTowards(currentSpeed, currentDiff.speed, currentDiff.acceleration * Time.deltaTime);

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].point, currentSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[currentIndex].point) < 0.05f)
        {
            coroutineWait = StartCoroutine(WaitAtPoint(waypoints[currentIndex].stopTime));

            if (isGoingForward)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = waypoints.Length - 2;
                    isGoingForward = false;
                }
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 1;
                    isGoingForward = true;
                }
            }
        }
    }

    private IEnumerator WaitAtPoint(float duration)
    {
        isWaiting = true;

        yield return new WaitForSeconds(duration);

        isWaiting = false;
    }

    public void Refresh()
    {
        if (coroutineWait != null)
        {
            StopCoroutine(coroutineWait);
            coroutineWait = null;
        }

        isWaiting = false;

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        currentIndex = startIndex;
        isGoingForward = baseIsGoingForward;

        currentSpeed = currentDiff.speed;
    }
}