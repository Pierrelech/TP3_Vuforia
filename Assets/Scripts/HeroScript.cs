using System.Collections;
using UnityEngine;

[System.Serializable]
public class HeroEvolutionData
{
    [Header("Conteneur de l'évolution")]
    public GameObject evolutionRoot;   // EMPTY Evo_0, Evo_1, etc.

    [Header("Stats de l'évolution")]
    public int maxPV = 1000;
    public int attackBase = 100;
}

public class HeroScript : MonoBehaviour
{
    [Header("Stats actuelles")]
    public int pv;
    public int max_pv;
    public int attack_base;
    public int lvl = 0;

    [Header("Évolutions")]
    public HeroEvolutionData[] evolutions;

    private int currentEvolutionIndex = 0;
    private bool isEvolving = false;

    // Références dynamiques vers l'évolution active
    private Animator currentAnimator;
    private AudioSource currentAudio;

    public Animator Animator => currentAnimator;
    public bool IsEvolving => isEvolving;

    // ---------------- INITIALISATION ----------------

    void Start()
    {
        ApplyEvolution(0, instant: true);
    }

    // ---------------- ATTAQUE ----------------

    public void Attack(HeroScript target)
    {
        if (isEvolving || target == null) return;

        target.pv -= attack_base;

        currentAnimator.SetTrigger("PunchTrigger");
        target.Animator.SetTrigger("HittedTrigger");

        target.ClampPV();
    }

    // ---------------- HEAL / BUFF ----------------

    public void Heal(int amount)
    {
        pv = Mathf.Clamp(pv + amount, 0, max_pv);
        currentAnimator.SetTrigger("HealTrigger");
    }

    public void Buff()
    {
        attack_base += 50;
    }

    // ---------------- ÉVOLUTION ----------------

    public bool TryEvolutionFromCard(int requiredLevel)
    {
        if (isEvolving) return false;
        if (lvl < requiredLevel) return false;
        if (currentEvolutionIndex >= evolutions.Length - 1) return false;

        StartCoroutine(EvolutionRoutine());
        return true;
    }

    private IEnumerator EvolutionRoutine()
    {
        isEvolving = true;

        currentAnimator.SetTrigger("Evolution");

        yield return null;
        AnimatorStateInfo state = currentAnimator.GetCurrentAnimatorStateInfo(0);

        while (state.normalizedTime < 1f || currentAnimator.IsInTransition(0))
            yield return null;

        lvl++;
        ApplyEvolution(currentEvolutionIndex + 1, instant: false);

        isEvolving = false;
    }

    // ---------------- APPLICATION ÉVOLUTION ----------------

    private void ApplyEvolution(int index, bool instant)
    {
        // Désactiver toutes les évolutions
        foreach (var evo in evolutions)
            evo.evolutionRoot.SetActive(false);

        // Activer la nouvelle
        HeroEvolutionData evoData = evolutions[index];
        evoData.evolutionRoot.SetActive(true);

        currentEvolutionIndex = index;

        // Récupérer Animator & AudioSource SUR L'EMPTY
        currentAnimator = evoData.evolutionRoot.GetComponent<Animator>();
        currentAudio = evoData.evolutionRoot.GetComponent<AudioSource>();

        // Mise à jour des stats
        max_pv = evoData.maxPV;
        attack_base = evoData.attackBase;

        // Gestion PV
        if (instant)
        {
            pv = max_pv;
        }
        else
        {
            float ratio = (float)pv / max_pv;
            pv = Mathf.RoundToInt(ratio * max_pv);
        }

        ClampPV();
    }

    private void ClampPV()
    {
        pv = Mathf.Clamp(pv, 0, max_pv);
    }

    // ---------------- RESET ----------------

    public void ResetHero(bool hardReset)
    {
        lvl = 0;
        ApplyEvolution(0, instant: true);
    }

    public void GainLevel(int amount)
    {
        lvl += amount;
    }
}
