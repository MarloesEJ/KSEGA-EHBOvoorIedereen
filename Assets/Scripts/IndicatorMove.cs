using UnityEngine;

public class IndicatorMove : MonoBehaviour
{
    public RectTransform bar;        // sleep je TimingBar hier in
    public float speed = 300f;       // pixels per seconde

    private RectTransform indicator;
    private float dir = 1f;

    void Awake()
    {
        indicator = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (bar == null) return;

        float barWidth = bar.rect.width;
        float halfIndicator = indicator.rect.width * 0.5f;

        // Grenzen (links en rechts binnen de bar)
        float minX = halfIndicator;
        float maxX = barWidth - halfIndicator;

        // Huidige positie
        Vector2 pos = indicator.anchoredPosition;

        // Beweeg
        pos.x += dir * speed * Time.deltaTime;

        // Omkeren bij randen
        if (pos.x <= minX) { pos.x = minX; dir = 1f; }
        if (pos.x >= maxX) { pos.x = maxX; dir = -1f; }

        indicator.anchoredPosition = pos;
    }
}
