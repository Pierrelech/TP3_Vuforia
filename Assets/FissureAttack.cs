using UnityEngine;
using Vuforia;

public class FissureAttack : MonoBehaviour
{
    public float detectionRange = 0.5f;

    private bool isFissureVisible = false;          // la carte Fissure est-elle vue ?
    private bool hasAttackedWhileVisible = false;   // attaque déjà faite pour cette "session" de visibilité

    // ✳️ À connecter dans l'Inspector → DefaultObserverEventHandler / On Target Found
    public void OnAttackFound()
    {
        Debug.Log("Fissure found");
        isFissureVisible = true;
        hasAttackedWhileVisible = false; // on autorise une nouvelle attaque
    }

    // ✳️ À connecter dans l'Inspector → DefaultObserverEventHandler / On Target Lost
    public void OnAttackLost()
    {
        Debug.Log("Fissure lost");
        isFissureVisible = false;
        hasAttackedWhileVisible = false;
    }

    void Update()
    {
        // Si la carte n'est pas visible → on ne fait rien
        if (!isFissureVisible)
            return;

        // Si on a déjà attaqué pendant cette phase de visibilité → on arrête
        if (hasAttackedWhileVisible)
            return;

        // On essaie de faire attaquer le héros le plus proche
        bool attacked = MakeNearestHeroAttackOnce();

        if (attacked)
        {
            hasAttackedWhileVisible = true; // on bloque les attaques suivantes tant que la carte reste visible
        }
    }

    private bool MakeNearestHeroAttackOnce()
    {
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");

        HeroScript nearestHero = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject heroObject in heroes)
        {
            HeroScript hero = heroObject.GetComponent<HeroScript>();
            if (hero == null) continue;

            // On vérifie que la carte du héros est bien TRACKED
            ObserverBehaviour heroTracker = heroObject.GetComponent<ObserverBehaviour>();
            if (heroTracker != null)
            {
                var heroStatus = heroTracker.TargetStatus.Status;
                if (heroStatus != Status.TRACKED &&
                    heroStatus != Status.EXTENDED_TRACKED &&
                    heroStatus != Status.LIMITED)
                {
                    continue; // pas encore vraiment détecté
                }
            }

            float distance = Vector3.Distance(transform.position, heroObject.transform.position);

            if (distance <= detectionRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestHero = hero;
            }
        }

        if (nearestHero != null)
        {
            Debug.Log(nearestHero.name + " lance l'attaque Fissure sur son adversaire !");
            nearestHero.Attack();   // 💥 c’est LUI qui attaque son currentOpponent
            return true;
        }

        // Aucun héros prêt pour l’instant → on retentera dans le prochain Update
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
