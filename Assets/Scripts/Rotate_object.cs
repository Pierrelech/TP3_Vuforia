using UnityEngine;

public class Rotate_object : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject objectRotate;
    public float rotateSpeed = 50f;
    bool rotateStatus = false;

    public void ScaleOn()
    {
        objectRotate.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }
    
    public void Rotasi()
    {
        if (rotateStatus == false)
        {
            rotateStatus = true;
        }
        else
        {
            rotateStatus = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateStatus == true)
        {
            objectRotate.transform.Rotate(Vector3.up, Time.deltaTime*rotateSpeed);
        }
    }
}
