using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Brick Healt")]
    [SerializeField] private int _health = 3;

    [Header("Brick Color")]
    [SerializeField] private Color[] _healthColors =
    {
        Color.green,
        Color.yellow,
        Color.red
    };

    private SpriteRenderer _spriteRenderer;

    public int Health
    {
        get => _health;

        set
        {
            _health = value;
            UpdateColor();
        }
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        
        Health = _health;
    }

    public bool TakeDamage()
    {
        Health--;

        if (Health <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    private void UpdateColor()
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        if (_health > 0 && _health <= _healthColors.Length)
        {
            _spriteRenderer.color = _healthColors[_health - 1];
        }
    }
}

