using UnityEngine;

public class PatrolAI : MonoBehaviour
{
    [Header("PATROL AI")]
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    public Transform[] patrolPoints;

    private Rigidbody2D body;
    private int currentPatrolIndex = 0;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ChangePatrolPoint();
    }

    void ChangePatrolPoint()
    {
        Vector2 targetPosition = patrolPoints[currentPatrolIndex].position;
        Vector2 direction = (targetPosition - body.position).normalized;

        body.velocity = direction * speed;

        if (Vector2.Distance(body.position, targetPosition) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
}
