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

    public Difficulty trapDifficulty = Difficulty.Easy;

    public Waypoint[] waypoints;
    public float speed = 2f;
    public float rotationSpeed = 360f;

    private int currentIndex = 0;
    private bool isGoingForward = true;
    private bool isWaiting = false;

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting) {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].point, speed * Time.deltaTime);
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