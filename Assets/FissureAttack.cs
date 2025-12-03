using UnityEngine;
using Vuforia;

public class FissureAttack : MonoBehaviour
{
    private bool isCardVisible = false;
    private bool actionTriggeredThisVisibility = false;

    private GameBehaviour gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameBehaviour>();
    }

    // Appelé par Vuforia → On Target Found
    public void OnAttackFound()
    {
        Debug.Log("Fissure card detected");
        isCardVisible = true;
        actionTriggeredThisVisibility = false;
        TryTriggerAttack();
    }

    // Appelé par Vuforia → On Target Lost
    public void OnAttackLost()
    {
        Debug.Log("Fissure card lost");
        isCardVisible = false;
        actionTriggeredThisVisibility = false;
    }

    private void TryTriggerAttack()
    {
        if (!isCardVisible) return;
        if (actionTriggeredThisVisibility) return;

        // Demande au GameManager de jouer l'action d'attaque du tour
        bool attackDone = gameManager.TryAttackCurrentPlayer();

        if (attackDone)
        {
            actionTriggeredThisVisibility = true;
            Debug.Log("Attack triggered by fissure card");
        }
        else
        {
            Debug.Log("Attack NOT triggered (either not this player's turn or game paused)");
        }
    }
}
