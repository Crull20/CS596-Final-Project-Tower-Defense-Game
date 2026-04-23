using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private FloatingJoystickUI moveJoystick;

    [Header("Character")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private Transform graphics;

    [Header("Camera")]
    [SerializeField] private Transform cameraPole;
    [SerializeField] private Transform followCamera;
    [SerializeField] private LayerMask cameraObstacleLayers;
    [SerializeField] private float cameraYawSensitivity = 0.15f;
    [SerializeField] private float minCameraDistance = 3f;
    [SerializeField] private float maxCameraDistance = 8f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float cameraHeight = 4f;
    [SerializeField] private float lookTargetHeight = 1.5f;
    [SerializeField] private float cameraPitch = 40f;

    private int rightFingerId = -1;
    private float halfScreenWidth;

    private Vector2 lookInput;
    private Vector3 moveDirection;

    private float yaw;
    private float currentCameraDistance = 6f;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        halfScreenWidth = Screen.width * 0.5f;
        yaw = transform.eulerAngles.y;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance, maxCameraDistance);
    }

    private void Update()
    {
        HandleTouchInput();
        HandleCameraRotation();
        HandlePinchZoom();
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyRotation();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void HandleTouchInput()
    {
        lookInput = Vector2.zero;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (moveJoystick != null && moveJoystick.IsMyFinger(touch.fingerId))
            {
                moveJoystick.ProcessTouch(touch);
                continue;
            }

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                continue;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (touch.position.x < halfScreenWidth)
                    {
                        if (moveJoystick != null && !moveJoystick.IsHeld)
                            moveJoystick.TryBegin(touch, halfScreenWidth);
                    }
                    else if (rightFingerId == -1 && Input.touchCount == 1)
                    {
                        rightFingerId = touch.fingerId;
                    }
                    break;

                case TouchPhase.Moved:
                    if (touch.fingerId == rightFingerId && Input.touchCount == 1)
                        lookInput = touch.deltaPosition * cameraYawSensitivity;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    private void HandleCameraRotation()
    {
        if (Input.touchCount != 1 || rightFingerId == -1)
            return;

        yaw += lookInput.x;

        if (cameraPole != null)
            cameraPole.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private void HandlePinchZoom()
    {
        if (Input.touchCount != 2)
            return;

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);

        float pinchDelta = currentDistance - prevDistance;

        currentCameraDistance -= pinchDelta * zoomSpeed;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance, maxCameraDistance);
    }

    private void HandleMovementInput()
    {
        Vector2 input = moveJoystick != null ? moveJoystick.InputVector : Vector2.zero;

        if (input.sqrMagnitude < 0.001f)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 forward = cameraPole != null ? cameraPole.forward : transform.forward;
        Vector3 right = cameraPole != null ? cameraPole.right : transform.right;

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

    private void UpdateCameraPosition()
    {
        if (cameraPole == null || followCamera == null)
            return;

        Vector3 target = cameraPole.position + Vector3.up * lookTargetHeight;

        Quaternion orbitRotation = Quaternion.Euler(cameraPitch, yaw, 0f);
        Vector3 desiredPosition = target + orbitRotation * new Vector3(0f, 0f, -currentCameraDistance);

        Vector3 direction = desiredPosition - target;
        float distance = direction.magnitude;

        if (Physics.Raycast(target, direction.normalized, out RaycastHit hit, distance, cameraObstacleLayers, QueryTriggerInteraction.Ignore))
        {
            followCamera.position = hit.point - direction.normalized * 0.2f;
        }
        else
        {
            followCamera.position = desiredPosition;
        }

        followCamera.LookAt(target);
    }
}