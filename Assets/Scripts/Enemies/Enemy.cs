using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("ENEMY")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private float knobackForce = 20f;

    [SerializeField]
    private bool invertFlip = false;
    private float timeForNextDamage = 0.5f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private ParticleSystem dustParticles;

    private bool FlipSprite => spriteRenderer.flipX = body.velocity.x < 0;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        dustParticles = GetComponentInChildren<ParticleSystem>();

        dustParticles.Play();
    }

    void Update()
    {
        timeForNextDamage += Time.deltaTime;
        spriteRenderer.flipX = invertFlip ? FlipSprite : !FlipSprite;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeForNextDamage < 0.5f)
        {
            timeForNextDamage = 0;
            return;
        }

        if (!collision.CompareTag("Player"))
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        bool hitFromLeft = collision.transform.position.x < boxCollider.bounds.min.x;
        bool hitFromRight = collision.transform.position.x > boxCollider.bounds.max.x;

        if (hitFromLeft || hitFromRight)
        {
            player.Knockback(
                hitFromLeft
                    ? PlayerController.KnockbackType.Left
                    : PlayerController.KnockbackType.Right,
                knobackForce
            );
            player.TakeDamage();
            return;
        }

        player.Knockback(PlayerController.KnockbackType.Up);
        Die();
    }

    void Die()
    {
        SFXController.instance.PlayHitSound();
        GameObject effect = Instantiate(
            GameManager.instance.enemyDieEffect,
            transform.position,
            Quaternion.identity
        );
        Destroy(gameObject);
        Destroy(effect, 0.35f);
    }
}
