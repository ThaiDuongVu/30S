using UnityEngine;
using UnityEngine.InputSystem;

public class InputTypeController : MonoBehaviour
{
    #region Singleton

    private static InputTypeController _inputTypeControllerInstance;

    public static InputTypeController Instance
    {
        get
        {
            if (_inputTypeControllerInstance == null)
                _inputTypeControllerInstance = FindObjectOfType<InputTypeController>();
            return _inputTypeControllerInstance;
        }
    }

    #endregion

    private InputType _inputType = InputType.MouseKeyboard;

    #region Unity Event

    private void Update()
    {
        if (Mouse.current.delta.ReadValue().magnitude > 0f) _inputType = InputType.MouseKeyboard;
    }

    #endregion

    public void CheckInputType(InputAction.CallbackContext context)
    {
        _inputType = context.control.device == InputSystem.devices[0] || context.control.device == InputSystem.devices[1]
            ? InputType.MouseKeyboard
            : InputType.Gamepad;
    }
}