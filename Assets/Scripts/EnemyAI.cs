using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float speed;

    [SerializeField]
    private LayerMask enemyLayer;

    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D col;
    private SpriteRenderer sprite;
    private bool isGrounded;
    private float time;

    private List<Vector2> movePositions;
    private bool cachedQueryStartInColliders;
    private ParticleSystem dustParticles;

    private int velocityXHash = Animator.StringToHash("velocity_x");
    private int isGroundedHash = Animator.StringToHash("is_grounded");

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        movePositions = new List<Vector2>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        dustParticles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePositions();
        Move();
        HandleAnimations();
        CheckCollisions();
        DustParticles();
    }

    void HandlePositions()
    {
        Vector2 targetPosition = target.position;

        bool listIsEmpty = movePositions.Count == 0;

        if (listIsEmpty)
            movePositions.Add(targetPosition);

        if (!listIsEmpty && movePositions[movePositions.Count - 1] != targetPosition) // if the last position is different from the target position
            movePositions.Add(targetPosition);

        if (Vector2.Distance(body.position, movePositions[0]) < 0.5f) // if the distance between the body and the first position is less than 3
            movePositions.RemoveAt(0);
    }

    void Move()
    {
        if (movePositions.Count == 0)
            return;

        Vector2 newPos = (movePositions[0] - body.position).normalized;

        float verticalSpeedMultiplier = 1f;
        if (!isGrounded)
            verticalSpeedMultiplier = 2.4f;

        body.velocity = new Vector2(newPos.x * speed, newPos.y * speed * verticalSpeedMultiplier);
    }

    void HandleAnimations()
    {
        anim.SetBool(isGroundedHash, isGrounded);

        sprite.flipX = body.velocity.x < 0;
    }

    void CheckCollisions()
    {
        bool groundHit = Physics2D.CapsuleCast(
            col.bounds.center,
            col.size,
            col.direction,
            0,
            Vector2.down,
            0.10f,
            ~enemyLayer
        );

        isGrounded = groundHit;
    }

    void DustParticles()
    {
        if (isGrounded)
            dustParticles.Play();
        else
            dustParticles.Stop();
    }
}
