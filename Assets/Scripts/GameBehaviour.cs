using System.Collections;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public enum TurnState { Player1, Player2 }

    [Header("Références")]
    public HeroScript player1;
    public HeroScript player2;
    public UIManager ui;

    [Header("Paramètres")]
    public int round = 1;
    public TurnState currentTurn = TurnState.Player1;
    public float turnDuration = 60f;

    private bool secondaryActionUsed;
    private bool attackUsed;
    private float timer;
    private bool gamePaused;

    public bool IsPaused => gamePaused;

    void Start()
    {
        ui = FindFirstObjectByType<UIManager>();
        StartGame();
    }

    // ---------------- GAME FLOW ----------------

    public void StartGame()
    {
        round = 1;
        player1.ResetHero(true);
        player2.ResetHero(true);

        ui.ShowBigMessage("Début de la partie !");
        StartRound();
    }

    void StartRound()
    {
        ui.ShowBigMessage($"Round {round} !", 1.5f, true);
        StartCoroutine(StartTurnAfterDelay(TurnState.Player1, 1.5f));
    }

    IEnumerator StartTurnAfterDelay(TurnState turn, float delay)
    {
        PauseGame(true);
        yield return new WaitForSeconds(delay);
        PauseGame(false);
        StartTurn(turn);
    }

    void StartTurn(TurnState turn)
    {
        currentTurn = turn;
        secondaryActionUsed = false;
        attackUsed = false;
        timer = turnDuration;

        ui.ShowBigMessage($"Tour du {(turn == TurnState.Player1 ? "Joueur 1" : "Joueur 2")}");
        ui.UpdateHUD(timer);
    }

    void EndTurn()
    {
        if (currentTurn == TurnState.Player1)
            StartTurn(TurnState.Player2);
        else
        {
            round++;
            StartRound();
        }
    }

    public void PauseGame(bool value)
    {
        gamePaused = value;
    }

    // ---------------- ACTIONS ----------------

    public bool TryAttack()
    {
        if (gamePaused || attackUsed) return false;

        HeroScript attacker = (currentTurn == TurnState.Player1) ? player1 : player2;
        HeroScript target = (currentTurn == TurnState.Player1) ? player2 : player1;

        attacker.Attack(target);
        attackUsed = true;

        if (target.pv <= 0)
        {
            HandleVictory(attacker, target);
        }
        else
        {
            EndTurn();
        }

        return true;
    }

    private void HandleVictory(HeroScript winner, HeroScript loser)
    {
        Debug.Log($"{winner.name} gagne la manche !");

        // Pause le jeu
        PauseGame(true);

        // Level up du vainqueur
        winner.GainLevel(1);

        // Reset PV du perdant uniquement
        loser.pv = loser.max_pv;

        // UI
        ui.ShowBigMessage(
            $"{winner.name} gagne la manche !",
            2f,
            true
        );

        // Nouvelle manche après délai
        StartCoroutine(StartNextRoundAfterDelay(2f));
    }


    private IEnumerator StartNextRoundAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    PauseGame(false);

    round++;
    StartRound();
}


    public bool TryHeal(int amount)
    {
        if (gamePaused || secondaryActionUsed || attackUsed) return false;

        HeroScript hero = (currentTurn == TurnState.Player1) ? player1 : player2;
        hero.Heal(amount);
        secondaryActionUsed = true;
        return true;
    }

    public bool TryBuff()
    {
        if (gamePaused || secondaryActionUsed || attackUsed) return false;

        HeroScript hero = (currentTurn == TurnState.Player1) ? player1 : player2;
        hero.Buff();
        secondaryActionUsed = true;
        return true;
    }

    public bool TryEvolution(int requiredLevel)
    {
        if (gamePaused) return false;

        HeroScript hero = (currentTurn == TurnState.Player1) ? player1 : player2;
        return hero.TryEvolutionFromCard(requiredLevel);
    }

    // ---------------- TIMER ----------------

    void Update()
    {
        if (gamePaused) return;

        timer -= Time.deltaTime;
        ui.UpdateHUD(timer);

        if (timer <= 0 && !attackUsed)
            EndTurn();
    }
}
