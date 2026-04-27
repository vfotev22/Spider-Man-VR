using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraRaycast : MonoBehaviour
{
    [Header("References")]
    public PhotoCounter photoCounter;
    public Transform rayOrigin;
    public CameraGrab cameraGrab;

    [Header("Input")]
    public InputActionProperty rightTriggerAction;
    public InputActionProperty leftTriggerAction;

    [Header("Raycast")]
    public float rayDistance = 100f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip cameraShotSound;
    public AudioClip amazingVoice;
    public AudioClip greatVoice;
    public AudioClip goodVoice;

    [Header("Shared Score")]
    public static List<string> photoRatings = new List<string>();
    public static List<int> photoScores = new List<int>();
    public static int totalScore = 0;

    void Update()
    {
        if (cameraGrab == null)
            return;

        if (cameraGrab.HeldCamera != gameObject)
            return;

        if (rightTriggerAction.action.WasPressedThisFrame() ||
            leftTriggerAction.action.WasPressedThisFrame())
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        if (rayOrigin == null || photoCounter == null)
            return;

        Vector3 direction = rayOrigin.forward;
        Debug.DrawRay(rayOrigin.position, direction * rayDistance, Color.green, 2f);

        if (!Physics.Raycast(rayOrigin.position, direction, out RaycastHit hit, rayDistance))
        {
            Debug.Log("[PHOTO RESULT] Miss — nothing stored");
            return;
        }

        string hitTag = hit.collider.tag;
        Debug.Log("Hit: " + hit.collider.name + " | Tag: " + hitTag);

        if (!hitTag.StartsWith("Amazing") &&
            !hitTag.StartsWith("Great") &&
            !hitTag.StartsWith("Good"))
        {
            Debug.Log("[PHOTO RESULT] Miss — nothing stored");
            return;
        }

        char groupNumber = hitTag[hitTag.Length - 1];

        string rating = "";
        int score = 0;

        if (hitTag.StartsWith("Amazing"))
        {
            rating = "Amazing";
            score = 3;
            PlayVoice(amazingVoice);
        }
        else if (hitTag.StartsWith("Great"))
        {
            rating = "Great";
            score = 2;
            PlayVoice(greatVoice);
        }
        else if (hitTag.StartsWith("Good"))
        {
            rating = "Good";
            score = 1;
            PlayVoice(goodVoice);
        }

        PlayVoice(cameraShotSound);

        StorePhoto(rating, score);
        photoCounter.AddPhoto();

        Debug.Log($"[PHOTO STORED] Hit: {hitTag}");
        Debug.Log($"[PHOTO STORED] Rating: {rating} | Score: {score}");
        Debug.Log($"[PHOTO STORED] Group: {groupNumber}");
        Debug.Log($"[TOTAL SCORE] {totalScore}");

        DestroyScoreGroup(groupNumber);
    }

    void StorePhoto(string rating, int score)
    {
        photoRatings.Add(rating);
        photoScores.Add(score);
        totalScore += score;
    }

    void PlayVoice(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void DestroyScoreGroup(char groupNumber)
    {
        string[] prefixes = { "Amazing", "Great", "Good" };

        foreach (string prefix in prefixes)
        {
            string fullTag = prefix + groupNumber;
            GameObject[] objects = GameObject.FindGameObjectsWithTag(fullTag);

            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }

        string markerTag = "ObjectiveMarker" + groupNumber;
        GameObject[] markers = GameObject.FindGameObjectsWithTag(markerTag);

        foreach (GameObject marker in markers)
        {
            Destroy(marker);
        }
    }

    public static void ResetScore()
    {
        photoRatings.Clear();
        photoScores.Clear();
        totalScore = 0;
    }
}