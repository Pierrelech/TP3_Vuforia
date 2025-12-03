using UnityEngine;

[System.Serializable]
public class MoleculeRecipe
{
    public string moleculeName;          // "Eau"
    public GameObject moleculePrefab;    // Modèle 3D H2O
    public int nbH;
    public int nbO;
    public int nbC;
}
