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
    [SerializeField] private CinemachineOrbitalFollow followCamera;
    [SerializeField] private Transform cameraPole;
    [SerializeField] private LayerMask cameraObstacleLayers;
    [SerializeField] private float cameraYawSensitivity = 0.15f;
    [SerializeField] private float minCameraDistance = 3f;
    [SerializeField] private float maxCameraDistance = 8f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float cameraHeight = 4f;
    
    private Vector2 lookInput;
    private Vector3 moveDirection;
    private Vector2 yawV;

    private float currentCameraDistance = 6f;
    
    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        yawV = Vector2.zero;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance, maxCameraDistance);
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

    public void HandleCameraRotation(Vector2 input)
    {
        yawV.x += input.x;
        yawV.y -= input.y;

        if (!followCamera) return; // Must have a valid reference to a Cinemachine camera
        
        // Orbital Follow Cinemachine cameras use the horizontal and vertical axes for camera positioning
        followCamera.HorizontalAxis.Value = yawV.x;
        followCamera.VerticalAxis.Value = yawV.y;
    }

    public void HandleCameraZoom(float pinchDelta)
    {
        // Calculate the camera distance with the camera zoom speed
        // Clamp the distance
        currentCameraDistance -= pinchDelta * zoomSpeed;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance, maxCameraDistance);
        // Orbital Follow Cinemachine cameras use Radius as the camera distance
        followCamera.Radius = currentCameraDistance;
    }

    private void HandleMovementInput()
    {
        Vector2 input = moveJoystick ? moveJoystick.InputVector : Vector2.zero;

        if (input.sqrMagnitude < joystickDeadzone)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 forward = cameraPole ? cameraPole.forward : transform.forward;
        Vector3 right = cameraPole ? cameraPole.right : transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = (right * input.x + forward * input.y).normalized;
    }

    private void ApplyMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void ApplyRotation()
    {
        if (moveDirection.sqrMagnitude < 0.001f)
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

    // private void UpdateCameraPosition()
    // {
    //     if (cameraPole == null || followCamera == null)
    //         return;
    //
    //     Vector3 backward = -cameraPole.forward;
    //     backward.y = 0f;
    //     backward.Normalize();
    //
    //     Vector3 desiredPosition = cameraPole.position + Vector3.up * cameraHeight + backward * currentCameraDistance;
    //     Vector3 origin = cameraPole.position + Vector3.up * cameraHeight;
    //     Vector3 direction = desiredPosition - origin;
    //     float distance = direction.magnitude;
    //
    //     if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, cameraObstacleLayers, QueryTriggerInteraction.Ignore))
    //     {
    //         //followCamera.position = hit.point - direction.normalized * 0.2f;
    //     }
    //     else
    //     {
    //         //followCamera.position = desiredPosition;
    //     }
    //
    //     //followCamera.LookAt(cameraPole.position + Vector3.up * cameraHeight);
    // }
}