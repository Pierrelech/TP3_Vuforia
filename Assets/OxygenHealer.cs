using UnityEngine;
using Vuforia;

public class OxygenHealer : MonoBehaviour
{
    public float detectionRange = 0.5f;
    public int healAmount = 100;

    private bool isOxygenVisible = false;      // Oxygen est-elle vue ?
    private bool hasHealedWhileVisible = false; // Heal déjà fait pour cette "session" de visibilité

    // ✳️ A connecter dans l'Inspector → DefaultObserverEventHandler / On Target Found
    public void OnOxygenFound()
    {
        Debug.Log("Oxygen found");
        isOxygenVisible = true;
        hasHealedWhileVisible = false; // on autorise un nouveau heal
    }

    // ✳️ A connecter dans l'Inspector → DefaultObserverEventHandler / On Target Lost
    public void OnOxygenLost()
    {
        Debug.Log("Oxygen lost");
        isOxygenVisible = false;
        hasHealedWhileVisible = false;
    }

    void Update()
    {
        // Si Oxygen n'est pas visible → on ne fait rien
        if (!isOxygenVisible)
            return;

        // Si on a déjà heal pendant cette phase de visibilité → on ne refait rien
        if (hasHealedWhileVisible)
            return;

        // On essaie de soigner le héros le plus proche
        bool healed = HealNearestHeroOnce();

        if (healed)
        {
            hasHealedWhileVisible = true; // on bloque les heals suivants tant qu'Oxygen reste visible
        }
    }

    private bool HealNearestHeroOnce()
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
            Debug.Log(nearestHero.name + " est soigné par l'oxygène (héros le plus proche) !");
            nearestHero.Heal(healAmount);
            return true;
        }

        // Aucun héros prêt pour l’instant → on retentera dans le prochain Update
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
