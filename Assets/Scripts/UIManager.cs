using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Références")]
    public GameBehaviour gameManager;

    [Header("UI Elements")]
    public TMP_Text bigMessage;   // En plein centre
    public TMP_Text hudInfo;      // Petit texte permanent
    public TMP_Text scoreText;
    public GameObject restartButton;

    [Header("Cards UI")]
    public GameObject attackCardIcon;
    public GameObject healCardIcon;
    public GameObject evolutionCardIcon;


    private Coroutine messageRoutine;
    public void OnRestartClicked()
    {
        gameManager.RestartGame();
    }

    public void ShowRestartButton(bool value)
    {
        restartButton.SetActive(value);
    }

    void Start()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameBehaviour>();

        UpdateHUD(0f); // Mise en place au début

        attackCardIcon.SetActive(true);
        healCardIcon.SetActive(true);
        evolutionCardIcon.SetActive(false);
 

    }

    // ---------- BIG MESSAGE ----------
    public void ShowBigMessage(string text, float duration = 1.5f, bool pauseGame = true)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(MessageRoutine(text, duration, pauseGame));
    }

    public void UnlockEvolutionCard()
    {
        if (evolutionCardIcon.activeSelf)
            return;

        evolutionCardIcon.SetActive(true);
        ShowBigMessage("Nouvelle carte débloquée : Évolution !", 2f, false);
    }


    private IEnumerator MessageRoutine(string text, float duration, bool pauseGame)
    {
        if (pauseGame)
            gameManager.PauseGame(true);

        bigMessage.text = text;
        bigMessage.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        bigMessage.gameObject.SetActive(false);

        if (pauseGame)
            gameManager.PauseGame(false);
    }
     

    // ---------- HUD PERMANENT ----------
    public void UpdateHUD(float timer)
    {
        string joueur =
            (gameManager.currentTurn == GameBehaviour.TurnState.Player1)
            ? "Joueur 1"
            : "Joueur 2";

        int seconds = Mathf.CeilToInt(timer);

        hudInfo.text = $"Round : {gameManager.round}\nTour : {joueur}\nTemps : {seconds}s";
    }

    public void UpdateScore(int scoreP1, int scoreP2)
    {
        scoreText.text = $"Score \n{scoreP1} - {scoreP2}";
    }

}
