using System.Collections;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public enum TurnState { Player1, Player2 }

    [Header("Références")]
    public HeroScript player1;
    public HeroScript player2;
    public UIManager ui;

    [Header("Gameplay")]
    public float turnDuration = 60f;
    public int evolutionLevel = 1;

    [Header("Distances")]
    public float startFightDistance = 0.35f;
    public float fightDistance = 0.35f;

    public TurnState currentTurn = TurnState.Player1;
    public int round = 1;


    [Header("Match Settings")]
    public int maxMatchesWon = 3;

    private int player1Score = 0;
    private int player2Score = 0;

    private bool matchOver = false;

    private bool cardUsedThisTurn;
    private bool gamePaused;
    private bool matchStarted;
    private float timer;



    public bool IsPaused => gamePaused;

    // ---------------- INIT ----------------

    void Start()
    {
        ui = FindFirstObjectByType<UIManager>();

        PauseGame(true);
        matchStarted = false;

        ui.ShowBigMessage("Approchez les héros pour commencer", 2f, false);
    }

    // ---------------- UPDATE ----------------

    void Update()
    {
        // 🔥 Toujours gérer Fight par la distance
        UpdateFightByDistance();

        // 🔥 Tant que le match n'a pas commencé, on attend la distance
        if (!matchStarted)
        {
            CheckStartDistance();
            return;
        }

        if (gamePaused)
            return;

        timer -= Time.deltaTime;
        ui.UpdateHUD(timer);

        if (timer <= 0 && !cardUsedThisTurn)
            EndTurn();
    }


    // ---------------- DISTANCE ----------------

    private void CheckStartDistance()
    {
        float distance = Vector3.Distance(
            player1.transform.position,
            player2.transform.position
        );

        if (distance <= startFightDistance)
        {
            matchStarted = true;

            // Reset complet
            player1.ResetHero(true);
            player2.ResetHero(true);

            PauseGame(false);
            StartGame();
        }
    }

    private void UpdateFightByDistance()
    {
        float distance = Vector3.Distance(
            player1.transform.position,
            player2.transform.position
        );

        bool inFightRange = distance <= startFightDistance;

        // ----- PLAYER 1 -----
        if (!player1.IsDead)
        {
            player1.SetInGuard(inFightRange);

            if (inFightRange)
                player1.FaceTarget(player2.transform.position);
        }
        else
        {
            // IMPORTANT : ne jamais forcer Fight si mort
            player1.SetInGuard(true);
        }

        // ----- PLAYER 2 -----
        if (!player2.IsDead)
        {
            player2.SetInGuard(inFightRange);

            if (inFightRange)
                player2.FaceTarget(player1.transform.position);
        }
        else
        {
            player2.SetInGuard(true);
        }
    }


    // ---------------- GAME FLOW ----------------

    private void StartGame()
    {
        round = 1;
        ui.ShowBigMessage("Début de la partie !");
        StartRound();
    }

    private void StartRound()
    {
        ui.ShowBigMessage($"Round {round} !", 1.5f, true);
        StartCoroutine(StartTurnAfterDelay(TurnState.Player1, 1.5f));
    }

    private IEnumerator StartTurnAfterDelay(TurnState turn, float delay)
    {
        PauseGame(true);
        yield return new WaitForSeconds(delay);
        PauseGame(false);
        StartTurn(turn);
    }

    private void StartTurn(TurnState turn)
    {
        currentTurn = turn;
        cardUsedThisTurn = false;
        timer = turnDuration;

        ui.ShowBigMessage(
            $"Tour du {(turn == TurnState.Player1 ? "Joueur 1" : "Joueur 2")}"
        );
    }

    private void EndTurn()
    {
        if (currentTurn == TurnState.Player1)
            StartTurn(TurnState.Player2);
        else
        {
            round++;
            StartRound();
        }
    }

    // ---------------- ACTIONS ----------------

    private bool CanUseCard()
    {
        if (matchOver)
            return false;

        // ⚠️ interdit toute action hors combat
        float distance = Vector3.Distance(
            player1.transform.position,
            player2.transform.position
        );

        bool inFight = distance <= fightDistance;

        return inFight && !gamePaused && !cardUsedThisTurn;
    }

    public bool TryAttack()
    {
        if (!CanUseCard()) return false;

        HeroScript attacker =
            (currentTurn == TurnState.Player1) ? player1 : player2;
        HeroScript target =
            (currentTurn == TurnState.Player1) ? player2 : player1;

        attacker.Attack(target);

        cardUsedThisTurn = true;

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

    public bool TryHeal(int amount)
    {
        if (!CanUseCard()) return false;

        HeroScript hero =
            (currentTurn == TurnState.Player1) ? player1 : player2;

        hero.Heal(amount);
        cardUsedThisTurn = true;
        EndTurn();
        return true;
    }

    public bool TryBuff()
    {
        if (!CanUseCard()) return false;

        HeroScript hero =
            (currentTurn == TurnState.Player1) ? player1 : player2;

        hero.Buff();
        cardUsedThisTurn = true;
        EndTurn();
        return true;
    }

    public bool TryEvolution(int requiredLevel)
    {
        if (!CanUseCard()) return false;

        HeroScript hero =
            (currentTurn == TurnState.Player1) ? player1 : player2;

        bool success = hero.TryEvolutionFromCard(requiredLevel);

        if (success)
        {
            cardUsedThisTurn = true;
            EndTurn();
        }

        return success;
    }

    // ---------------- VICTOIRE ----------------

    private void HandleVictory(HeroScript winner, HeroScript loser)
    {
        PauseGame(true);

        // Mise à jour score
        if (winner == player1)
            player1Score++;
        else
            player2Score++;

        ui.UpdateScore(player1Score, player2Score);

        // Level up du vainqueur
        winner.GainLevel(1);

        if (winner.lvl >= evolutionLevel)
            ui.UnlockEvolutionCard();

        // 🔥 Laisser la mort se jouer
        StartCoroutine(HandleDeathAndNextRound(winner, loser));
    }

    private IEnumerator HandleDeathAndNextRound(HeroScript winner, HeroScript loser)
    {
        // ⏱ Temps pour laisser jouer l’animation de mort
        yield return new WaitForSeconds(3.5f);

        // Maintenant seulement → revive
        loser.Revive();

        // Vérifier fin de match
        if (player1Score >= maxMatchesWon || player2Score >= maxMatchesWon)
        {
            DeclareFinalWinner(winner);
            yield break;
        }

        ui.ShowBigMessage($"{winner.name} gagne la manche !", 2f, true);

        yield return new WaitForSeconds(2f);

        PauseGame(false);
        round++;
        StartRound();
    }


    private void DeclareFinalWinner(HeroScript winner)
    {
        matchOver = true;
        PauseGame(true);

        ui.ShowBigMessage(
            $"{winner.name} remporte le match !",
            3f,
            true
        );

        ui.ShowRestartButton(true);
    }

    public void RestartGame()
    {
        player1Score = 0;
        player2Score = 0;
        round = 1;
        matchOver = false;

        ui.UpdateScore(player1Score, player2Score);
        ui.ShowRestartButton(false);

        player1.ResetHero(true);
        player2.ResetHero(true);

        matchStarted = false;
        PauseGame(true);

        ui.ShowBigMessage("Approchez les héros pour commencer", 2f, false);
    }


    private IEnumerator StartNextRoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PauseGame(false);
        round++;
        StartRound();
    }

    // ---------------- PAUSE ----------------

    public void PauseGame(bool value)
    {
        gamePaused = value;
    }
}
