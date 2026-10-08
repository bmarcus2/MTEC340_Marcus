using Unity.Collections.Tests.CoreCLR.TestJobs;
using Unity.VisualScripting;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    [Header("Ball Properties")]
    [SerializeField] private float _launchForce = 7.0f;
    [SerializeField] private float _speedIncrement = 1.1f;
    [SerializeField] private float _steepnessThreshhold = 0.5f;
    [SerializeField] private float _paddleInfluence = 0.4f;

    private Rigidbody2D _rb;

    private AudioSource _source;
    [SerializeField] private AudioClip _wallHit;
    [SerializeField] private AudioClip _paddleHit;
    [SerializeField] private AudioClip _scorePoint;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _source = GetComponent<AudioSource>();

        ResetBall();
    }

    private void Update()
    {
        
        _rb.simulated = GameBehavior.Instance.State == Utilities.GameState.Play;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();

            if (brick != null)
            {
               
                bool brickDestroyed = brick.TakeDamage();

                
                if (brickDestroyed)
                {
                    Debug.Log("Brick broken!");

                    GameBehavior.Instance.ScorePoint();

                    if (_source != null && _scorePoint != null)
                    {
                        _source.PlayOneShot(_scorePoint);
                    }
                }
            }
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            if (!Mathf.Approximately(collision.rigidbody.linearVelocityY, 0.0f))
            {
                Debug.Log("Collision with paddle!!!");

                Vector2 direction = _rb.linearVelocity * (1.0f - _paddleInfluence)
                                    + collision.rigidbody.linearVelocity * _paddleInfluence;

                _rb.linearVelocity =
                    _rb.linearVelocity.magnitude *
                    direction.normalized *
                    _speedIncrement;
            }

            if (_source != null && _paddleHit != null)
            {
                _source.PlayOneShot(_paddleHit);
            }
        }
        else
        {
            if (_source != null && _wallHit != null)
            {
                _source.pitch = Random.Range(0.9f, 1.1f);
                _source.volume = Random.Range(0.8f, 1.0f);

                _source.clip = _wallHit;
                _source.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ResetBall();
    }

    private void ResetBall()
    {
        _rb.linearVelocity = Vector2.zero;

        transform.position = Vector3.zero;

        Vector2 direction = Random.insideUnitCircle.normalized;

        CheckSteepness(ref direction);

        _rb.AddForce(direction * _launchForce, ForceMode2D.Impulse);
    }

    private void CheckSteepness(ref Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < _steepnessThreshhold)
        {
            direction.x += 0.5f * Mathf.Sign(direction.x);
            direction.Normalize();
        }
    }
}