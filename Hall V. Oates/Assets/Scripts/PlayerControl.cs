
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float horizontalLimit = 8f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 0.3f;

    [Header("Player Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite powerUpSprite;
    [SerializeField] private Sprite powerDownSprite;

    private float nextFireTime;
    private float fireRateMultiplier = 1f;
    private bool shieldActive;
    private bool rapidFireActive;
    private bool powerDownActive;

    private SpriteRenderer spriteRenderer;

    public bool ShieldActive
    {
        get => shieldActive;
        set
        {
            shieldActive = value;
            UpdatePlayerSprite();
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null && normalSprite == null)
        {
            normalSprite = spriteRenderer.sprite;
        }

        UpdatePlayerSprite();
    }

    public void SetRapidFire(bool enabled)
    {
        rapidFireActive = enabled;
        fireRateMultiplier = enabled ? 2.5f : 1f;
        UpdatePlayerSprite();
    }

    public void SetPowerDown(bool enabled)
    {
        powerDownActive = enabled;
        UpdatePlayerSprite();
    }

    private void UpdatePlayerSprite()
    {
        if (spriteRenderer == null)
            return;

        // The power-down sprite takes priority if effects overlap.
        if (powerDownActive && powerDownSprite != null)
        {
            spriteRenderer.sprite = powerDownSprite;
        }
        else if ((rapidFireActive || shieldActive) &&
                 powerUpSprite != null)
        {
            spriteRenderer.sprite = powerUpSprite;
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public void ResetPowerEffects()
    {
        powerDownActive = false;
        shieldActive = false;
        rapidFireActive = false;
        fireRateMultiplier = 1f;
        UpdatePlayerSprite();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        Move();

        if (Input.GetKey(KeyCode.Space) ||
            Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    private void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");

        float currentSpeed = powerDownActive
            ? moveSpeed * 0.5f
            : moveSpeed;

        Vector3 position = transform.position;

        position.x += input * currentSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(
            position.x, -horizontalLimit, horizontalLimit);

        transform.position = position;
    }

    private void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (projectilePrefab == null || firePoint == null)
            return;

        nextFireTime = Time.time +
            fireInterval / fireRateMultiplier;

        Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity);

        GameManager.Instance.PlayShootSfx();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        if (shieldActive)
        {
            Destroy(other.gameObject);
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
