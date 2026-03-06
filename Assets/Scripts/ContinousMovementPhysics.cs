using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementPhysics : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 35f;
    public float maxAccelerationForce = 40f;
    public bool onlyMoveWhenGrounded = false;
    public float airControlMultiplier = 0.4f;

    [Header("Turning")]
    public float turnSpeed = 90f;

    [Header("Jump")]
    public float jumpVelocity = 7f;

    [Header("Swing")]
    public Swing leftSwing;
    public Swing rightSwing;
    public float swingMoveMultiplier = 0.3f;

    [Header("Input")]
    public InputActionProperty moveInputSource;
    public InputActionProperty turnInputSource;
    public InputActionProperty jumpInputSource;

    [Header("References")]
    public Rigidbody rb;
    public Transform directionSource;
    public Transform playerHead;
    public CapsuleCollider bodyCollider;

    [Header("Grounding")]
    public LayerMask groundLayer;
    public float groundCheckOffset = 0.05f;

    private Vector2 moveInputAxis;
    private Vector2 turnInputAxis;
    private bool jumpPressed;
    private bool isGrounded;

    void Update()
    {
        moveInputAxis = moveInputSource.action.ReadValue<Vector2>();
        turnInputAxis = turnInputSource.action.ReadValue<Vector2>();

        if (jumpInputSource.action.WasPressedThisFrame())
        {
            jumpPressed = true;
        }

        HandleTurning();
    }

    void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();

        HandleMovement();
        HandleJump();

        jumpPressed = false;
    }

    void HandleMovement()
    {
        if (onlyMoveWhenGrounded && !isGrounded)
            return;

        bool isSwinging =
            (leftSwing != null && leftSwing.IsSwinging) ||
            (rightSwing != null && rightSwing.IsSwinging);

        Quaternion yaw = Quaternion.Euler(0f, directionSource.eulerAngles.y, 0f);
        Vector3 moveDirection = yaw * new Vector3(moveInputAxis.x, 0f, moveInputAxis.y);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float inputAmount = moveInputAxis.magnitude;

        // While swinging, don't kill momentum if the player isn't pushing the stick
        if (isSwinging && inputAmount < 0.1f)
            return;

        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;

        float speedMultiplier = isSwinging ? swingMoveMultiplier : 1f;

        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 targetHorizontalVelocity = moveDirection * moveSpeed * speedMultiplier;
        Vector3 velocityDifference = targetHorizontalVelocity - currentHorizontalVelocity;

        Vector3 force = velocityDifference * acceleration * controlMultiplier;
        force = Vector3.ClampMagnitude(force, maxAccelerationForce);

        rb.AddForce(force, ForceMode.Acceleration);
    }

    void HandleTurning()
    {
        float turnAmount = turnInputAxis.x;

        if (Mathf.Abs(turnAmount) < 0.1f)
            return;

        float rotationAmount = turnAmount * turnSpeed * Time.deltaTime;
        transform.RotateAround(playerHead.position, Vector3.up, rotationAmount);
    }

    void HandleJump()
    {
        if (!jumpPressed || !isGrounded)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpVelocity;
        rb.linearVelocity = velocity;
    }

    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = (bodyCollider.height * 0.5f) - bodyCollider.radius + groundCheckOffset;

        bool hasHit = Physics.SphereCast(
            start,
            bodyCollider.radius * 0.95f,
            Vector3.down,
            out RaycastHit hitInfo,
            rayLength,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        return hasHit;
    }
}