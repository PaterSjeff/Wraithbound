using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        Instance = this;

        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable(); // Enables the "Player" action map
    }

    public Vector2 GetMouseScreenPosition()
    {
        // This handles both Mouse and Touch (if Touch is mapped to Point)
        return Mouse.current.position.ReadValue(); 
    }

    public bool IsMouseButtonDown()
    {
        // For now, we poll the left click. 
        // In the polished version, we will use events (inputActions.Player.Click.performed).
        return Mouse.current.leftButton.wasPressedThisFrame;
    }
}