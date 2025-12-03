using UnityEngine;

public class ScaleModifior : MonoBehaviour
{
    [Header("Configuration des Touches")]
    public KeyCode scaleUpKey = KeyCode.UpArrow;
    public KeyCode scaleDownKey = KeyCode.DownArrow;

    [Header("Paramètres de Scale")]
    public float scaleSpeed = 2.0f;
    public float minSize = 0.1f;

    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

    void Update()
    {
        if (Input.GetKey(scaleUpKey))
        {
            myTransform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
        }

        if (Input.GetKey(scaleDownKey))
        {
            Vector3 newScale = myTransform.localScale - (Vector3.one * scaleSpeed * Time.deltaTime);

            if (newScale.x > minSize)
            {
                myTransform.localScale = newScale;
            }
        }
    }
}