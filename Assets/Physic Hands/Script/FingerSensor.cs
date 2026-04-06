using UnityEngine;

public class FingerSensor : MonoBehaviour
{
    [Header("Finger info")]
    public string fingerName = "Index";

    [Header("Finger references")]
    public Transform masterTip;
    public Transform physicsTip;

    [Header("Compression")]
    public float maxCompression = 0.03f;

    [Header("Bar visualization")]
    public Transform pressureBar;          // объект столбца
    public float maxBarHeight = 0.2f;      // максимальная высота
    public float smoothSpeed = 10f;

    [Header("Signal (for future LSL)")]
    public float minFrequency = 1f;
    public float maxFrequency = 20f;

    bool isTouching = false;

    float pressure = 0f;
    float visualPressure = 0f;

    // float currentFrequency = 0f;   // оставляем для будущего
    // float phase = 0f;

    void Update()
    {
        if (!isTouching)
        {
            pressure = 0f;
        }
        else
        {
            float compression =
                Vector3.Distance(masterTip.position, physicsTip.position);

            pressure =
                Mathf.Clamp01(compression / maxCompression);

            // ===== ЛОГИКА ЧАСТОТЫ (НЕ УДАЛЯТЬ) =====
            /*
            currentFrequency =
                Mathf.Lerp(minFrequency, maxFrequency, pressure);

            phase += Time.deltaTime * currentFrequency * 2 * Mathf.PI;
            */
        }

        UpdateBar();
    }

    void UpdateBar()
    {
        if (!pressureBar)
            return;

        visualPressure = Mathf.Lerp(
            visualPressure,
            pressure,
            Time.deltaTime * smoothSpeed
        );

        Vector3 scale = pressureBar.localScale;
        scale.y = Mathf.Max(0.001f, visualPressure * maxBarHeight);
        pressureBar.localScale = scale;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        isTouching = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        isTouching = false;
    }
}