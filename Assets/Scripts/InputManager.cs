using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager gameplayInstance;
    [SerializeField] private EventChannelSO gameEvent; // Assign this in the Editor
    public bool isLeftMouseDown;

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

    public void OnLeftMouseDown(InputAction.CallbackContext context)
    {
        isLeftMouseDown = context.performed;
    }
}
