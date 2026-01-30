using System.Collections.Generic;
using UnityEngine;

public class StepRowLayout : MonoBehaviour
{
    [SerializeField] private StepPlaceholder[] items;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float horizontalPadding = 0.6f;
    [SerializeField] private bool shuffleOnStart = false;
    [SerializeField] private bool useFixedXBounds = true;
    [SerializeField] private float minX = -6f;
    [SerializeField] private float maxX = 6f;
    [SerializeField] private bool anchorToTop = true;
    [SerializeField] private float topPadding = 0.8f;
    [SerializeField] private float rowOffset = 0f;

    private void Start()
    {
        Layout(shuffleOnStart);
    }

    public void SetItems(StepPlaceholder[] stepItems)
    {
        items = stepItems;
    }

    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    public void Layout()
    {
        Layout(shuffleOnStart);
    }

    public void Layout(bool applyShuffle)
    {
        var ordered = GetOrderedItems();
        if (ordered.Count == 0)
        {
            return;
        }

        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        if (applyShuffle)
        {
            Shuffle(ordered);
        }

        var positions = CalculatePositions(ordered.Count, cam);
        for (int i = 0; i < ordered.Count; i++)
        {
            var target = ordered[i].transform;
            var position = positions[i];
            target.position = new Vector3(position.x, position.y, target.position.z);

            var draggable = target.GetComponent<DraggablePlaceholder>();
            if (draggable != null)
            {
                draggable.SetHomePosition(target.position);
            }
        }
    }

    private List<StepPlaceholder> GetOrderedItems()
    {
        var list = new List<StepPlaceholder>();
        if (items != null && items.Length > 0)
        {
            list.AddRange(items);
        }
        else
        {
            list.AddRange(GetComponentsInChildren<StepPlaceholder>());
        }

        list.RemoveAll(item => item == null);
        list.Sort((a, b) => a.orderIndex.CompareTo(b.orderIndex));
        return list;
    }

    private List<Vector3> CalculatePositions(int count, Camera cam)
    {
        var positions = new List<Vector3>(count);
        if (count <= 0)
        {
            return positions;
        }

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float layoutMinX = cam.transform.position.x - halfWidth;
        float layoutMaxX = cam.transform.position.x + halfWidth;
        if (useFixedXBounds)
        {
            layoutMinX = minX;
            layoutMaxX = maxX;
        }

        if (layoutMaxX < layoutMinX)
        {
            float temp = layoutMinX;
            layoutMinX = layoutMaxX;
            layoutMaxX = temp;
        }

        float usableWidth = Mathf.Max(0.1f, (layoutMaxX - layoutMinX) - (horizontalPadding * 2f));
        float spacing = count > 1 ? usableWidth / (count - 1) : 0f;
        float startX = layoutMinX + horizontalPadding;
        float topY = cam.transform.position.y + halfHeight - topPadding;
        float bottomY = cam.transform.position.y - halfHeight + topPadding;
        float rowY = anchorToTop ? (topY - rowOffset) : (bottomY + rowOffset);

        for (int i = 0; i < count; i++)
        {
            positions.Add(new Vector3(startX + spacing * i, rowY, 0f));
        }

        return positions;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
