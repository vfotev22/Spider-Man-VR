using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenuRoot;

    [Header("Pause Input")]
    public InputActionProperty pauseAction;

    [Header("Player Lock")]
    public ContinuousMovementPhysics movementScript;
    public Rigidbody playerRb;

    [Header("Disable While Pause Menu Is Active")]
    public CameraGrab cameraGrab;
    public Swing leftSwing;
    public Swing rightSwing;
    public WallClimb wallClimb;

    [Header("Stopwatch")]
    public VRStopwatch stopwatchScript;

    [Header("UI To Hide While Pause Menu Is Active")]
    public GameObject[] uiToHide;

    [Header("Menu Raycast")]
    public Transform leftRayHand;
    public Transform rightRayHand;
    public Transform leftPredictionPoint;
    public Transform rightPredictionPoint;
    public InputActionProperty leftConfirmAction;
    public InputActionProperty rightConfirmAction;
    public float rayDistance = 10f;
    public LayerMask menuLayer;

    private GameObject leftTarget;
    private GameObject rightTarget;
    private bool isPaused;

    private Vector3 storedLinearVelocity;
    private Vector3 storedAngularVelocity;
    private bool storedUseGravity;
    private bool storedWasKinematic;

    void Start()
    {
        if (pauseAction.action != null)
            pauseAction.action.Enable();

        if (leftConfirmAction.action != null)
            leftConfirmAction.action.Enable();

        if (rightConfirmAction.action != null)
            rightConfirmAction.action.Enable();

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);

        if (leftPredictionPoint != null)
            leftPredictionPoint.gameObject.SetActive(false);

        if (rightPredictionPoint != null)
            rightPredictionPoint.gameObject.SetActive(false);

        isPaused = false;
    }

    void Update()
    {
        if (pauseAction.action != null && pauseAction.action.WasPressedThisFrame())
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (!isPaused)
            return;

        HandleMenuRaycast();
        HandleMenuConfirm();
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(true);

        LockPlayer();
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        UnlockPlayer();

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);
    }

    void LockPlayer()
    {
        if (playerRb != null)
        {
            storedLinearVelocity = playerRb.linearVelocity;
            storedAngularVelocity = playerRb.angularVelocity;
            storedUseGravity = playerRb.useGravity;
            storedWasKinematic = playerRb.isKinematic;

            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.useGravity = false;
            playerRb.isKinematic = true;
        }

        if (leftSwing != null)
            leftSwing.StopSwing();

        if (rightSwing != null)
            rightSwing.StopSwing();

        if (movementScript != null)
            movementScript.enabled = false;

        if (cameraGrab != null)
            cameraGrab.enabled = false;

        if (leftSwing != null)
            leftSwing.enabled = false;

        if (rightSwing != null)
            rightSwing.enabled = false;

        if (wallClimb != null)
            wallClimb.enabled = false;

        if (stopwatchScript != null)
            stopwatchScript.enabled = false;

        SetHiddenUI(false);
    }

    void UnlockPlayer()
    {
        if (playerRb != null)
        {
            playerRb.isKinematic = storedWasKinematic;
            playerRb.useGravity = storedUseGravity;
            playerRb.linearVelocity = storedLinearVelocity;
            playerRb.angularVelocity = storedAngularVelocity;
        }

        if (movementScript != null)
            movementScript.enabled = true;

        if (cameraGrab != null)
            cameraGrab.enabled = true;

        if (leftSwing != null)
            leftSwing.enabled = true;

        if (rightSwing != null)
            rightSwing.enabled = true;

        if (wallClimb != null)
            wallClimb.enabled = true;

        if (stopwatchScript != null)
            stopwatchScript.enabled = true;

        SetHiddenUI(true);

        if (leftPredictionPoint != null)
            leftPredictionPoint.gameObject.SetActive(false);

        if (rightPredictionPoint != null)
            rightPredictionPoint.gameObject.SetActive(false);

        leftTarget = null;
        rightTarget = null;
    }

    void SetHiddenUI(bool isVisible)
    {
        if (uiToHide == null)
            return;

        foreach (GameObject ui in uiToHide)
        {
            if (ui != null)
                ui.SetActive(isVisible);
        }
    }

    void HandleMenuRaycast()
    {
        leftTarget = RaycastFromHand(leftRayHand, leftPredictionPoint);
        rightTarget = RaycastFromHand(rightRayHand, rightPredictionPoint);
    }

    GameObject RaycastFromHand(Transform hand, Transform predictionPoint)
    {
        if (hand == null)
            return null;

        Ray ray = new Ray(hand.position, hand.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, menuLayer))
        {
            if (predictionPoint != null)
            {
                predictionPoint.gameObject.SetActive(true);
                predictionPoint.position = hit.point;
            }

            return hit.collider.gameObject;
        }

        if (predictionPoint != null)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = hand.position + hand.forward * rayDistance;
        }

        return null;
    }

    void HandleMenuConfirm()
    {
        if (rightConfirmAction.action != null && rightConfirmAction.action.WasPressedThisFrame())
            HandleSelection(rightTarget);

        if (leftConfirmAction.action != null && leftConfirmAction.action.WasPressedThisFrame())
            HandleSelection(leftTarget);
    }

    void HandleSelection(GameObject target)
    {
        if (target == null)
            return;

        if (target.name == "Play")
        {
            ResumeGame();
        }
        else if (target.name == "Exit")
        {
            ExitGame();
        }
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}