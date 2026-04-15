using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PhotoCounter : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI photoText;

    [Header("Settings")]
    public int photosNeeded = 3;

    private int currentPhotos = 0;

    void Start()
    {
        UpdateDisplay();
    }
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddPhoto();
        }
    }
    public void AddPhoto()
    {
        currentPhotos++;
        UpdateDisplay();

        if (currentPhotos >= photosNeeded)
        {
            OnPhotosComplete();
        }
    }

    void UpdateDisplay()
    {
        photoText.text = "Photos: " + currentPhotos + " / " + photosNeeded;
    }

    void OnPhotosComplete()
    {
        Debug.Log("Objective unlocked!");

    }

    public void ResetPhotos()
    {
        currentPhotos = 0;
        UpdateDisplay();
    }
}