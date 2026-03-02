using UnityEngine;
using UnityEngine.InputSystem;

public class ContinousMovementPhysics : MonoBehaviour
{
    public float speed = 1;
    public float jumpVelocity = 7;

    public bool onlyMoveWhenGrounded = false;
    
    public InputActionProperty moveInputSource;
    public InputActionProperty jumpInputSource;
    public Rigidbody rb;

    public LayerMask groundLayer;

    public Transform directionSource;
    public CapsuleCollider bodyCollider;

    private Vector2 inputMoveAxis;
    private bool isGrounded;

    void Update()
    {
        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();

        if(jumpInput && isGrounded)
        {
            rb.linearVelocity = Vector3.up * jumpVelocity;
        }
    }

    void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();

        if(!onlyMoveWhenGrounded || (onlyMoveWhenGrounded && isGrounded))
        {
            Quaternion yaw = Quaternion.Euler(0, directionSource.eulerAngles.y, 0);
            Vector3 direction = yaw * new Vector3(inputMoveAxis.x, 0, inputMoveAxis.y);

            rb.MovePosition(rb.position + direction * Time.fixedDeltaTime * speed);
        }
        
    }

    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;

        bool hasHit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);

        return hasHit;
    }
}
