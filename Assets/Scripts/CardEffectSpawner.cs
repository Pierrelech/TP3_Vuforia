using UnityEngine;
using Vuforia;

public class CardEffectSpawner : MonoBehaviour
{
    [Header("Effet à spawn au-dessus de la carte")]
    public GameObject effectPrefab;

    [Header("Offset vertical de l'effet")]
    public float verticalOffset = 0.15f;

    private GameObject spawnedEffect;

    public void OnCardDetected()
    {
        SpawnEffect();
    }

    public void OnCardLost()
    {
        if (spawnedEffect != null)
            Destroy(spawnedEffect);
    }

    void SpawnEffect()
    {
        if (effectPrefab == null) return;

        // Position juste au-dessus de la carte, peu importe sa rotation
        Vector3 spawnPos = transform.position + transform.up * verticalOffset;

        spawnedEffect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);

        // Faire l'effet suivre la carte pendant qu’elle est visible
        spawnedEffect.transform.SetParent(transform);

        // Effet auto destruction après 2 secondes
        Destroy(spawnedEffect, 2f);
    }
}