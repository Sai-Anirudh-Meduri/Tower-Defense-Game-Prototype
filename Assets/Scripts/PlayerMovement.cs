using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Mouse Look")]
    [SerializeField] public float mouseSensitivity = 100f;
    [SerializeField] private Transform cameraHolder;
    [Tooltip("Maximum degrees the player may look DOWN from horizontal (0-90).")]
    [SerializeField, Range(0f, 90f)] private float lookDownLimit = 75f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Slope Sliding")]
    [SerializeField] private float slideForce = 175f;
    [SerializeField] private float maxGroundAngle = 60f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Animator animator;

    private Vector2 inputMovement;
    private bool jumpPressed;
    private bool runHeld;

    private float mouseX;
    private float mouseY;
    private float xRotation = 0f;

    private bool isGrounded;
    private Vector3 groundNormal = Vector3.up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ReadInput();
        RotateView();
        CheckGrounded();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false; // reset jump
        }

        if (isGrounded && IsOnSlope())
        {
            SlideDownSlope();
        }
    }

    private void ReadInput()
    {
        inputMovement = Vector2.zero;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        // WASD movement input
        if (keyboard.wKey.isPressed) inputMovement.y += 1;
        if (keyboard.sKey.isPressed) inputMovement.y -= 1;
        if (keyboard.aKey.isPressed) inputMovement.x -= 1;
        if (keyboard.dKey.isPressed) inputMovement.x += 1;
        inputMovement = Vector2.ClampMagnitude(inputMovement, 1f);

        runHeld = keyboard.leftShiftKey.isPressed;

        // Spacebar jump
        if (keyboard.spaceKey.wasPressedThisFrame)
            jumpPressed = true;

        // Mouse look
        Vector2 mouseDelta = mouse.delta.ReadValue();
        mouseX = mouseDelta.x;
        mouseY = mouseDelta.y;
    }

    private void Move()
    {
        float speed = runHeld ? runSpeed : walkSpeed;

        Vector3 move = new Vector3(inputMovement.x, 0f, inputMovement.y);
        Vector3 moveDirection = transform.TransformDirection(move) * speed;

        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    private void RotateView()
    {
        float yaw = mouseX * mouseSensitivity * Time.deltaTime;
        float pitch = mouseY * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * yaw);

        xRotation -= pitch;
        xRotation = Mathf.Clamp(xRotation, -90f, lookDownLimit);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void CheckGrounded()
    {
        float radius = capsuleCollider.radius * 0.95f;
        // Start from the *bottom* of the capsule
        Vector3 origin = transform.position + Vector3.up * (radius + 0.05f);

        RaycastHit hit;
        float castDistance = 0.2f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out hit, castDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
            groundNormal = hit.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    private bool IsOnSlope()
    {
        float angle = Vector3.Angle(groundNormal, Vector3.up);
        return angle > 0.1f && angle <= maxGroundAngle;
    }

    private void SlideDownSlope()
    {
        Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
        rb.AddForce(slopeDirection * slideForce, ForceMode.Acceleration);
    }

    private void HandleAnimations()
    {
        if (animator == null) return;

        bool isMoving = inputMovement.magnitude > 0.1f;

        if (isMoving)
            animator.Play("Player Walk");
        else
            animator.Play("Player Idle");
    }
}

