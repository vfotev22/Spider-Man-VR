using UnityEngine;
using UnityEngine.InputSystem;  // <-- new Input System

public class HighResScreenshot : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ScreenCapture.CaptureScreenshot("poster.png", 8);
            Debug.Log("Screenshot taken!");
        }
    }
}