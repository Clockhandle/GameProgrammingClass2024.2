using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private PlayerInput inputActions;
    private bool isHolding;
    public static Vector3 MouseWorldPosition { get; private set; } // Stores world position
    public static bool IsPointerOverUI { get; private set; }
    public static bool IsSpecificUIDragging { get; private set; }

    public static event Action OnTap;
    public static event Action OnHold;
    public static event Action OnRelease;

    private void Awake()
    {
        inputActions = new PlayerInput();

        // Subscribe to input events
        inputActions.Mouse.Click.performed += _ => OnMouseClick();
        inputActions.Mouse.Hold.performed += _ => OnMouseHold();
        inputActions.Mouse.Release.performed += _ => OnMouseRelease();
    }

    private void OnEnable() => inputActions.Enable();

    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        IsPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        MouseWorldPosition = GetMouseWorldPosition();
        if (isHolding)
        {
            //Vector2 mousePosition = Mouse.current.position.ReadValue();
            //Debug.Log($"Mouse Hold at screen position: {mousePosition}");
            //Debug.Log($"Mouse Hold at world position: {MouseWorldPosition}");
        }
    }

    private void OnMouseClick()
    {
        if (IsPointerOverUI) return;
        Debug.Log("OnTap");
        OnTap?.Invoke();
    }

    private void OnMouseHold()
    {
        if (IsPointerOverUI) return;
        isHolding = true;
        OnHold?.Invoke();
    }

    private void OnMouseRelease()
    {
        if (IsPointerOverUI) return;
        isHolding = false;
        OnRelease?.Invoke();
    }

    public static void SignalUIDragActive()
    {
        IsSpecificUIDragging = true;
        // Debug.Log("InputManager: UI Drag Started"); // Optional debug
    }

    public static void SignalUIDragInactive()
    {
        IsSpecificUIDragging = false;
        // Debug.Log("InputManager: UI Drag Ended"); // Optional debug
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Your implementation (ideally the robust one)
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        // Example using Plane Raycast (adjust plane definition as needed)
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (groundPlane.Raycast(ray, out float enterDistance))
        {
            return ray.GetPoint(enterDistance);
        }
        return Vector3.zero; // Fallback
    }
}
