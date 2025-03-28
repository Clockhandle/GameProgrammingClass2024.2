using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System;

public class InputManager : MonoBehaviour
{
    private PlayerInput inputActions;
    private bool isHolding;
    public static Vector3 MouseWorldPosition { get; private set; } // Stores world position

    public static event Action OnTap;
    public static event Action OnHold;
    public static event Action OnRelease;

    private void Awake()
    {
        inputActions = new PlayerInput();

        // Subscribe to input events
        inputActions.Mouse.Click.performed += ctx => OnMouseClick(ctx);
        inputActions.Mouse.Hold.performed += ctx => OnMouseHold(ctx);
        inputActions.Mouse.Release.performed += _ => OnMouseRelease();
    }

    private void OnEnable() => inputActions.Enable();

    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        MouseWorldPosition = GetMouseWorldPosition();
        if (isHolding)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log($"Mouse Hold at screen position: {mousePosition}");
            Debug.Log($"Mouse Hold at world position: {MouseWorldPosition}");
        }
    }

    private void OnMouseClick(InputAction.CallbackContext ctx)
    {
        // Check if the click was a tap (quick press and release)
        if (ctx.interaction is TapInteraction)
        {
            Debug.Log("Mouse Tap detected!");
            OnTap?.Invoke();
        }
    }

    private void OnMouseHold(InputAction.CallbackContext ctx)
    {
        Debug.Log("Mouse Hold started!");
        isHolding = true;
        OnHold?.Invoke();
    }

    private void OnMouseRelease()
    {
        Debug.Log("Mouse Released!");
        isHolding = false;
        OnRelease?.Invoke();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        float depth = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
    }
}
