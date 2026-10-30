using System.Collections;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] protected float duration = 5f;

    private bool collected;

    protected abstract void Apply(PlayerController player);
    protected abstract void Remove(PlayerController player);

    public bool TryCollect(PlayerController player)
    {
        if (collected || player == null)
            return false;

        collected = true;

        Collider2D powerUpCollider = GetComponent<Collider2D>();
        SpriteRenderer powerUpRenderer =
            GetComponent<SpriteRenderer>();

        if (powerUpCollider != null)
            powerUpCollider.enabled = false;

        if (powerUpRenderer != null)
            powerUpRenderer.enabled = false;

        StartCoroutine(PowerUpRoutine(player));

        return true;
    }

    private IEnumerator PowerUpRoutine(PlayerController player)
    {
        Apply(player);

        if (GameManager.Instance != null)
            GameManager.Instance.PlayPowerUpSfx();

        yield return new WaitForSeconds(duration);

        if (player != null)
            Remove(player);

        Destroy(gameObject);
    }
}