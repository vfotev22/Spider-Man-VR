using UnityEngine;
using UnityEngine.InputSystem;

public class Swing : MonoBehaviour
{
    [Header("References")]
    public Transform startSwingHand;
    public Transform playerBody;
    public Transform predictionPoint;
    public Rigidbody playerRigidbody;
    public LineRenderer lineRenderer;

    [Header("Input")]
    public InputActionProperty swingAction;
    public InputActionProperty pullAction;

    [Header("Swing Settings")]
    public float maxDistance = 35f;
    public LayerMask swingableLayer;

    [Header("Trigger Reel")]
    public float reelSpeed = 5f;
    public float minRopeLength = 2f;
    public float triggerPullForce = 8f;
    public float triggerPullUpwardBoost = 0f;

    [Header("Physical Pull Motion")]
    public float pullMotionThreshold = .9f;
    public float pullMotionVelocityBoost = 6f;
    public float maxPullSpeedTowardWeb = 12f;
    public float pullMotionCooldown = 0.15f;

    [Header("Joint Settings")]
    public float spring = 8f;
    public float damper = 0.4f;
    public float massScale = 4.5f;
    public float minDistanceMultiplier = 0.5f;
    public float maxDistanceMultiplier = 0.6f;

    private SpringJoint joint;
    private Vector3 swingPoint;
    private bool hasHit;

    private Vector3 previousHandPosition;
    private float pullCooldownTimer;

    public bool IsSwinging => joint != null;

    public Vector3 GetCurrentSwingPoint()
    {
        return swingPoint;
    }

    void Start()
    {
        previousHandPosition = startSwingHand.position;
    }

    void Update()
    {
        GetSwingPoint();

        if (swingAction.action.WasPressedThisFrame())
        {
            StartSwing();
        }
        else if (swingAction.action.WasReleasedThisFrame())
        {
            StopSwing();
        }

        DrawRope();
    }

    void FixedUpdate()
    {
        if (pullCooldownTimer > 0f)
            pullCooldownTimer -= Time.fixedDeltaTime;

        PullRopeWithTrigger();
        DetectPhysicalPullMotion();

        previousHandPosition = startSwingHand.position;
    }

    public void StartSwing()
    {
        if (!hasHit || joint != null)
            return;

        joint = playerRigidbody.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distance = Vector3.Distance(playerRigidbody.position, swingPoint);

        joint.maxDistance = distance * maxDistanceMultiplier;
        joint.minDistance = distance * minDistanceMultiplier;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    public void PullRopeWithTrigger()
    {
        if (joint == null)
            return;

        if (pullAction.action.IsPressed())
        {
            joint.maxDistance = Mathf.Max(
                joint.maxDistance - reelSpeed * Time.fixedDeltaTime,
                minRopeLength
            );

            joint.minDistance = Mathf.Max(
                joint.minDistance - (reelSpeed * 0.5f) * Time.fixedDeltaTime,
                minRopeLength * 0.5f
            );

            Vector3 directionToWeb = (swingPoint - playerRigidbody.position).normalized;
            Vector3 pullDirection = (directionToWeb + Vector3.up * triggerPullUpwardBoost).normalized;

            playerRigidbody.AddForce(pullDirection * triggerPullForce, ForceMode.Acceleration);
        }
    }

    public void DetectPhysicalPullMotion()
    {
        if (joint == null || playerBody == null)
            return;

        if (pullCooldownTimer > 0f)
            return;

        Vector3 handVelocity = (startSwingHand.position - previousHandPosition) / Time.fixedDeltaTime;

        Vector3 handToBodyDirection = (playerBody.position - startSwingHand.position).normalized;
        float pullAmount = Vector3.Dot(handVelocity, handToBodyDirection);

        if (pullAmount < pullMotionThreshold)
            return;

        Vector3 directionToWeb = (swingPoint - playerRigidbody.position).normalized;

        float currentSpeedTowardWeb = Vector3.Dot(playerRigidbody.linearVelocity, directionToWeb);
        float allowedBoost = Mathf.Max(0f, maxPullSpeedTowardWeb - currentSpeedTowardWeb);

        if (allowedBoost <= 0f)
            return;

        float appliedBoost = Mathf.Min(pullMotionVelocityBoost, allowedBoost);

        playerRigidbody.AddForce(directionToWeb * appliedBoost, ForceMode.VelocityChange);

        pullCooldownTimer = pullMotionCooldown;
    }

    public void StopSwing()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
    }

    public void GetSwingPoint()
    {
        if (joint != null)
        {
            if (predictionPoint != null)
                predictionPoint.gameObject.SetActive(false);
            return;
        }

        hasHit = Physics.Raycast(
            startSwingHand.position,
            startSwingHand.forward,
            out RaycastHit raycastHit,
            maxDistance,
            swingableLayer
        );

        if (hasHit)
        {
            swingPoint = raycastHit.point;

            if (predictionPoint != null)
            {
                predictionPoint.gameObject.SetActive(true);
                predictionPoint.position = swingPoint;
            }
        }
        else
        {
            if (predictionPoint != null)
                predictionPoint.gameObject.SetActive(false);
        }
    }

    public void DrawRope()
    {
        if (joint == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startSwingHand.position);
        lineRenderer.SetPosition(1, swingPoint);
    }
}