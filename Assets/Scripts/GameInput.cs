using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; } //
    public event EventHandler OnMenuButtonPressed;
    private Input_Actions inputActions;
    private void Awake()
    {
        Instance = this;
        inputActions = new Input_Actions();
        inputActions.Enable();

        inputActions.Player.Menu.performed += Menu_performed;
    }

    private void Menu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMenuButtonPressed?.Invoke(this, EventArgs.Empty);
    }
    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public bool IsUpActionPressed()
    {
        return inputActions.Player.LanderUp.IsPressed();
    }

    public bool IsLeftActionPressed()
    {
        return inputActions.Player.LanderLeft.IsPressed();
    }

    public bool IsRightActionPressed()
    {
        return inputActions.Player.LanderRight.IsPressed();
    }

    public Vector2 GetMovementInputValueVector2()
    {
        return inputActions.Player.Movement.ReadValue<Vector2>();
    }
}
