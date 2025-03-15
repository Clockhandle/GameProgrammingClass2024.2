using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool isRunning = false;
    private Vector2 _moveVelocity;
    private Rigidbody2D _rb;
    public float maxRunSpeed = 7.0f;
    public float maxWalkSpeed = 3.0f;
    private Vector2 moveInput;
    private bool _isFacingRight = true;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Move(1f, 0.9f, moveInput);
    }

    private void Move(float acceleration, float decceleration, Vector2 moveInput)
    {
        FlipCharacter(moveInput);
        if (moveInput != Vector2.zero)
        {
            Vector2 targetVelocity = Vector2.zero;
            if (isRunning)
            {
               targetVelocity = new Vector2(moveInput.x, 0f) * maxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * maxWalkSpeed;
            }

            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
        }

        else if (moveInput == Vector2.zero) 
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, decceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
        }
    }

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log(context.ReadValue<Vector2>());
    }

    private void FlipCharacter(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0f)
        {
            Flip(true);
        }
        else if(!_isFacingRight && moveInput.x > 0f)
        {
            Flip(false);
        }
    }

    private void Flip(bool flip)
    {
        if(flip)
        {
            _isFacingRight = false;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isFacingRight = true;
            transform.Rotate(0f, -180f, 0f);
        }
    }


}
