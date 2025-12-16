using System.Collections;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public enum TurnState { Player1, Player2 }
    public UIManager ui;

    [Header("Héros")]
    public HeroScript player1;
    public HeroScript player2;

    [Header("Paramètres de jeu")]
    public int round = 1;
    public TurnState currentTurn = TurnState.Player1;
    public float turnDuration = 60f;

    private bool secondaryActionUsed = false; // Heal ou Buff
    private bool attackUsed = false;          // Attaque principale

    private float timer = 0f;
    private bool gamePaused = false;

    public bool IsPaused => gamePaused;


    void Start()
    {
        ui = FindFirstObjectByType<UIManager>();
        StartGame();
    }

    // ------------------ DEBUT DE PARTIE ------------------

    public void StartGame()
    {
        round = 1;

        player1.ResetHero(true);
        player2.ResetHero(true);

        player1.currentOpponent = player2;
        player2.currentOpponent = player1;

        ui.ShowBigMessage("Début de la partie !");
        ui.UpdateHUD(timer);

        StartRound();
    }

    public void PauseGame(bool value)
    {
        gamePaused = value;
        Debug.Log("Game paused = " + value);
    }


    // ------------------ ROUNDS ------------------

    void StartRound()
    {
        ui.ShowBigMessage($"Round {round} !", 1.5f, true);
        StartCoroutine(StartTurnAfterDelay(TurnState.Player1, 1.5f));
    }

    private IEnumerator StartTurnAfterDelay(TurnState turn, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn(turn);
    }


    // ------------------ TOURS ------------------

    void StartTurn(TurnState turn)
    {
        currentTurn = turn;

        // Reset des actions du tour
        secondaryActionUsed = false;
        attackUsed = false;

        timer = turnDuration;

        ui.ShowBigMessage(
            $"Tour du {(turn == TurnState.Player1 ? "Joueur 1" : "Joueur 2")}"
        );
        ui.UpdateHUD(timer);

    }

    // ------------------ ACTIONS : ATTAQUE ------------------

    public bool TryAttackCurrentPlayer()
    {
        if (gamePaused)
        {
            Debug.Log("Le jeu est en pause, action secondaire impossible.");
            return false;
        }
        HeroScript currentHero =
            (currentTurn == TurnState.Player1) ? player1 : player2;

        if (attackUsed)
        {
            Debug.Log("Attaque déjà utilisée !");
            return false;
        }

        // Le joueur attaque
        currentHero.Attack();
        attackUsed = true;

        EndTurn();
        return true;
    }

    // ------------------ ACTIONS : HEAL / BUFF ------------------

    public bool TryUseSecondaryAction(System.Action<HeroScript> action)
    {
        if (gamePaused) {
            Debug.Log("Le jeu est en pause, action secondaire impossible.");
            return false;
        }

        HeroScript currentHero =
            (currentTurn == TurnState.Player1) ? player1 : player2;

        if (secondaryActionUsed)
        {
            Debug.Log("Action secondaire déjà utilisée ce tour !");
            return false;
        }

        if (attackUsed)
        {
            Debug.Log("Impossible d'utiliser Heal ou Buff après l'attaque !");
            return false;
        }

        action(currentHero);
        secondaryActionUsed = true;

        return true;
    }

    // ------------------ FIN DU TOUR ------------------

    void EndTurn()
    {
        if (currentTurn == TurnState.Player1)
        {
            StartTurn(TurnState.Player2);
        }
        else
        {
            round++;
            StartRound();
        }
    }

    // ------------------ TIMER ------------------

    void Update()
    {
        if (gamePaused) return;  // ⛔ on stoppe tout

        timer -= Time.deltaTime;
        ui.UpdateHUD(timer);

        if (timer <= 0 && !attackUsed)
        {
            Debug.Log("Temps écoulé → Fin automatique du tour");
            EndTurn();
        }
    }

}
