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

    public Difficulty difficulty = Difficulty.Easy;

    public SawDiff diffEasy = new SawDiff(2f, 0f, 0f);
    public SawDiff diffMedium = new SawDiff(3f, 0f, 0f);
    public SawDiff diffHard = new SawDiff(4f, 0.5f, 6f);

    private SawDiff currentDiff;

    public Waypoint[] waypoints;
    public float rotationSpeed = 360f;

    private float currentSpeed;

    private int currentIndex = 0;
    private bool isGoingForward = true;
    private bool isWaiting = false;

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

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting) {
            return;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, currentDiff.speed, currentDiff.acceleration * Time.deltaTime);

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].point, currentSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[currentIndex].point) < 0.05f)
        {
            StartCoroutine(WaitAtPoint(waypoints[currentIndex].stopTime));

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
}