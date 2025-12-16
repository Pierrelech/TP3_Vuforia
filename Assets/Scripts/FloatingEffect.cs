using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float floatHeight = 0.025f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
