using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementPhysics : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float acceleration = 35f;
    public float maxAccelerationForce = 40f;
    public bool onlyMoveWhenGrounded = false;
    public float airControlMultiplier = 0.4f;

    public float turnSpeed = 90f;

    public float jumpVelocity = 7f;

    public Swing leftSwing;
    public Swing rightSwing;
    public float swingMoveMultiplier = 0.3f;
    public float swingTangentBoost = 8f;

    public InputActionProperty moveInputSource;
    public InputActionProperty turnInputSource;
    public InputActionProperty jumpInputSource;

    public Rigidbody rb;
    public Transform directionSource;
    public Transform playerHead;
    public CapsuleCollider bodyCollider;

    public LayerMask groundLayer;
    public float groundCheckOffset = 0.05f;

    private Vector2 moveInputAxis;
    private Vector2 turnInputAxis;
    private bool jumpPressed;
    private bool isGrounded;

    public WallClimb wallClimb;

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
        if (wallClimb != null && wallClimb.IsClimbing)
            return;

        if (onlyMoveWhenGrounded && !isGrounded)
            return;

        bool isSwinging =
            (leftSwing != null && leftSwing.IsSwinging) ||
            (rightSwing != null && rightSwing.IsSwinging);

        Quaternion yaw = Quaternion.Euler(0f, directionSource.eulerAngles.y, 0f);
        Vector3 moveDirection = yaw * new Vector3(moveInputAxis.x, 0f, moveInputAxis.y);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float inputAmount = moveInputAxis.magnitude;

        if (inputAmount < 0.1f)
            return;

        if (isSwinging)
        {
            Swing activeSwing = null;

            if (leftSwing != null && leftSwing.IsSwinging)
                activeSwing = leftSwing;
            else if (rightSwing != null && rightSwing.IsSwinging)
                activeSwing = rightSwing;

            if (activeSwing != null)
            {
                Vector3 ropeDir = (rb.position - activeSwing.GetCurrentSwingPoint()).normalized;
                Vector3 tangentialMove = Vector3.ProjectOnPlane(moveDirection, ropeDir).normalized;

                rb.AddForce(tangentialMove * swingTangentBoost, ForceMode.Acceleration);
            }

            return;
        }

        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;

        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 targetHorizontalVelocity = moveDirection * moveSpeed;
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
        if (wallClimb != null && wallClimb.IsClimbing)
            return;

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