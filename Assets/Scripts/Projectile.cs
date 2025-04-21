using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _lifetime = 2f;

    private Rigidbody2D _rigidbody2D;
    private float _speed;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float speed)
    {
        _speed = speed;
        _rigidbody2D.velocity = transform.right * _speed;
        Destroy(gameObject, _lifetime);
    }
}