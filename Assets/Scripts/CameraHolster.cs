using UnityEngine;

public class CameraHolster : MonoBehaviour
{
    public Transform cameraObject;
    public Transform hipPoint;
    public Vector3 holsteredLocalPosition = Vector3.zero;
    public Vector3 holsteredLocalRotation = Vector3.zero;

    void Start()
    {
        HolsterCamera();
    }

    public void HolsterCamera()
    {
        if (cameraObject == null || hipPoint == null)
            return;

        Rigidbody rb = cameraObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        cameraObject.SetParent(hipPoint);
        cameraObject.localPosition = holsteredLocalPosition;
        cameraObject.localRotation = Quaternion.Euler(holsteredLocalRotation);
    }
}