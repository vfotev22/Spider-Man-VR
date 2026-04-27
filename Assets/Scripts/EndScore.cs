using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EndScore : MonoBehaviour
{
    public TextMeshProUGUI TotalScore, photo1, photo2, photo3, rating1, rating2, rating3, timed;
    public GameObject timer;

    VRStopwatch TimerComp;

    float timeelap = 0f;

    void Start()
    {
        TimerComp = timer.GetComponent<VRStopwatch>();
        TotalScore.text = "0";
    }

    void Update()
    {
        int value = CameraRaycast.totalScore;
        List<string> ratings = CameraRaycast.photoRatings;
        List<int> photos = CameraRaycast.photoScores;

        timeelap = TimerComp.elapsedTime;
        timed.text = timeelap.ToString();

        TotalScore.text = value.ToString();

        if (photos.Count > 0)
        {
            photo1.text = photos[0].ToString();
            rating1.text = ratings[0];
        }

        if (photos.Count > 1)
        {
            photo2.text = photos[1].ToString();
            rating2.text = ratings[1];
        }

        if (photos.Count > 2)
        {
            photo3.text = photos[2].ToString();
            rating3.text = ratings[2];
        }
    }
}