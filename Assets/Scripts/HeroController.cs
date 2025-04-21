using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _horizontalJumpForce = 5f;
    [SerializeField] private float _verticalJumpForce = 10f;

    [Header("Time Slowdown")]
    [SerializeField] private float _slowdownFactor = 0.5f;
    [SerializeField] private float _slowdownDuration = 1f;

    [Header("References")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rigidbody2D;
    private PlayerInputSystem _input;
    private Camera _mainCamera;
    private Vector2 _cursorPosition;
    private bool _isGrounded;
    private bool _canMove;
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
        if (_isGrounded) HandleMovement();

        UpdateCursorPosition();
        RotateTowardsCursor();
        HandleShooting();
        CheckGround();

        Debug.Log(_canMove);
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
        if (!_isGrounded) return;

        float direction = _input.Player.Move.ReadValue<Vector2>().x;
        _rigidbody2D.velocity = new Vector2(direction * _moveSpeed, _rigidbody2D.velocity.y);
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _groundCheckRadius,
            _groundLayer
        );

        if (_isGrounded)
        {
            _canMove = true;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_isGrounded) return;

        _canMove = false;

        float direction = Mathf.Sign(transform.localScale.x);

        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        _rigidbody2D.AddForce(new Vector2(direction * _horizontalJumpForce, _verticalJumpForce), ForceMode2D.Impulse);

        TimeManager.Instance.ActivateSlowdown(_slowdownFactor, _slowdownDuration);
    }

    private void HandleShooting()
    {
        if (_input.Player.Fire.triggered)
        {
            _weapon?.Shoot(transform.localScale.x);
        }
    }
}