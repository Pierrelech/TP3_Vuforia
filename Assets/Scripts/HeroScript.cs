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

    [Header("Animator params")]
    public string fightBool = "Fight";

    public string punchTrigger = "PunchTrigger";
    public string kickTrigger = "KickTrigger";
    public string hurricaneTrigger = "HurricaneTrigger";

    public string hitTrigger = "HittedTrigger";
    public string healTrigger = "HealTrigger";
    public string deathTrigger = "DeathTrigger";
    public string evolutionTrigger = "EvolutionTrigger"; // À AJOUTER
    public bool IsDead => pv <= 0;


    [Header("Audio")]
    public AudioClip punchSound;
    public AudioClip kickSound;
    public AudioClip hurricaneSound;
    public AudioClip hitSound;
    public AudioClip healSound;
    public AudioClip evolveSound;
    public AudioClip deathSound;


    private int currentEvolutionIndex = 0;
    private bool isEvolving = false;
    private Transform currentVisualRoot;


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

    private void PlaySound(AudioClip clip)
    {
        if (currentAudio == null || clip == null)
            return;

        currentAudio.PlayOneShot(clip);
    }


    public void Attack(HeroScript target)
    {
        if (isEvolving || target == null) return;

        target.pv -= attack_base;

        // Choix attaque selon le niveau
        int maxAttack = Mathf.Min(lvl + 1, 3);
        int attackIndex = Random.Range(0, maxAttack);

        string trigger;
        AudioClip attackSound;

        switch (attackIndex)
        {
            case 0:
                trigger = punchTrigger;
                attackSound = punchSound;
                break;
            case 1:
                trigger = kickTrigger;
                attackSound = kickSound;
                break;
            default:
                trigger = hurricaneTrigger;
                attackSound = hurricaneSound;
                break;
        }

        // Animation + son attaque
        if (currentAnimator)
            currentAnimator.SetTrigger(trigger);



        PlaySound(attackSound);

        // 🔥 COUP FATAL ?
        if (target.pv <= 0)
        {
            VFXManager.Instance?.PlayVFX(VFXType.Death, target.currentVisualRoot);

            if (target.currentAnimator)
            {
                // 🔥 GARANTIR l'état Idle/Fight (seul état connecté à Death)
                target.currentAnimator.SetBool(fightBool, true);
                Debug.Log(fightBool);

                // ⚠️ optionnel mais sûr : on laisse 1 frame à l'Animator
                target.currentAnimator.Update(0f);

                // Déclenchement normal de la mort
                target.currentAnimator.SetTrigger(deathTrigger);
            }

            if (target.currentAudio && deathSound)
                target.currentAudio.PlayOneShot(deathSound);

            target.ClampPV();
            return;
        }

        VFXManager.Instance?.PlayVFX(VFXType.Attack, target.currentVisualRoot);
        // ✅ Sinon seulement → Hit
        if (target.currentAnimator)
            target.currentAnimator.SetTrigger(hitTrigger);

        if (target.currentAudio && hitSound)
            target.currentAudio.PlayOneShot(hitSound);

        target.ClampPV();
    }



    // Transform functions

    public void SetInGuard(bool value)
    {
        if (currentAnimator == null) return;
        currentAnimator.SetBool(fightBool, value);
    }


    public void FaceTarget(Vector3 worldTarget)
    {
        if (currentVisualRoot == null) return;

        Vector3 dir = worldTarget - currentVisualRoot.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        currentVisualRoot.rotation = Quaternion.LookRotation(dir);
    }



    // ---------------- HEAL / BUFF ----------------

    public void Heal(int amount)
    {
        pv = Mathf.Clamp(pv + amount, 0, max_pv);
        VFXManager.Instance?.PlayVFX(VFXType.Heal, currentVisualRoot);

        if (currentAnimator)
            currentAnimator.SetTrigger(healTrigger);

        if (currentAudio && healSound)
            currentAudio.PlayOneShot(healSound);
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

        if (currentAnimator)
            currentAnimator.SetTrigger(evolutionTrigger);


        if (currentAudio && evolveSound)
            currentAudio.PlayOneShot(evolveSound);

        yield return null;
        AnimatorStateInfo state = currentAnimator.GetCurrentAnimatorStateInfo(0);

        while (state.normalizedTime < 1f || currentAnimator.IsInTransition(0))
            yield return null;

        ApplyEvolution(currentEvolutionIndex + 1, instant: false);
        VFXManager.Instance?.PlayVFX(VFXType.Evolution, currentVisualRoot);
        isEvolving = false;
    }

    public void Revive()
    {
        pv = max_pv;

        if (currentAnimator != null)
        {
            currentAnimator.Rebind();
            currentAnimator.Update(0f);
            currentAnimator.SetBool(fightBool, true);
        }
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
        currentVisualRoot = evoData.evolutionRoot.transform;

        currentAnimator = evoData.evolutionRoot.GetComponentInChildren<Animator>();
        currentAudio = evoData.evolutionRoot.GetComponentInChildren<AudioSource>();

        if (currentAnimator == null)
        {
            Debug.LogError($"[HeroScript] Aucun Animator trouvé dans {evoData.evolutionRoot.name}");
        }


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

        // 🔥 INITIALISATION CRITIQUE DE L’ANIMATOR
        if (currentAnimator != null)
        {
            currentAnimator.Rebind();
            currentAnimator.Update(0f);
            currentAnimator.SetBool(fightBool, true);
        }

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
