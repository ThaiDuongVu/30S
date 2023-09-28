using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce;

    private bool _isMoving;
    private Vector2 _currentDirection;

    private Player _player;

    private Rigidbody2D _rigidbody;

    private InputManager _inputManager;

    #region Unity Events

    private void OnEnable()
    {
        _inputManager = new InputManager();

        // Handle player movement input
        _inputManager.Player.Move.performed += MoveOnPerformed;
        _inputManager.Player.Move.canceled += MoveOnCanceled;

        _inputManager.Enable();
    }

    private void OnDisable()
    {
        _inputManager.Disable();
    }

    private void Awake()
    {
        _player = GetComponent<Player>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_isMoving) _rigidbody.AddForce(moveForce * _currentDirection, ForceMode2D.Force);
    }

    #endregion

    #region Input Handlers

    private void MoveOnPerformed(InputAction.CallbackContext context)
    {
        InputTypeController.Instance.CheckInputType(context);
        
        if (GameController.Instance.State != GameState.InProgress) return;
        Move(context.ReadValue<Vector2>().normalized);
    }

    private void MoveOnCanceled(InputAction.CallbackContext context)
    {
        Stop();
    }

    #endregion

    #region Movement Methods

    private void Move(Vector2 direction)
    {
        _isMoving = true;
        _currentDirection = direction;

        if (direction.x < 0f) _player.SetFlipped(true);
        else if (direction.x > 0f) _player.SetFlipped(false);
    }

    private void Stop()
    {
        _isMoving = false;
    }

    #endregion
}
