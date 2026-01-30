using System.Collections.Generic;
using UnityEngine;

public class PlaceholderRowLayout : MonoBehaviour
{
    [SerializeField] private StepPlaceholder[] placeholders;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float bottomPadding = 0.6f;
    [SerializeField] private float horizontalPadding = 0.8f;
    [SerializeField] private bool shuffleOnStart = true;
    [SerializeField] private bool useFixedXBounds = true;
    [SerializeField] private float minX = -6f;
    [SerializeField] private float maxX = 6f;

    private List<Vector3> slotPositions = new List<Vector3>();
    private Dictionary<StepPlaceholder, int> placeholderToSlot = new Dictionary<StepPlaceholder, int>();
    private StepPlaceholder[] slotToPlaceholder;

    private void Start()
    {
        Layout(shuffleOnStart);
    }

    public void SetPlaceholders(StepPlaceholder[] items)
    {
        placeholders = items;
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
        var ordered = GetOrderedPlaceholders();
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

        slotPositions = CalculateSlotPositions(ordered.Count, cam);
        EnsureSlotMapping(ordered.Count);

        for (int i = 0; i < ordered.Count; i++)
        {
            var placeholder = ordered[i];
            var position = slotPositions[i];
            position.z = placeholder.transform.position.z;
            placeholder.transform.position = position;
            slotToPlaceholder[i] = placeholder;
            placeholderToSlot[placeholder] = i;
        }
    }

    public void SnapToClosestSlot(StepPlaceholder placeholder)
    {
        if (placeholder == null || slotPositions == null || slotPositions.Count == 0)
        {
            return;
        }

        int closestIndex = 0;
        float bestDistance = float.MaxValue;
        Vector3 position = placeholder.transform.position;
        for (int i = 0; i < slotPositions.Count; i++)
        {
            float distance = (position - slotPositions[i]).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closestIndex = i;
            }
        }

        int previousSlot = -1;
        placeholderToSlot.TryGetValue(placeholder, out previousSlot);

        if (slotToPlaceholder == null || slotToPlaceholder.Length != slotPositions.Count)
        {
            slotToPlaceholder = new StepPlaceholder[slotPositions.Count];
        }

        StepPlaceholder occupant = slotToPlaceholder[closestIndex];
        if (occupant != null && occupant != placeholder && previousSlot >= 0 && previousSlot < slotPositions.Count)
        {
            var swapPosition = slotPositions[previousSlot];
            swapPosition.z = occupant.transform.position.z;
            occupant.transform.position = swapPosition;
            slotToPlaceholder[previousSlot] = occupant;
            placeholderToSlot[occupant] = previousSlot;
        }
        else if (previousSlot >= 0 && previousSlot < slotPositions.Count)
        {
            slotToPlaceholder[previousSlot] = null;
        }

        var snapPosition = slotPositions[closestIndex];
        snapPosition.z = placeholder.transform.position.z;
        placeholder.transform.position = snapPosition;
        slotToPlaceholder[closestIndex] = placeholder;
        placeholderToSlot[placeholder] = closestIndex;
    }

    private List<StepPlaceholder> GetOrderedPlaceholders()
    {
        var list = new List<StepPlaceholder>();
        if (placeholders != null && placeholders.Length > 0)
        {
            list.AddRange(placeholders);
        }
        else
        {
            list.AddRange(GetComponentsInChildren<StepPlaceholder>());
        }

        list.RemoveAll(item => item == null);
        list.Sort((a, b) => a.orderIndex.CompareTo(b.orderIndex));
        return list;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void EnsureSlotMapping(int count)
    {
        if (slotToPlaceholder == null || slotToPlaceholder.Length != count)
        {
            slotToPlaceholder = new StepPlaceholder[count];
        }
        else
        {
            for (int i = 0; i < slotToPlaceholder.Length; i++)
            {
                slotToPlaceholder[i] = null;
            }
        }

        placeholderToSlot.Clear();
    }

    private List<Vector3> CalculateSlotPositions(int count, Camera cam)
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
        float y = cam.transform.position.y - halfHeight + bottomPadding;

        for (int i = 0; i < count; i++)
        {
            positions.Add(new Vector3(startX + spacing * i, y, 0f));
        }

        return positions;
    }
}
