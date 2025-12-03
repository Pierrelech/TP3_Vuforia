using UnityEngine;

public class move_rotate : MonoBehaviour
{
    public float speed;        // vitesse a definir dans l'interface unity
    public float rotateSpeed;  // vitesse de rotation a definir dans l'interface unity

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // si la touche fleche du haut appuyee, alors faire une transformation de type translation selon un vecteur "en avant"
        // avec la vitesse speed defini precedemment
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        // si la touche fleche du bas appuyee, alors faire une transformation de type translation selon un vecteur "en arriere"
        // avec la vitesse speed defini precedemment
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }

        // si la touche fleche du bas appuyee, alors faire une transformation de type rotation sur x
        // avec la vitesse rotationSpeed defini precedemment
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down, Time.deltaTime * rotateSpeed);
        }

        // si la touche fleche du bas appuyee, alors faire une transformation de type rotation sur x
        // avec la vitesse rotationSpeed defini precedemment
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
        }
    }
}
