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

    private Coroutine messageRoutine;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameBehaviour>();

        UpdateHUD(0f); // Mise en place au début
    }

    // ---------- BIG MESSAGE ----------
    public void ShowBigMessage(string text, float duration = 1.5f, bool pauseGame = true)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(MessageRoutine(text, duration, pauseGame));
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
}
