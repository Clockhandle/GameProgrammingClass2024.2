using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager gameplayInstance;
    [SerializeField] private EventChannelSO gameEvent; // Assign this in the Editor

    //Input Events
    public UnityEvent OnLeftClickDown;
    public UnityEvent OnLeftClickRelease;
    public UnityEvent OnRightClickDown;
    public UnityEvent OnRightClickRelease;

    //Pretained input reference
    public bool isLeftClickBeingPressed;
    private void Awake()
    {
        if (gameplayInstance == null)
        {
            gameplayInstance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates if multiple InputManagers exist
        }
    }
    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            gameEvent.Raise();
        }
    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnLeftClickDown?.Invoke();
            isLeftClickBeingPressed = true;
        }
        else if(context.canceled)
        {
            isLeftClickBeingPressed = false;
            OnLeftClickRelease?.Invoke();
        }
    }

    public void OnRightMouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRightClickDown?.Invoke();
        }
        else if (context.canceled)
        {
            OnRightClickRelease?.Invoke();
        }
    }
}
