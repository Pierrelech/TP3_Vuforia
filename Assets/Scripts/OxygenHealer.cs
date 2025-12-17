using UnityEngine;
using Vuforia;

public class OxygenHealer : MonoBehaviour
{
    [Header("Paramètres du heal")]
    public int healAmount = 100;

    private bool isCardVisible = false;
    private bool actionTriggeredThisVisibility = false;

    private GameBehaviour gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameBehaviour>();
    }

    // ---------- VUFORIA EVENTS ----------

    // À connecter à DefaultObserverEventHandler → On Target Found
    public void OnOxygenFound()
    {
        Debug.Log("Oxygen card detected");
        isCardVisible = true;
        actionTriggeredThisVisibility = false;

        TryTriggerHeal();
    }

    // À connecter à DefaultObserverEventHandler → On Target Lost
    public void OnOxygenLost()
    {
        Debug.Log("Oxygen card lost");
        isCardVisible = false;
        actionTriggeredThisVisibility = false;
    }

    // ---------- LOGIQUE ----------

    private void TryTriggerHeal()
    {
        if (!isCardVisible) return;
        if (actionTriggeredThisVisibility) return;
        if (gameManager == null) return;

        bool healDone = gameManager.TryHeal(healAmount);

        if (healDone)
        {
            actionTriggeredThisVisibility = true;
            Debug.Log("Heal triggered by Oxygen card");
        }
        else
        {
            Debug.Log("Heal NOT triggered (conditions not met)");
        }
    }
}
