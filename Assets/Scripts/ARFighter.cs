using UnityEngine;

public class ARFighter : MonoBehaviour
{
    public enum Team
    {
        Hero,
        Enemy
    }

    [Header("Réglages généraux")]
    public Team team = Team.Hero;
    public float fightDistance = 0.15f;    // Distance max pour engager le combat (en mètres approx)
    public float maxFacingAngle = 30f;     // Angle max pour considérer que le perso "regarde" l'ennemi

    [Header("Animation")]
    public Animator animator;
    public string attackTriggerName = "Attack"; // Nom du trigger dans l'Animator
    public string idleStateName = "Idle";       // Nom de l'état idle (optionnel)

    private bool isFighting = false;
    private ARFighter currentOpponent = null;

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // Ici je décide que seuls les Héros initient le combat,
        // mais tu peux changer pour que les ennemis attaquent aussi.
        if (team != Team.Hero) return;

        // Si on est déjà en train de se battre, on peut décider de ne rien faire
        if (isFighting && currentOpponent != null)
            return;

        // Chercher le meilleur ennemi à attaquer
        ARFighter opponent = FindClosestOpponent();

        if (opponent != null && IsFacing(opponent.transform) && IsInFightRange(opponent.transform))
        {
            StartFight(opponent);
        }
    }

    ARFighter FindClosestOpponent()
    {
        ARFighter[] allFighters = FindObjectsOfType<ARFighter>();
        ARFighter best = null;
        float bestDist = Mathf.Infinity;

        foreach (var f in allFighters)
        {
            if (f == this) continue;
            if (f.team == this.team) continue; // on ne regarde que l'équipe adverse

            float d = Vector3.Distance(transform.position, f.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = f;
            }
        }

        return best;
    }

    bool IsInFightRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= fightDistance;
    }

    bool IsFacing(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToTarget);

        return angle <= maxFacingAngle;
    }

    void StartFight(ARFighter opponent)
    {
        isFighting = true;
        currentOpponent = opponent;

        Debug.Log($"{name} attaque {opponent.name} !");

        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        // Si tu veux aussi jouer une anim "Hit" sur l'ennemi :
        if (opponent.animator != null)
        {
            opponent.animator.SetTrigger("Hit"); // à condition d'avoir ce trigger dans son Animator
        }
    }

    // Méthode que tu pourras appeler depuis un event d'animation
    public void StopFight()
    {
        isFighting = false;
        currentOpponent = null;

        if (animator != null && !string.IsNullOrEmpty(idleStateName))
        {
            animator.Play(idleStateName);
        }
    }
}
