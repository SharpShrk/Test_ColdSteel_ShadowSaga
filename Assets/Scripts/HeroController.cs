using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _horizontalJumpForce = 5f;
    [SerializeField] private float _verticalJumpForce = 10f;
    [SerializeField] private float _movementAcceleration = 3f;

    [Header("References")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private TimeManager _timeManager;
    [SerializeField] private Animator _animator;

    private float _currentDirection;
    private Rigidbody2D _rigidbody2D;
    private PlayerInputSystem _input;
    private Camera _mainCamera;
    private Vector2 _cursorPosition;
    private bool _isGrounded;
    private float _groundCheckRadius = 0.05f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _input = new PlayerInputSystem();
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.Jump.performed -= OnJump;
    }

    private void Update()
    {
        if (_isGrounded)
            HandleMovement();

        UpdateCursorPosition();
        RotateTowardsCursor();
        HandleShooting();
        CheckGround();
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isMoving = Mathf.Abs(_currentDirection) > Mathf.Epsilon;
        _animator.SetBool("IsMove", isMoving);
    }

    private void UpdateCursorPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        _cursorPosition = _mainCamera.ScreenToWorldPoint(mousePos);
    }

    private void RotateTowardsCursor()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(_cursorPosition.x - transform.position.x);
        transform.localScale = scale;
    }

    private void HandleMovement()
    {
        float direction = _input.Player.Move.ReadValue<Vector2>().x;
        _currentDirection = direction;

        Vector2 targetVelocity = new Vector2(direction * _moveSpeed, _rigidbody2D.velocity.y);
        Vector2 velocityChange = (targetVelocity - _rigidbody2D.velocity);

        _rigidbody2D.AddForce(velocityChange * _movementAcceleration);
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_isGrounded) return;

        float direction = Mathf.Sign(transform.localScale.x);
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        _rigidbody2D.AddForce(new Vector2(direction * _horizontalJumpForce, _verticalJumpForce), ForceMode2D.Impulse);
        _timeManager.ActivateSlowdown();
    }

    private void HandleShooting()
    {
        if (_input.Player.Fire.triggered)
        {
            _weapon?.Shoot(transform.localScale.x);
        }
    }
}