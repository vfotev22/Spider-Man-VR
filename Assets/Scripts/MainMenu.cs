using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menuRoot;

    [Header("Player Lock")]
    public ContinuousMovementPhysics movementScript;
    public Rigidbody playerRb;

    [Header("Disable While Menu Is Active")]
    public CameraGrab cameraGrab;
    public Swing leftSwing;
    public Swing rightSwing;
    public WallClimb wallClimb;
    public PauseMenu pauseMenu;

    [Header("UI To Hide While Menu Is Active")]
    public GameObject[] uiToHide;

    [Header("Menu Audio")]
    public AudioSource audioSource;
    public AudioClip gameStartVoiceLine;

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

    void Start()
    {
        if (gameObject.activeInHierarchy)
            LockPlayer();
    }

    void OnEnable()
    {
        LockPlayer();
    }

    void OnDisable()
    {
        UnlockPlayer();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        HandleMenuRaycast();
        HandleMenuConfirm();
    }

    public void LockPlayer()
    {
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.useGravity = true;
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

        SetHiddenUI(false);
    }

    public void UnlockPlayer()
    {
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

        SetHiddenUI(true);
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
        else
        {
            if (predictionPoint != null)
            {
                predictionPoint.gameObject.SetActive(true);
                predictionPoint.position = hand.position + hand.forward * rayDistance;
            }

            return null;
        }
    }

    void HandleMenuConfirm()
    {
        if (rightConfirmAction.action.WasPressedThisFrame())
            HandleSelection(rightTarget);

        if (leftConfirmAction.action.WasPressedThisFrame())
            HandleSelection(leftTarget);
    }

    void HandleSelection(GameObject target)
    {
        if (target == null)
            return;

        if (target.name == "Play")
            PlayGame();
        else if (target.name == "Exit")
            ExitGame();
    }

    public void PlayGame()
    {
        UnlockPlayer();

        if (menuRoot != null)
            menuRoot.SetActive(false);
        else
            gameObject.SetActive(false);

        if (audioSource != null && gameStartVoiceLine != null)
            audioSource.PlayOneShot(gameStartVoiceLine);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}