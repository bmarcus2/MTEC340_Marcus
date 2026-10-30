using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + Vector2.up * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        PowerUp powerUp = other.GetComponent<PowerUp>();

        if (powerUp == null)
            powerUp = other.GetComponentInParent<PowerUp>();

        if (powerUp != null)
        {
            PlayerController player =
                FindFirstObjectByType<PlayerController>();

            if (player != null && powerUp.TryCollect(player))
            {
                Destroy(gameObject);
                return;
            }
        }

        
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy == null)
            enemy = other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}