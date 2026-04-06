using UnityEngine;
using TMPro;

public class VRStopwatch : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;

    [Header("Settings")]
    public bool facePlayer = true;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        isRunning = true;
        UpdateDisplay();
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplay();
        }
    }

    void LateUpdate()
    {
        if (facePlayer && Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);

        timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        UpdateDisplay();
    }
}