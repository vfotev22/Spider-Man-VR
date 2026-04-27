using UnityEngine;
using UnityEngine.InputSystem;

public class CameraGrab : MonoBehaviour
{
    public Transform rightHand;
    public HandCameraDetector rightDetector;
    public InputActionProperty rightGripAction;

    public Vector3 heldLocalPosition = Vector3.zero;
    public Vector3 heldLocalRotation = Vector3.zero;

    private GameObject heldCamera;
    private Rigidbody heldCameraRb;
    private bool wasGripPressedLastFrame;
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
        bool rightCanGrab = rightDetector != null && rightDetector.IsTouchingCamera;

        if (rightPressed && !wasGripPressedLastFrame)
        {
            if (!IsHoldingCamera && rightCanGrab && isInCameraZone)
            {
                Grab(rightDetector.CurrentCamera);
            }
        }

        if (!rightPressed && wasGripPressedLastFrame)
        {
            if (IsHoldingCamera)
            {
                Release();
            }
        }

        wasGripPressedLastFrame = rightPressed;
    }

    private void Grab(Collider cameraCollider)
    {
        if (cameraCollider == null)
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

        heldCamera.transform.SetParent(rightHand);
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