using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraRaycast : MonoBehaviour
{
    public PhotoCounter photoCounter;
    public Transform rayOrigin;
    public InputActionProperty triggerAction;
    public CameraGrab cameraGrab;

    public float rayDistance = 100f;

    public AudioSource audioSource;
    public AudioClip cameraShotSound;

    public AudioClip amazingVoice;
    public AudioClip greatVoice;
    public AudioClip goodVoice;

    public List<string> photoRatings = new List<string>();
    public List<int> photoScores = new List<int>();
    public int totalScore = 0;

    void Update()
    {
        if (cameraGrab == null || !cameraGrab.IsHoldingCamera)
            return;

        if (triggerAction.action.WasPressedThisFrame())
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        if (rayOrigin == null)
            return;

        Vector3 direction = rayOrigin.forward;
        Debug.DrawRay(rayOrigin.position, direction * rayDistance, Color.green, 2f);

        if (Physics.Raycast(rayOrigin.position, direction, out RaycastHit hit, rayDistance))
        {
            string tag = hit.collider.tag;

            Debug.Log("Hit: " + hit.collider.name);

            if (tag.StartsWith("Amazing") || tag.StartsWith("Great") || tag.StartsWith("Good"))
            {
                if (audioSource != null && cameraShotSound != null)
                    audioSource.PlayOneShot(cameraShotSound);

                char groupNumber = tag[tag.Length - 1];

                string rating = "";
                int score = 0;

                if (tag.StartsWith("Amazing"))
                {
                    rating = "Amazing";
                    score = 3;
                    photoCounter.AddPhoto();

                    if (audioSource != null && amazingVoice != null)
                        audioSource.PlayOneShot(amazingVoice);
                }
                else if (tag.StartsWith("Great"))
                {
                    rating = "Great";
                    score = 2;
                    photoCounter.AddPhoto();

                    if (audioSource != null && greatVoice != null)
                        audioSource.PlayOneShot(greatVoice);
                }
                else if (tag.StartsWith("Good"))
                {
                    rating = "Good";
                    score = 1;
                    photoCounter.AddPhoto();

                    if (audioSource != null && goodVoice != null)
                        audioSource.PlayOneShot(goodVoice);
                }

                photoRatings.Add(rating);
                photoScores.Add(score);
                totalScore += score;

                Debug.Log($"[PHOTO STORED] Hit: {tag}");
                Debug.Log($"[PHOTO STORED] Rating: {rating} | Score: {score}");
                Debug.Log($"[PHOTO STORED] Group: {groupNumber}");
                Debug.Log($"[TOTAL SCORE] {totalScore}");

                DestroyScoreGroup(groupNumber);
            }
            else
            {
                Debug.Log("[PHOTO RESULT] Miss — nothing stored");
            }
        }
        else
        {
            Debug.Log("[PHOTO RESULT] Miss — nothing stored");
        }
    }

    void DestroyScoreGroup(char groupNumber)
    {
        string[] prefixes = { "Amazing", "Great", "Good" };

        Debug.Log($"[DESTROY] Removing score group {groupNumber}");

        foreach (string prefix in prefixes)
        {
            string fullTag = prefix + groupNumber;
            GameObject[] objects = GameObject.FindGameObjectsWithTag(fullTag);

            foreach (GameObject obj in objects)
            {
                Destroy(obj);
                Debug.Log($"[DESTROY] Removed {obj.name}");
            }
        }

        string markerTag = "ObjectiveMarker" + groupNumber;
        GameObject[] markers = GameObject.FindGameObjectsWithTag(markerTag);

        foreach (GameObject marker in markers)
        {
            Destroy(marker);
            Debug.Log($"[DESTROY] Removed marker {marker.name} with tag {markerTag}");
        }
    }
}