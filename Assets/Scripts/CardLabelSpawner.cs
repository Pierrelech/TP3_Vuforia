using UnityEngine;

public class CardLabelSpawner : MonoBehaviour
{
    [Header("Le prefab du texte 3D")]
    public GameObject labelPrefab;

    [Header("Texte à afficher")]
    public string labelText = "ACTION";

    [Header("Décalage au-dessus de la carte")]
    public float verticalOffset = 0.12f;

    private GameObject currentLabel;

    public void OnCardDetected()
    {
        SpawnLabel();
    }

    public void OnCardLost()
    {
        if (currentLabel != null)
            Destroy(currentLabel);
    }

    void SpawnLabel()
    {
        if (labelPrefab == null) return;

        Vector3 spawnPos = transform.position + transform.up * verticalOffset;

        currentLabel = Instantiate(labelPrefab, spawnPos, Quaternion.identity);
        currentLabel.transform.SetParent(transform);

        // Mettre le bon texte
        var tmp = currentLabel.GetComponent<TMPro.TextMeshPro>();
        tmp.text = labelText;

        // Durée de vie
        Destroy(currentLabel, 1.8f);
    }
}
