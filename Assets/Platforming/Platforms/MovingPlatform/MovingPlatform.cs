using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector3 point; 
        public float stopTime = 1f;
    }

    public Difficulty difficulty = Difficulty.Easy;

    public float speedEasy = 2f;
    public float speedMedium = 3f;
    public float speedHard = 4f;

    public Waypoint[] waypoints;

    private float speed;

    private int currentIndex = 0; 
    private bool isGoingForward = true; 
    private bool isWaiting = false;

    private Vector3 lastPosition;

    private void Awake()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                speed = speedEasy;
                break;
            case Difficulty.Medium:
                speed = speedMedium;
                break;
            case Difficulty.Hard:
                speed = speedHard;
                break;
        }
    }

    void Start() {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting) {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].point, speed * Time.deltaTime);

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

    void LateUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) 
            {
                PlayerMovement player = hit.GetComponent<PlayerMovement>();

                if (player != null && player.isGrounded) {
                    hit.transform.position += transform.position - lastPosition;
                }
            }
        }

        lastPosition = transform.position;
    }
}