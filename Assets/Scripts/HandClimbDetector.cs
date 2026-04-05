using UnityEngine;
using System.Collections.Generic;

public class HandClimbDetector : MonoBehaviour
{
    public LayerMask climbableLayer;

    public bool IsTouchingClimbable => overlappingClimbables.Count > 0;
    public Collider CurrentWall { get; private set; }

    private readonly HashSet<Collider> overlappingClimbables = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & climbableLayer) == 0)
            return;

        overlappingClimbables.Add(other);
        CurrentWall = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!overlappingClimbables.Contains(other))
            return;

        overlappingClimbables.Remove(other);

        if (CurrentWall == other)
            CurrentWall = null;
    }
}