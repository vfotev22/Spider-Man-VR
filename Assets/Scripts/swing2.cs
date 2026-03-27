using UnityEngine;
using UnityEngine.InputSystem;

public class swing2 : MonoBehaviour
{
    [Header("References")]
    public Transform startSwingHand;
    public Transform player;                  // XR Origin root
    public CharacterController characterController;
    public Transform predictionPoint;
    public LineRenderer lineRenderer;

    [Header("Input")]
    public InputActionProperty swingAction;
    public InputActionProperty pullAction;

    [Header("Swing Settings")]
    public float maxDistance = 35f;
    public LayerMask swingableLayer;
    public float swingMoveSpeed = 8f;
    public float pullSpeed = 12f;
    public float ropeSlack = 0.5f;

    private Vector3 swingPoint;
    private bool hasHit;
    private bool swinging;
    private float currentRopeLength;

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

        if (swinging)
        {
            HandleSwingMovement();
        }

        DrawRope();
    }

    public void StartSwing()
    {
        if (!hasHit) return;

        swinging = true;
        currentRopeLength = Vector3.Distance(player.position, swingPoint);
    }

    public void StopSwing()
    {
        swinging = false;
    }

    public void GetSwingPoint()
    {
        if (swinging)
        {
            predictionPoint.gameObject.SetActive(false);
            return;
        }

        RaycastHit hit;
        hasHit = Physics.Raycast(
            startSwingHand.position,
            startSwingHand.forward,
            out hit,
            maxDistance,
            swingableLayer
        );

        if (hasHit)
        {
            swingPoint = hit.point;
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = swingPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }
    }

    private void HandleSwingMovement()
    {
        Vector3 playerPos = player.position;
        Vector3 toAnchor = swingPoint - playerPos;
        float distanceToAnchor = toAnchor.magnitude;

        if (distanceToAnchor <= 0.01f)
            return;

        Vector3 ropeDirection = toAnchor.normalized;

        if (pullAction.action.IsPressed())
        {
            currentRopeLength -= pullSpeed * Time.deltaTime;
            currentRopeLength = Mathf.Max(1.5f, currentRopeLength);
        }

        Vector3 handForwardFlat = Vector3.ProjectOnPlane(startSwingHand.forward, Vector3.up).normalized;
        Vector3 move = handForwardFlat * swingMoveSpeed * Time.deltaTime;

        Vector3 predictedPosition = player.position + move;
        float predictedDistance = Vector3.Distance(predictedPosition, swingPoint);

        if (predictedDistance > currentRopeLength - ropeSlack)
        {
            Vector3 offsetFromAnchor = predictedPosition - swingPoint;
            predictedPosition = swingPoint + offsetFromAnchor.normalized * (currentRopeLength - ropeSlack);
            move = predictedPosition - player.position;
        }

        characterController.Move(move);
    }

    public void DrawRope()
    {
        if (!swinging)
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