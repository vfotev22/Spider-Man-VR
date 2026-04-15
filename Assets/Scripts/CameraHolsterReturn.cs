using UnityEngine;

public class CameraHolsterReturn : MonoBehaviour
{
    public Transform hipPoint;
    public Vector3 holsteredLocalPosition = Vector3.zero;
    public Vector3 holsteredLocalRotation = Vector3.zero;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool canReturnToHolster = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canReturnToHolster)
            return;

        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;

        ReturnToHolster();
    }

    public void ReturnToHolster()
    {
        if (hipPoint == null)
            return;

        transform.SetParent(hipPoint);
        transform.localPosition = holsteredLocalPosition;
        transform.localRotation = Quaternion.Euler(holsteredLocalRotation);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }
    }
}