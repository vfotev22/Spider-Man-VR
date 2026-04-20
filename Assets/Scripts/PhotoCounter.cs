using UnityEngine;
using TMPro;
using System.Collections;

public class PhotoCounter : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI photoText;

    [Header("Settings")]
    public int photosNeeded = 3;

    [Header("Stopwatch")]
    public VRStopwatch stopwatch;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip completionVoice;
    public float voiceDelay = 3f;

    private int currentPhotos = 0;

    public int CurrentPhotos => currentPhotos;
    public int PhotosNeeded => photosNeeded;

    public float finalTimeValue;
    public string finalFormattedTime;

    private bool completionRecorded = false;

    void Start()
    {
        UpdateDisplay();
    }

    public void AddPhoto()
    {
        if (completionRecorded)
            return;

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
        if (completionRecorded)
            return;

        completionRecorded = true;

        if (stopwatch != null)
        {
            finalTimeValue = stopwatch.GetElapsedTime();
            finalFormattedTime = stopwatch.GetFormattedTime();
            stopwatch.StopTimer();

            Debug.Log("[PHOTO COMPLETE] Final Time Value: " + finalTimeValue);
            Debug.Log("[PHOTO COMPLETE] Final Formatted Time: " + finalFormattedTime);
        }

        Debug.Log("Objective unlocked!");

        // 🔊 play voice after delay
        StartCoroutine(PlayCompletionVoiceAfterDelay());
    }

    IEnumerator PlayCompletionVoiceAfterDelay()
    {
        yield return new WaitForSeconds(voiceDelay);

        if (audioSource != null && completionVoice != null)
        {
            audioSource.PlayOneShot(completionVoice);
        }
    }

    public void ResetPhotos()
    {
        currentPhotos = 0;
        completionRecorded = false;
        finalTimeValue = 0f;
        finalFormattedTime = "";
        UpdateDisplay();
    }
}