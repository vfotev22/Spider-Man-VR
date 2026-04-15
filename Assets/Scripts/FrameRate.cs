using UnityEngine;
using TMPro;

public class FrameRate : MonoBehaviour
{
    public TMP_Text fpsText;

    float deltaTime = 0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        fpsText.text = "FPS: " + Mathf.RoundToInt(fps);
    }
}