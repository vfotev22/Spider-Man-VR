using UnityEngine;
using System.Collections.Generic;

public class HandCameraDetector : MonoBehaviour
{
    public string cameraTag = "Camera";

    public bool IsTouchingCamera => overlappingCameras.Count > 0;
    public Collider CurrentCamera { get; private set; }

    private readonly HashSet<Collider> overlappingCameras = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(cameraTag))
            return;

        overlappingCameras.Add(other);
        CurrentCamera = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!overlappingCameras.Contains(other))
            return;

        overlappingCameras.Remove(other);

        if (CurrentCamera == other)
            CurrentCamera = GetAnyCamera();
    }

    private Collider GetAnyCamera()
    {
        foreach (Collider col in overlappingCameras)
            return col;

        return null;
    }
}