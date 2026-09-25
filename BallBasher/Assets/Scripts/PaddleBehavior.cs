using UnityEngine;

public class PaddleBehavior : MonoBehaviour
{
    private float _direction = 0.0f;
    
    [SerializeField] private float _speed = 5.0f;

    [SerializeField] private KeyCode _rightDirection = KeyCode.RightArrow;
    [SerializeField] private KeyCode _leftDirection = KeyCode.LeftArrow;
    
    private Rigidbody2D _rb;

    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocityY = _direction * _speed;
    }

    void Update()
    {
        float movement = 0.0f;
        
        _direction = 0.0f;

        if (Input.GetKey(_rightDirection))
        {
            movement += _speed;
            _direction += 1.0f;
        }

        if (Input.GetKey(_leftDirection))
        {
            movement -= _speed;
            _direction -= 1.0f;
        }
        
        movement *= Time.deltaTime;
        
        transform.Translate(0.0f, movement, 0.0f);

        
        
    }
}