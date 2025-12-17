using UnityEngine;
using Vuforia;

public class EvolveFighter : MonoBehaviour
{
    [Header("Conditions d'évolution")]
    public int requiredLevel = 1; // Niveau minimum requis pour évoluer

    private bool isCardVisible = false;
    private bool actionTriggeredThisVisibility = false;

    private GameBehaviour gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameBehaviour>();
    }

    // ---------- VUFORIA EVENTS ----------

    // À connecter à OnTargetFound
    public void OnEvolutionFound()
    {
        Debug.Log("Evolution card detected");
        isCardVisible = true;
        actionTriggeredThisVisibility = false;

        TryTriggerEvolution();
    }

    // À connecter à OnTargetLost
    public void OnEvolutionLost()
    {
        Debug.Log("Evolution card lost");
        isCardVisible = false;
        actionTriggeredThisVisibility = false;
    }

    // ---------- LOGIQUE ----------

    private void TryTriggerEvolution()
    {
        if (!isCardVisible) return;
        if (actionTriggeredThisVisibility) return;
        if (gameManager == null) return;

        bool evolutionDone = gameManager.TryEvolution(requiredLevel);

        if (evolutionDone)
        {
            actionTriggeredThisVisibility = true;
            Debug.Log("Evolution triggered by card");
        }
        else
        {
            Debug.Log("Evolution NOT triggered (conditions not met)");
        }
    }
}
