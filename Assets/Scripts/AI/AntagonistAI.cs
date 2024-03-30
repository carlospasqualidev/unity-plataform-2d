using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class AntagonistAI : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private LayerMask enemyLayer;

    private Transform target;
    private Rigidbody2D body;
    private Animator anim;
    private CapsuleCollider2D col;
    private SpriteRenderer sprite;
    private bool isGrounded;
    private List<Vector2> movePositions;
    private ParticleSystem dustParticles;
    private readonly int isGroundedHash = Animator.StringToHash("is_grounded");
    private float stepTime;
    private bool isJumping;

    [Header("SOUNDS")]
    [SerializeField]
    private float stepTimeInterval = 0.25f; //Intervalo de tempo entre os passos

    [SerializeField]
    private AudioClip[] stepSounds; //Sons de passos

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        movePositions = new List<Vector2>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        dustParticles = GetComponentInChildren<ParticleSystem>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePositions();
        Move();
        HandleAnimations();
        CheckCollisions();
        DustParticles();

        PlayStepSound();
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

        if (!groundHit)
        {
            if (isJumping)
                return;

            SFXController.instance.PlayJumpSound(0.25f);
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }
    }

    void DustParticles()
    {
        if (isGrounded)
            dustParticles.Play();
        else
            dustParticles.Stop();
    }

    private void PlayStepSound()
    {
        stepTime += Time.deltaTime;

        if (stepTime >= stepTimeInterval && isGrounded && body.velocity.x != 0)
        {
            SFXController.instance.PlaySound(stepSounds, 0.25f);
            stepTime = 0f; // Resetar o tempo
        }
    }
}
