using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class markDist : MonoBehaviour
{
    public GameObject imageTracker;
    public Vector3 delta;
    public bool goIn = false;

    void Update()
    {
        // Ancienne version (commentée)
        // var trackableImage = imageTracker.GetComponent<TrackableBehaviour>();

        // Version actuelle avec Vuforia récent
        var trackableImage = imageTracker.GetComponent<ObserverBehaviour>();

        // Ancienne version (commentée)
        // var statusImage = trackableImage.CurrentStatus;

        var statusImage = trackableImage.TargetStatus.Status;

        // Ancienne condition (commentée)
        // if (statusImage == TrackableBehaviour.Status.TRACKED)

        if (statusImage == Status.TRACKED)
        {
            goIn = true;
            delta = Camera.main.transform.position - transform.position;
        }
        else
        {
            goIn = false;
        }
    }
}
