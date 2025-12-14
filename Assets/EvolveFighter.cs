using NUnit.Framework.Internal.Commands;
using UnityEngine;


public class EvolveFighter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Evolution")]
    public GameObject[] evolutionModels;  // liste des skins (Skin_0, Skin_1, ...)
    private int currentEvolutionIndex = 0;

    bool ResultsEvolve;

    // Update is called once per frame

    public HeroScript HeroScript;
    void Update()
    {
        
    }

    private void Start()
    {
        
    }

    public void EvolutionChar()
    {
        ResultsEvolve = HeroScript.TryEvolutionFromCard(3);
        if (ResultsEvolve == true)
        {
            if (currentEvolutionIndex == 0)
            {
                evolutionModels[currentEvolutionIndex].SetActive(false);
                currentEvolutionIndex++;
                evolutionModels[currentEvolutionIndex].SetActive(true);
            }
            
        }

        else
        {
            return;
        }

    }
}
