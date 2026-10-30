
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float bottomLimit = -4.5f;

    [Header("Zigzag Movement")]
    [SerializeField] private float zigzagAmplitude = 1.2f;
    [SerializeField] private float zigzagFrequency = 3f;

    [Header("Health and Score")]
    [SerializeField] private int health = 1;
    [SerializeField] private int scoreValue = 100;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isDead;
    private bool isZigzag;
    private float zigzagOriginX;
    private float zigzagStartTime;

    public int Health
    {
        get => health;
        private set => health = Mathf.Max(0, value);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        GetComponent<Collider2D>().isTrigger = true;

        zigzagOriginX = rb.position.x;
        zigzagStartTime = Time.time;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetZigzag(Sprite zigzagSprite)
    {
        isZigzag = true;
        zigzagOriginX = rb.position.x;
        zigzagStartTime = Time.time;

        if (zigzagSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = zigzagSprite;
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (GameManager.Instance != null &&
            !GameManager.Instance.IsPlaying)
            return;

        Vector2 nextPosition = rb.position;

        nextPosition.y -= moveSpeed * Time.fixedDeltaTime;

        if (isZigzag)
        {
            float elapsed = Time.time - zigzagStartTime;

            nextPosition.x = zigzagOriginX +
                Mathf.Sin(elapsed * zigzagFrequency) *
                zigzagAmplitude;
        }

        rb.MovePosition(nextPosition);

        if (nextPosition.y <= bottomLimit)
        {
            ReachBottom();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        Health -= damage;
        if (GameManager.Instance != null)
        {
    GameManager.Instance.PlayEnemyDeathSfx();
        }
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        WaveManager waveManager =
            FindFirstObjectByType<WaveManager>();

        if (waveManager != null)
        {
            waveManager.TryDropPowerUp(transform.position);
        }

        Destroy(gameObject);
    }

    private void ReachBottom()
    {
        if (isDead)
            return;

        isDead = true;

        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        if (player != null && player.ShieldActive)
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReachBottom();
        }
    }
}
