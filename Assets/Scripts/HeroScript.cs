using Unity.VisualScripting;
using UnityEngine;

public class HeroScript : MonoBehaviour
{
    [Header("Stats")]
    public int max_pv = 1000;
    public int attack_base = 100;
    public int pv = 900;
    public int lvl = 0;
    public HeroScript currentOpponent;

    [Header("Animation")]
    public Animator animator;

    [Header("Visuel PV")]
    public Renderer heroRenderer;

    void Start()
    {
        pv = Mathf.Clamp(pv, 0, max_pv);

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (animator != null)
            animator.SetTrigger("PunchTrigger");
            currentOpponent.animator.SetTrigger("HittedTrigger");
            currentOpponent.UpdateColor();

        if (currentOpponent != null)
            currentOpponent.pv -= attack_base;
    }

    public void Buff()
    {
        attack_base += 50;
    }

    // Dans HeroScript.cs

    public void Heal(int healAmount) // <-- On ajoute l'argument 'int healAmount'
    {
        // Optionnel : Vous pouvez commenter ou supprimer l'animation si elle est incorrecte
        if (animator != null) 
            animator.SetTrigger("HealTrigger"); 

        pv += healAmount; // <-- On utilise la valeur passée en argument
        pv = Mathf.Clamp(pv, 0, max_pv); // On utilise Clamp pour simplifier la vérification max_pv
        UpdateColor();
    }
    // --- Couleur en fonction des PV ---
    void UpdateColor()
    {
        if (heroRenderer == null) return;

        float ratio = (float)pv / max_pv;
        Color c;

        if (ratio <= 0.1)
        {
            c = Color.black;        // mort
        }
        else if (ratio >= 0.9f)
        {
            c = Color.green;        // full ou presque
        }
        else if (ratio >= 0.4f)
        {
            c = Color.yellow;       // milieu de vie
        }
        else
        {
            c = Color.red;          // low HP
        }

        heroRenderer.material.color = c;
        if(pv <= 0)
        {
            animator.SetTrigger("DeathTrigger");
            currentOpponent.IncrementLVL();
        }
    }

    void IncrementLVL()
    {
        lvl++;
    }
}
