using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class EndScore : MonoBehaviour
{
    public TextMeshProUGUI TotalScore, photo1, photo2, photo3, rating1, rating2, rating3, timed;
    public GameObject camera, timer;
    CameraRaycast CameraComp;
    VRStopwatch TimerComp;
    List<string> ratings;
    List<int> photos;
    int value = 0;
    float timeelap = 0f;
    void Start()
    {
        CameraComp = camera.GetComponent<CameraRaycast>();
        TimerComp = timer.GetComponent<VRStopwatch>();
        TotalScore.text = "0";
    }

    void Update()
    {
        value = CameraComp.totalScore;
        ratings = CameraComp.photoRatings;
        photos = CameraComp.photoScores;

        timeelap = TimerComp.elapsedTime;

        timed.text = timeelap.ToString();

        TotalScore.text = value.ToString();

        if(photos.Count != 0)
        {
            photo1.text = photos[0].ToString();
            if(photos.Count >= 2)
            photo2.text = photos[1].ToString();
            if(photos.Count >= 3)
            photo3.text = photos[2].ToString();
            rating1.text = ratings[0];
            if(ratings.Count >= 2)
            rating2.text = ratings[1];
            if(ratings.Count >= 3)
            rating3.text = ratings[2];
        }
    }

}