using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroUI : MonoBehaviour
{
    [Header("Références")]
    public HeroScript hero;

    [Header("UI")]
    public Slider healthSlider;
    public TMP_Text levelText;
    public Image fillImage;
    [Header("Animation")]
    public float smoothSpeed = 5f; // 0 = instantané

    private float targetValue;

    void Start()
    {
        if (hero == null)
            hero = GetComponentInParent<HeroScript>();

        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;

        UpdateInstant();
    }

    void Update()
    {
        UpdateSmooth();

        float ratio = healthSlider.value;

        fillImage.color =
            ratio <= 0.2f ? Color.red :
            ratio <= 0.4f ? new Color(1f, 0.6f, 0f) :
            ratio <= 0.7f ? Color.yellow :
            Color.green;
    }

    void UpdateInstant()
    {
        if (hero == null) return;

        float ratio = (float)hero.pv / hero.max_pv;
        healthSlider.value = ratio;
        targetValue = ratio;

        levelText.text = $"LVL {hero.lvl}";
    }

    void UpdateSmooth()
    {
        if (hero == null) return;

        float ratio = (float)hero.pv / hero.max_pv;
        targetValue = ratio;

        if (smoothSpeed > 0f)
        {
            healthSlider.value = Mathf.Lerp(
                healthSlider.value,
                targetValue,
                Time.deltaTime * smoothSpeed
            );
        }
        else
        {
            healthSlider.value = targetValue;
        }

        levelText.text = $"LVL {hero.lvl}";
    }
}
