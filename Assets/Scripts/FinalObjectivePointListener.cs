using UnityEngine;

public class FinalObjectivePointListener : MonoBehaviour
{
    public PhotoCounter photoCounter;

    public GameObject finalObjectiveMarker;
    public GameObject endLevelZone;

    private bool hasActivated = false;

    void Start()
    {
        if (finalObjectiveMarker != null)
            finalObjectiveMarker.SetActive(false);

        if (endLevelZone != null)
            endLevelZone.SetActive(false);
    }

    void Update()
    {
        if (hasActivated || photoCounter == null)
            return;

        if (photoCounter.CurrentPhotos >= photoCounter.PhotosNeeded)
        {
            ActivateFinalObjective();
        }
    }

    void ActivateFinalObjective()
    {
        hasActivated = true;

        if (finalObjectiveMarker != null)
            finalObjectiveMarker.SetActive(true);

        if (endLevelZone != null)
            endLevelZone.SetActive(true);

        Debug.Log("[FINAL OBJECTIVE] Activated final objective marker and end level zone");
    }
}