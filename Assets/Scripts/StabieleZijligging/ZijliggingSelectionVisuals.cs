using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ZijliggingSelectionVisuals : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Border")]
    [SerializeField] private Color borderColor = new Color(0.2f, 0.55f, 1f, 1f);
    [SerializeField] private float borderPaddingRatio = 0.03f;
    [SerializeField] private float borderWidthRatio = 0.02f;
    [SerializeField] private int borderSortingOffset = 10;

    [Header("Badge")]
    [SerializeField] private Color badgeColor = new Color(0.2f, 0.55f, 1f, 1f);
    [SerializeField] private Color badgeTextColor = Color.black;
    [SerializeField] private float badgeRadiusRatio = 0.12f;
    [SerializeField] private float badgeOffsetRatio = 0.05f;
    [SerializeField] private float badgeLineWidthRatio = 0.02f;
    [SerializeField] private int badgeSegments = 32;
    [SerializeField] private int badgeSortingOffset = 20;
    [SerializeField] private int badgeTextSortingOffset = 25;
    [SerializeField] private int badgeFontSize = 24;
    [SerializeField] private float badgeTextScaleRatio = 0.75f;

    private LineRenderer borderRenderer;
    private LineRenderer badgeRenderer;
    private TextMesh badgeText;
    private static Material lineMaterial;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        EnsureVisuals();
        SetSelected(false, 0);
    }

    public void SetSelected(bool selected, int order)
    {
        bool show = selected && order > 0;
        if (borderRenderer != null)
        {
            borderRenderer.enabled = show;
        }

        if (badgeRenderer != null)
        {
            badgeRenderer.enabled = show;
        }

        if (badgeText != null)
        {
            badgeText.gameObject.SetActive(show);
            if (show)
            {
                badgeText.text = order.ToString();
            }
        }
    }

    public void RefreshLayout()
    {
        EnsureVisuals();
        UpdateGeometry();
    }

    private void EnsureVisuals()
    {
        if (lineMaterial == null)
        {
            var shader = Shader.Find("Sprites/Default");
            lineMaterial = shader != null ? new Material(shader) : null;
        }

        if (borderRenderer == null)
        {
            var borderGO = new GameObject("SelectionBorder");
            borderGO.transform.SetParent(transform, false);
            borderRenderer = borderGO.AddComponent<LineRenderer>();
            ConfigureLineRenderer(borderRenderer, borderColor, borderSortingOffset);
        }

        if (badgeRenderer == null)
        {
            var badgeGO = new GameObject("SelectionBadge");
            badgeGO.transform.SetParent(transform, false);
            badgeRenderer = badgeGO.AddComponent<LineRenderer>();
            ConfigureLineRenderer(badgeRenderer, badgeColor, badgeSortingOffset);

            var textGO = new GameObject("SelectionNumber");
            textGO.transform.SetParent(badgeGO.transform, false);
            badgeText = textGO.AddComponent<TextMesh>();
            badgeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            badgeText.fontStyle = FontStyle.Bold;
            badgeText.anchor = TextAnchor.MiddleCenter;
            badgeText.alignment = TextAlignment.Center;
            badgeText.fontSize = badgeFontSize;
            badgeText.color = badgeTextColor;

            var textRenderer = textGO.GetComponent<MeshRenderer>();
            if (textRenderer != null)
            {
                textRenderer.sortingOrder = GetBaseSortingOrder() + badgeTextSortingOffset;
                if (targetRenderer != null)
                {
                    textRenderer.sortingLayerID = targetRenderer.sortingLayerID;
                }
            }
        }

        UpdateGeometry();
    }

    private void ConfigureLineRenderer(LineRenderer renderer, Color color, int sortingOffset)
    {
        renderer.useWorldSpace = false;
        renderer.loop = false;
        renderer.positionCount = 0;
        renderer.startColor = color;
        renderer.endColor = color;
        renderer.startWidth = 0.05f;
        renderer.endWidth = 0.05f;
        renderer.numCapVertices = 6;
        renderer.numCornerVertices = 6;
        renderer.sortingOrder = GetBaseSortingOrder() + sortingOffset;
        if (targetRenderer != null)
        {
            renderer.sortingLayerID = targetRenderer.sortingLayerID;
        }
        if (lineMaterial != null)
        {
            renderer.material = lineMaterial;
        }
    }

    private void UpdateGeometry()
    {
        if (targetRenderer == null)
        {
            return;
        }

        var sprite = targetRenderer.sprite;
        Vector2 size = sprite != null ? sprite.bounds.size : Vector2.one;
        float minSize = Mathf.Max(0.001f, Mathf.Min(size.x, size.y));
        Vector2 worldSize = targetRenderer.bounds.size;
        float minWorldSize = Mathf.Max(0.001f, Mathf.Min(worldSize.x, worldSize.y));

        float padding = borderPaddingRatio * minSize;
        float halfWidth = (size.x * 0.5f) + padding;
        float halfHeight = (size.y * 0.5f) + padding;

        float borderWidth = borderWidthRatio * minWorldSize;
        borderRenderer.startWidth = borderWidth;
        borderRenderer.endWidth = borderWidth;

        borderRenderer.positionCount = 5;
        borderRenderer.SetPosition(0, new Vector3(-halfWidth, -halfHeight, 0f));
        borderRenderer.SetPosition(1, new Vector3(-halfWidth, halfHeight, 0f));
        borderRenderer.SetPosition(2, new Vector3(halfWidth, halfHeight, 0f));
        borderRenderer.SetPosition(3, new Vector3(halfWidth, -halfHeight, 0f));
        borderRenderer.SetPosition(4, new Vector3(-halfWidth, -halfHeight, 0f));

        float badgeRadius = badgeRadiusRatio * minSize;
        float badgeOffset = badgeOffsetRatio * minSize;
        Vector3 badgeCenter = new Vector3(
            -size.x * 0.5f + badgeOffset + badgeRadius,
            size.y * 0.5f - badgeOffset - badgeRadius,
            0f);

        float badgeWidth = badgeLineWidthRatio * minWorldSize;
        badgeRenderer.startWidth = badgeWidth;
        badgeRenderer.endWidth = badgeWidth;
        badgeRenderer.positionCount = badgeSegments + 1;
        for (int i = 0; i <= badgeSegments; i++)
        {
            float t = (float)i / badgeSegments;
            float angle = t * Mathf.PI * 2f;
            var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * badgeRadius;
            badgeRenderer.SetPosition(i, badgeCenter + offset);
        }

        if (badgeText != null)
        {
            badgeText.fontSize = badgeFontSize;
            badgeText.characterSize = badgeRadius * badgeTextScaleRatio;
            badgeText.transform.localPosition = badgeCenter;
            badgeText.transform.localScale = Vector3.one;
        }
    }

    private int GetBaseSortingOrder()
    {
        return targetRenderer != null ? targetRenderer.sortingOrder : 0;
    }
}
