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
            Vector2 mousePosition = Mouse.current.position.ReadValue();
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

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        float depth = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
    }
}
