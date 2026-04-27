using UnityEngine;
using UnityEngine.InputSystem;

public class WallClimb : MonoBehaviour
{
    public Rigidbody playerRb;

    public Transform leftHand;
    public Transform rightHand;

    public HandClimbDetector leftDetector;
    public HandClimbDetector rightDetector;

    public HandCameraDetector leftCameraDetector;
    public HandCameraDetector rightCameraDetector;

    public CameraGrab cameraGrab;

    public InputActionProperty leftGripAction;
    public InputActionProperty rightGripAction;

    public float climbVelocityMultiplier = 1.4f;
    public float velocityLerpSpeed = 12f;
    public float maxClimbSpeed = 6f;
    public float climbDrag = 20f;
    public float handSwitchBoost = 2.5f;
    public float climbAssistUpward = 1.5f;

    private enum ClimbHand
    {
        None,
        Left,
        Right
    }

    private ClimbHand activeHand = ClimbHand.None;
    private Vector3 previousActiveHandPosition;

    public bool IsClimbing => activeHand != ClimbHand.None;

    void Update()
    {
        bool leftPressed = leftGripAction.action.IsPressed();
        bool rightPressed = rightGripAction.action.IsPressed();

        bool cameraIsHeld = cameraGrab != null && cameraGrab.IsHoldingCamera;

        bool leftBlockedByCamera =
            cameraIsHeld ||
            (leftCameraDetector != null && leftCameraDetector.IsTouchingCamera);

        bool rightBlockedByCamera =
            cameraIsHeld ||
            (rightCameraDetector != null && rightCameraDetector.IsTouchingCamera);

        bool leftCanClimb =
            leftDetector != null &&
            leftDetector.IsTouchingClimbable &&
            !leftBlockedByCamera;

        bool rightCanClimb =
            rightDetector != null &&
            rightDetector.IsTouchingClimbable &&
            !rightBlockedByCamera;

        if (cameraIsHeld)
        {
            activeHand = ClimbHand.None;
            return;
        }

        if (leftPressed && leftCanClimb && activeHand != ClimbHand.Left)
        {
            activeHand = ClimbHand.Left;
            previousActiveHandPosition = leftHand.position;
            playerRb.linearVelocity += Vector3.up * handSwitchBoost;
        }
        else if (rightPressed && rightCanClimb && activeHand != ClimbHand.Right)
        {
            activeHand = ClimbHand.Right;
            previousActiveHandPosition = rightHand.position;
            playerRb.linearVelocity += Vector3.up * handSwitchBoost;
        }

        if (activeHand == ClimbHand.Left && !leftPressed)
        {
            if (rightPressed && rightCanClimb)
            {
                activeHand = ClimbHand.Right;
                previousActiveHandPosition = rightHand.position;
                playerRb.linearVelocity += Vector3.up * handSwitchBoost;
            }
            else
            {
                activeHand = ClimbHand.None;
            }
        }

        if (activeHand == ClimbHand.Right && !rightPressed)
        {
            if (leftPressed && leftCanClimb)
            {
                activeHand = ClimbHand.Left;
                previousActiveHandPosition = leftHand.position;
                playerRb.linearVelocity += Vector3.up * handSwitchBoost;
            }
            else
            {
                activeHand = ClimbHand.None;
            }
        }
    }

    void FixedUpdate()
    {
        if (cameraGrab != null && cameraGrab.IsHoldingCamera)
        {
            activeHand = ClimbHand.None;
            playerRb.useGravity = true;
            playerRb.linearDamping = 0f;
            return;
        }

        if (activeHand == ClimbHand.None)
        {
            playerRb.useGravity = true;
            playerRb.linearDamping = 0f;
            return;
        }

        playerRb.useGravity = false;
        playerRb.linearDamping = climbDrag;

        Transform currentHand = activeHand == ClimbHand.Left ? leftHand : rightHand;

        Vector3 handDelta = currentHand.position - previousActiveHandPosition;

        if (handDelta.magnitude < 0.005f)
            handDelta = Vector3.zero;

        Vector3 climbVelocity = -(handDelta / Time.fixedDeltaTime) * climbVelocityMultiplier;
        climbVelocity += Vector3.up * climbAssistUpward;
        climbVelocity = Vector3.ClampMagnitude(climbVelocity, maxClimbSpeed);

        playerRb.linearVelocity = Vector3.Lerp(
            playerRb.linearVelocity,
            climbVelocity,
            velocityLerpSpeed * Time.fixedDeltaTime
        );

        previousActiveHandPosition = currentHand.position;
    }
}