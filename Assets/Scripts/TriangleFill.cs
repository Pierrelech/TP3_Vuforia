using UnityEngine;
using Vuforia;

public class TriangleFill : MonoBehaviour
{
    [Header("Objets 3D (sommets du triangle)")]
    public GameObject capsule;
    public GameObject cube;
    public GameObject sphere;

    [Header("Trackers (ImageTargets) associés")]
    public GameObject capsuleTracker;
    public GameObject cubeTracker;
    public GameObject sphereTracker;

    [Header("Mesh de remplissage")]
    public MeshFilter triangleMeshFilter;
    public MeshRenderer triangleMeshRenderer;

    private Mesh triangleMesh;

    void Start()
    {
        // Création du mesh du triangle
        triangleMesh = new Mesh();
        triangleMesh.name = "TriangleFillMesh";

        if (triangleMeshFilter != null)
        {
            triangleMeshFilter.mesh = triangleMesh;
        }
    }

    void Update()
    {
        if (capsule == null || cube == null || sphere == null) return;
        if (capsuleTracker == null || cubeTracker == null || sphereTracker == null) return;
        if (triangleMeshFilter == null || triangleMeshRenderer == null) return;

        // Vérifier que les 3 cibles sont trackées
        var capObs = capsuleTracker.GetComponent<ObserverBehaviour>();
        var cubeObs = cubeTracker.GetComponent<ObserverBehaviour>();
        var sphObs = sphereTracker.GetComponent<ObserverBehaviour>();

        if (capObs == null || cubeObs == null || sphObs == null) return;

        var capStatus = capObs.TargetStatus.Status;
        var cubeStatus = cubeObs.TargetStatus.Status;
        var sphStatus = sphObs.TargetStatus.Status;

        bool allTracked =
            capStatus == Status.TRACKED &&
            cubeStatus == Status.TRACKED &&
            sphStatus == Status.TRACKED;

        if (!allTracked)
        {
            // On masque le mesh si tout n’est pas visible
            triangleMeshRenderer.enabled = false;
            return;
        }

        // Les 3 sont visibles : on remplit l’aire
        triangleMeshRenderer.enabled = true;

        // Récupération des positions monde des 3 sommets
        Vector3 A = capsule.transform.position;
        Vector3 B = cube.transform.position;
        Vector3 C = sphere.transform.position;

        // Mise à jour du mesh
        UpdateTriangleMesh(A, B, C);
    }

    void UpdateTriangleMesh(Vector3 A, Vector3 B, Vector3 C)
    {
        // On utilise directement les positions monde comme vertices

        Transform t = triangleMeshFilter.transform;

        // Conversion monde -> local
        Vector3[] vertices = new Vector3[3];
        vertices[0] = t.InverseTransformPoint(A);
        vertices[1] = t.InverseTransformPoint(B);
        vertices[2] = t.InverseTransformPoint(C);

        int[] triangles = new int[3] { 0, 1, 2 };

        triangleMesh.Clear();
        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangles;
        triangleMesh.RecalculateNormals();
    }

    // 🔹 Fonction appelée par le bouton pour changer la couleur
    public void RandomizeColor()
    {
        if (triangleMeshRenderer == null) return;

        Color randomColor = new Color(
            Random.value,
            Random.value,
            Random.value,
            0.4f // un peu transparent
        );

        // Assure-toi que le matériau utilise un shader Transparent
        triangleMeshRenderer.material.color = randomColor;
    }
}
