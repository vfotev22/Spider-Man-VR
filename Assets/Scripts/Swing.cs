using UnityEngine;
using UnityEngine.InputSystem;

public class Swing : MonoBehaviour
{
    [Header("References")]
    public Transform startSwingHand;
    public Transform predictionPoint;
    public Rigidbody playerRigidbody;
    public LineRenderer lineRenderer;

    [Header("Input")]
    public InputActionProperty swingAction;
    public InputActionProperty pullAction;

    [Header("Swing Settings")]
    public float maxDistance = 35f;
    public LayerMask swingableLayer;
    public float reelSpeed = 5f;
    public float minRopeLength = 2f;
    public float pullAssistForce = 2f;

    [Header("Joint Settings")]
    public float spring = 8f;
    public float damper = 0.4f;
    public float massScale = 4.5f;
    public float minDistanceMultiplier = 0.5f;
    public float maxDistanceMultiplier = 0.6f;

    private SpringJoint joint;
    private Vector3 swingPoint;
    private bool hasHit;

    public bool IsSwinging => joint != null;

    public Vector3 GetCurrentSwingPoint()
    {
        return swingPoint;
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
        PullRope();
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

    public void PullRope()
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

            Vector3 direction = (swingPoint - playerRigidbody.position).normalized;
            playerRigidbody.AddForce(direction * pullAssistForce, ForceMode.Acceleration);
        }
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