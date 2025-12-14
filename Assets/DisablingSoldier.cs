using UnityEngine;

public class DisablingSoldier : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Example (attach this script to a Game Manager or a relevant controller)
    public GameObject cybersoldier; // Drag the Cybersoldier object here in the Inspector

    void Start()
    {
        // Force the object to be disabled on start
        cybersoldier.SetActive(false);
    }
}
