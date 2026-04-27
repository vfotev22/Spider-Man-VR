using UnityEngine;
using UnityEngine.InputSystem;

public class CameraGrab : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;

    public HandCameraDetector rightDetector;
    public HandCameraDetector leftDetector;

    public InputActionProperty rightGripAction;
    public InputActionProperty leftGripAction;

    public Vector3 heldLocalPosition = Vector3.zero;
    public Vector3 heldLocalRotation = Vector3.zero;

    private GameObject heldCamera;
    private Rigidbody heldCameraRb;
    private bool wasRightGripPressedLastFrame;
    private bool wasLeftGripPressedLastFrame;
    private Collider[] playerColliders;
    private Collider[] heldCameraColliders;
    private bool isInCameraZone = false;

    public bool IsHoldingCamera => heldCamera != null;
    public GameObject HeldCamera => heldCamera;

    void Start()
    {
        playerColliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        bool rightPressed = rightGripAction.action.IsPressed();
        bool leftPressed = leftGripAction.action.IsPressed();

        bool rightCanGrab = rightDetector != null && rightDetector.IsTouchingCamera;
        bool leftCanGrab = leftDetector != null && leftDetector.IsTouchingCamera;

        if (!IsHoldingCamera && isInCameraZone)
        {
            if (rightPressed && !wasRightGripPressedLastFrame && rightCanGrab)
            {
                Grab(rightDetector.CurrentCamera, rightHand);
            }
            else if (leftPressed && !wasLeftGripPressedLastFrame && leftCanGrab)
            {
                Grab(leftDetector.CurrentCamera, leftHand);
            }
        }

        if (IsHoldingCamera)
        {
            if ((!rightPressed && wasRightGripPressedLastFrame) ||
                (!leftPressed && wasLeftGripPressedLastFrame))
            {
                Release();
            }
        }

        wasRightGripPressedLastFrame = rightPressed;
        wasLeftGripPressedLastFrame = leftPressed;
    }

    private void Grab(Collider cameraCollider, Transform hand)
    {
        if (cameraCollider == null || hand == null)
            return;

        heldCameraRb = cameraCollider.attachedRigidbody;

        if (heldCameraRb != null)
            heldCamera = heldCameraRb.gameObject;
        else
            heldCamera = cameraCollider.gameObject;

        heldCameraColliders = heldCamera.GetComponentsInChildren<Collider>();

        if (heldCameraRb != null)
        {
            heldCameraRb.linearVelocity = Vector3.zero;
            heldCameraRb.angularVelocity = Vector3.zero;
            heldCameraRb.isKinematic = true;
            heldCameraRb.useGravity = false;
            heldCameraRb.detectCollisions = true;
            heldCameraRb.interpolation = RigidbodyInterpolation.None;
        }

        foreach (var heldCol in heldCameraColliders)
        {
            foreach (var playerCol in playerColliders)
            {
                Physics.IgnoreCollision(heldCol, playerCol, true);
            }
        }

        heldCamera.transform.SetParent(hand);
        heldCamera.transform.localPosition = heldLocalPosition;
        heldCamera.transform.localRotation = Quaternion.Euler(heldLocalRotation);

        Debug.Log("[CAMERA] Grabbed camera");
    }

    private void Release()
    {
        if (heldCamera == null)
            return;

        heldCamera.transform.SetParent(null);

        foreach (var heldCol in heldCameraColliders)
        {
            foreach (var playerCol in playerColliders)
            {
                Physics.IgnoreCollision(heldCol, playerCol, false);
            }
        }

        if (heldCameraRb != null)
        {
            heldCameraRb.detectCollisions = true;
            heldCameraRb.isKinematic = false;
            heldCameraRb.useGravity = true;
            heldCameraRb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        Debug.Log("[CAMERA] Released camera");

        heldCamera = null;
        heldCameraRb = null;
        heldCameraColliders = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraZone"))
        {
            isInCameraZone = true;
            Debug.Log("[CAMERA ZONE] Entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraZone"))
        {
            isInCameraZone = false;
            Debug.Log("[CAMERA ZONE] Exited");
        }
    }
}