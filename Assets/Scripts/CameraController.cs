// CameraController.cs
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    // Keep existing fields...
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragSpeed = 0.5f;
    [SerializeField] private string dragButton = "left";

    private bool isDragging = false;
    private Vector3 lastMouseScreenPosition;

    // Keep Awake...
    void Awake()
    {
        if (mainCamera == null) mainCamera = GetComponent<Camera>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogError("CameraController could not find a Camera!");
    }

    void Update()
    {
        HandleCameraDrag();
    }

    private void HandleCameraDrag()
    {

        if (mainCamera == null) return;

        Mouse currentMouse = Mouse.current;
        if (currentMouse == null) return;

        ButtonControl buttonToCheck;
        switch (dragButton.ToLower())
        {
            case "right": buttonToCheck = currentMouse.rightButton; break;
            case "middle": buttonToCheck = currentMouse.middleButton; break;
            default: buttonToCheck = currentMouse.leftButton; break;
        }

        bool dragButtonPressed = buttonToCheck.isPressed;
        Vector2 currentMouseScreenPos = currentMouse.position.ReadValue();

        // --- Start Dragging Condition Modified ---
        // Condition: Button pressed, not already dragging camera,
        // AND pointer not currently over UI, ***AND no specific UI drag is active***
        if (dragButtonPressed && !isDragging && !InputManager.IsPointerOverUI && !InputManager.IsSpecificUIDragging) // <-- Added check
        {
            isDragging = true;
            lastMouseScreenPosition = currentMouseScreenPos;
        }
        // --- Stop Dragging --- (logic remains the same)
        else if (!dragButtonPressed && isDragging)
        {
            isDragging = false;
        }

        // --- During Drag --- (logic remains the same, including mid-drag UI check)
        if (isDragging)
        {
            if (InputManager.IsPointerOverUI) // Check if pointer *enters* UI mid-drag
            {
                isDragging = false;
                return;
            }

            Vector2 screenDelta = currentMouseScreenPos - (Vector2)lastMouseScreenPosition;
            Vector3 worldMove = Vector3.zero;
            // worldMove calculation... (keep your existing ortho/perspective logic)
            if (mainCamera.orthographic) { /* ortho calc */ worldMove = new Vector3(-screenDelta.x, -screenDelta.y, 0) * (mainCamera.orthographicSize * 2f / Screen.height) * dragSpeed; }
            else { /* perspective calc */ worldMove = new Vector3(-screenDelta.x, -screenDelta.y, 0) * dragSpeed * 0.1f; } // Tune multiplier

            transform.Translate(worldMove, Space.World);
            lastMouseScreenPosition = currentMouseScreenPos;
        }
    }
}