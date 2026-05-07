using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public FloatingJoystickUI moveJoystick;

    [Header("Character")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float joystickDeadzone = 0.001f;
    [SerializeField] private Transform graphics;

    [Header("Camera")] 
    [SerializeField] private Transform cameraPole;

    public bool CanMove { get; set; } = true;

    private Vector3 moveDirection;
    
    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyRotation();
    }

    private void HandleMovementInput()
    {
        if (!CanMove)
        {
            moveDirection = Vector3.zero;
            return;
        }

        // Check if the joystick reference is present before reading its value
        Vector2 input = moveJoystick ? moveJoystick.InputVector : Vector2.zero;

        // Do not move the player if the joystick is within the deadzone
        if (input.sqrMagnitude < joystickDeadzone)
        {
            moveDirection = Vector3.zero;
            return;
        }

        // Move in a direction based on where the camera is pointing
        Vector3 forward = cameraPole ? cameraPole.forward : transform.forward;
        Vector3 right = cameraPole ? cameraPole.right : transform.right;

        // Don't modify the player's Y position
        forward.y = 0f;
        right.y = 0f;
        
        forward.Normalize();
        right.Normalize();

        // Set player movement
        moveDirection = (right * input.x + forward * input.y).normalized;
    }

    private void ApplyMovement()
    {
        // Get the current velocity
        Vector3 velocity = rb.linearVelocity;

        if (!CanMove)
        {
            velocity.x = 0f;
            velocity.z = 0f;
            rb.linearVelocity = velocity;
            return;
        }
        // Multiply the player's X and Y velocities with the move speed
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void ApplyRotation()
    {
        if (!CanMove || moveDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        if (graphics != null)
        {
            graphics.rotation = Quaternion.Slerp(
                graphics.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
        else
        {
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }
}