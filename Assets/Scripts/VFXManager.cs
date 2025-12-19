using UnityEngine;

public enum VFXType
{
    Heal,
    Attack,
    Evolution,
    Death
}

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX Prefabs")]
    public GameObject healVFX;
    public GameObject attackVFX;
    public GameObject evolutionVFX;
    public GameObject deathVFX;

    public Vector3 defaultOffset = new Vector3(0, 0.15f, 0);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayVFX(VFXType type, Transform target)
    {
        GameObject prefab = type switch
        {
            VFXType.Heal => healVFX,
            VFXType.Attack => attackVFX,
            VFXType.Evolution => evolutionVFX,
            VFXType.Death => deathVFX,
            _ => null
        };

        if (prefab == null || target == null)
            return;

        Quaternion rot = Quaternion.Euler(new Vector3(-90, 0, 0));
        if (type == VFXType.Evolution )
            rot = Quaternion.Euler(new Vector3(-180, 0, 0));
            

        GameObject vfx = Instantiate(
            prefab,
            target.position + defaultOffset, rot
            
        );
        vfx.transform.localScale = vfx.transform.localScale/15f;

        vfx.transform.SetParent(target);
        Destroy(vfx, 5f);
    }
}
