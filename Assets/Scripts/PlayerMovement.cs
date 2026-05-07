using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public FloatingJoystickUI moveJoystick;
    [SerializeField] private Animator animator;

    [Header("Character")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float joystickDeadzone = 0.001f;
    [SerializeField] private Transform graphics;

    [Header("Camera")]
    [SerializeField] private Transform cameraPole;

    [Header("Animation")]
    [SerializeField] private float animationDampTime = 0.1f;

    public bool CanMove { get; set; } = true;

    private Vector3 moveDirection;
    private float moveAmount;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleMovementInput();
        UpdateAnimator();
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
            moveAmount = 0f;
            return;
        }

        Vector2 input = moveJoystick ? moveJoystick.InputVector : Vector2.zero;
        input = Vector2.ClampMagnitude(input, 1f);

        if (input.sqrMagnitude < joystickDeadzone)
        {
            moveDirection = Vector3.zero;
            moveAmount = 0f;
            return;
        }

        Vector3 forward = cameraPole ? cameraPole.forward : transform.forward;
        Vector3 right = cameraPole ? cameraPole.right : transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = right * input.x + forward * input.y;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        moveAmount = Mathf.Clamp01(input.magnitude);
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
    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        float targetSpeed = CanMove ? moveAmount : 0f;

        animator.SetFloat(
            SpeedHash,
            targetSpeed,
            animationDampTime,
            Time.deltaTime
        );
    }
}