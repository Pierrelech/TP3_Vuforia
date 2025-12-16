using System.Collections;
using UnityEngine;

public class HeroScript : MonoBehaviour
{
    [Header("Stats")]
    public int max_pv = 1000;
    public int attack_base = 100;
    public int pv = 900;
    public int lvl = 0;
    public HeroScript opponentA;
    public HeroScript opponentB;

    [Header("Animation")]
    public Animator animator;

    [Header("Visuel PV")]
    public Renderer heroRenderer;

    [Header("Distance")]
    public float fightDistance = 0.32f;

    [Header("Attaques")]
    // Doit matcher attackSounds en ordre
    private string[] attackTriggers =
    {
        "PunchTrigger",
        "KickTrigger",
        "HurricaneTrigger"
    };

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip[] attackSounds;   // même ordre que attackTriggers
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip healSound;
    public AudioClip buffSound;

    [Header("Hurricane")]
    public float hurricaneDuration = 2.0f; // durée approximative de l'anim

    private Vector3 hurricaneStartPos;
    private Quaternion hurricaneStartRot;

    private HeroScript GetActiveOpponent()
    {
        // 1) priorité : celui qui est tracké (Vuforia)
        if (IsOpponentValid(opponentA)) return opponentA;
        if (IsOpponentValid(opponentB)) return opponentB;

        return null;
    }

    private bool IsOpponentValid(HeroScript h)
    {
        if (h == null) return false;

        // Fallback simple : objet actif dans la hiérarchie
        if (!h.gameObject.activeInHierarchy) return false;

#if VUFORIA_PRESENT
        // Si Vuforia est là : on vérifie que la cible est réellement trackée
        var observer = h.GetComponentInParent<ObserverBehaviour>();
        if (observer != null)
        {
            var s = observer.TargetStatus.Status;
            bool tracked = (s == Status.TRACKED || s == Status.EXTENDED_TRACKED);
            if (!tracked) return false;
        }
#endif

        return true;
    }

    void Start()
    {
        pv = Mathf.Clamp(pv, 0, max_pv);

        // Activer uniquement le skin courant
      
            // fallback si tu n'utilises pas encore le système d'évolution
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        

        if (animator != null)
            animator.applyRootMotion = true;
    }



    public void Attack()
    {
        var currentOpponent = GetActiveOpponent();
        // Sécurité de base
        if (currentOpponent == null || animator == null)
            return;

        if (currentOpponent.animator == null)
            return;

        // Choix de l'attaque
        int maxIndex = (lvl >= attackTriggers.Length) ? attackTriggers.Length : (lvl + 1);
        int action = Random.Range(0, maxIndex);
        string selectedAttack = attackTriggers[action];

        // Dégâts
        currentOpponent.pv -= attack_base;

        // Son d'attaque (optionnel)
        if (audioSource != null &&
            attackSounds != null &&
            attackSounds.Length > action &&
            attackSounds[action] != null)
        {
            audioSource.PlayOneShot(attackSounds[action]);
        }

        // Cas spécial : Hurricane → on mémorise la position / rotation de départ
        if (selectedAttack == "HurricaneTrigger")
        {
            hurricaneStartPos = transform.position;
            hurricaneStartRot = transform.rotation;

            // (Optionnel) on tourne vers l'adversaire
            Vector3 toEnemy = currentOpponent.transform.position - transform.position;
            toEnemy.y = 0;
            if (toEnemy.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(toEnemy);

            StartCoroutine(RestoreAfterHurricane(hurricaneDuration));
        }

        // Animation d'attaque
        animator.SetTrigger(selectedAttack);

        // Réaction de l'adversaire
        if (currentOpponent.pv > 0)
        {
            currentOpponent.animator.SetTrigger("HittedTrigger");

            if (currentOpponent.audioSource != null && hurtSound != null)
                currentOpponent.audioSource.PlayOneShot(hurtSound);
        }
        else
        {
            currentOpponent.animator.SetTrigger("DeathTrigger");

            if (currentOpponent.audioSource != null && deathSound != null)
                currentOpponent.audioSource.PlayOneShot(deathSound);
        }

        // Remise en place locale (si le héros est enfant d'un autre objet)
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        IncrementLVL();
        currentOpponent.UpdateColor();
    }

    // Appelée par la carte Vuforia d'évolution

    private IEnumerator PlayEvolutionSequence()
    {
        // Lance l’anim
        animator.SetTrigger("Evolution");

        // On attend la fin de l’image pour que Unity mette à jour l’anim
        yield return null;

        // On récupère la durée de l’animation actuelle
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // Tant que l'animation n'est pas finie
        while (state.normalizedTime < 1f || animator.IsInTransition(0))
        {
            yield return null;
            state = animator.GetCurrentAnimatorStateInfo(0);
        }

        // 👉 Ici l’animation est TERMINÉE
        Debug.Log("Animation d'évolution terminée !");

        // 🔥 Maintenant tu mets ce que tu veux faire APRÈS l’évolution :
        // Exemple :
        // ActivateNextModel();
        // stats upgrade, effets, VFX, changement de prefab…
    }

    public bool TryEvolutionFromCard(int requiredLevel)
    {
        if (lvl < requiredLevel)
        {
            Debug.Log("Niveau insuffisant pour évoluer !");
            return false;
        }

        StartCoroutine(PlayEvolutionSequence());           // déclenche l'évolution normale
        
        return true;
    }

    private IEnumerator RestoreAfterHurricane(float delay)
    {
        // On laisse l'anim se jouer entièrement
        yield return new WaitForSeconds(delay);

        // On ramène le héros à son point de départ
        transform.position = hurricaneStartPos;
        transform.rotation = hurricaneStartRot;
    }

    public void Buff()
    {
        attack_base += 50;

        if (audioSource != null && buffSound != null)
            audioSource.PlayOneShot(buffSound);
    }

    public void Heal(int healAmount)
    {
        if (animator != null)
            animator.SetTrigger("HealTrigger");

        pv += healAmount;
        pv = Mathf.Clamp(pv, 0, max_pv);
        UpdateColor();

        if (audioSource != null && healSound != null)
            audioSource.PlayOneShot(healSound);
    }

    public void UpdateColor()
    {
        if (heroRenderer == null) return;

        float ratio = (float)pv / max_pv;
        Color c;

        if (ratio <= 0.2f)
            c = Color.black;       // mort
        else if (ratio >= 0.9f)
            c = Color.green;       // full
        else if (ratio >= 0.4f)
            c = Color.yellow;      // milieu
        else
            c = Color.red;         // low HP

        heroRenderer.material.color = c;
    }

    void IncrementLVL()
    {
        lvl++;
    }

    public void Evolution()
    {
        
        if (animator != null)
            animator.SetTrigger("Evolution");

        
    }

    void Update()
    {
        var currentOpponent = GetActiveOpponent();
        if (animator == null)
            return;

        if (currentOpponent == null)
        {
            animator.SetBool("Fight", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, currentOpponent.transform.position);
        bool isCloseEnough = distance <= fightDistance;

        animator.SetBool("Fight", isCloseEnough);
    public void ResetHero(bool hardReset=false)
    {
        pv = max_pv;
        UpdateColor();
        if (hardReset)
        {
            attack_base = 100;
            max_pv = 1000;
            lvl = 0;
        }
    }
}
