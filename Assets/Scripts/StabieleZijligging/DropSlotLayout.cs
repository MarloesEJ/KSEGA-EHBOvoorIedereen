using System.Collections.Generic;
using UnityEngine;
using System;

public class DropSlotLayout : MonoBehaviour
{
    [SerializeField] private DropSlot[] slots;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float horizontalPadding = 0.6f;
    [SerializeField] private bool useFixedXBounds = true;
    [SerializeField] private float minX = -6f;
    [SerializeField] private float maxX = 6f;
    [SerializeField] private bool anchorToTop = true;
    [SerializeField] private float topPadding = 0.8f;
    [SerializeField] private float rowOffset = 2.2f;
    [SerializeField] private float[] perSlotYOffset;
    [SerializeField] private float maxSnapDistance = 1.5f;

    public event Action Completed;

    private List<Vector3> slotPositions = new List<Vector3>();
    private readonly Dictionary<Transform, int> occupantToSlot = new Dictionary<Transform, int>();
    private Transform[] slotToOccupant = new Transform[0];
    private DropSlot[] orderedSlots = new DropSlot[0];
    private bool completionFired;

    private void Start()
    {
        Layout();
    }

    public void SetSlots(DropSlot[] dropSlots)
    {
        slots = dropSlots;
    }

    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    public void SetPerSlotYOffset(float[] offsets)
    {
        perSlotYOffset = offsets;
    }

    public void SetMaxSnapDistance(float distance)
    {
        maxSnapDistance = distance;
    }

    public void ReleaseOccupant(Transform draggable)
    {
        if (draggable == null)
        {
            return;
        }

        if (occupantToSlot.TryGetValue(draggable, out int slotIndex))
        {
            if (slotIndex >= 0 && slotIndex < slotToOccupant.Length && slotToOccupant[slotIndex] == draggable)
            {
                slotToOccupant[slotIndex] = null;
            }

            occupantToSlot.Remove(draggable);
        }
    }

    public void Layout()
    {
        var ordered = GetOrderedSlots();
        if (ordered.Count == 0)
        {
            return;
        }

        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        slotPositions = CalculatePositions(ordered.Count, cam);
        EnsureSlotMapping(ordered.Count);
        orderedSlots = ordered.ToArray();
        completionFired = false;

        for (int i = 0; i < ordered.Count; i++)
        {
            var target = ordered[i].transform;
            var position = slotPositions[i];
            target.position = new Vector3(position.x, position.y, target.position.z);
        }
    }

    public bool TrySnapToClosestSlot(Transform draggable, Vector3 fallbackPosition)
    {
        if (draggable == null || slotPositions.Count == 0)
        {
            return false;
        }

        if (slotToOccupant.Length != slotPositions.Count)
        {
            EnsureSlotMapping(slotPositions.Count);
        }

        int closestIndex = 0;
        float bestDistance = float.MaxValue;
        Vector3 position = draggable.position;
        for (int i = 0; i < slotPositions.Count; i++)
        {
            float distance = (position - slotPositions[i]).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closestIndex = i;
            }
        }

        if (maxSnapDistance > 0f)
        {
            float maxDistanceSquared = maxSnapDistance * maxSnapDistance;
            if (bestDistance > maxDistanceSquared)
            {
                return false;
            }
        }

        Transform occupant = slotToOccupant[closestIndex];
        if (occupant != null && occupant != draggable)
        {
            return false;
        }

        var snapPosition = slotPositions[closestIndex];
        snapPosition.z = draggable.position.z;
        draggable.position = snapPosition;
        slotToOccupant[closestIndex] = draggable;
        occupantToSlot[draggable] = closestIndex;
        CheckCompletion();
        return true;
    }

    private List<DropSlot> GetOrderedSlots()
    {
        var list = new List<DropSlot>();
        if (slots != null && slots.Length > 0)
        {
            list.AddRange(slots);
        }
        else
        {
            list.AddRange(GetComponentsInChildren<DropSlot>());
        }

        list.RemoveAll(item => item == null);
        list.Sort((a, b) => a.orderIndex.CompareTo(b.orderIndex));
        return list;
    }

    private void EnsureSlotMapping(int count)
    {
        slotToOccupant = new Transform[count];
        occupantToSlot.Clear();
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
            float offsetY = 0f;
            if (perSlotYOffset != null && i < perSlotYOffset.Length)
            {
                offsetY = perSlotYOffset[i];
            }

            positions.Add(new Vector3(startX + spacing * i, rowY + offsetY, 0f));
        }

        return positions;
    }

    private void CheckCompletion()
    {
        if (completionFired || orderedSlots.Length == 0)
        {
            return;
        }

        for (int i = 0; i < orderedSlots.Length; i++)
        {
            var occupant = slotToOccupant[i];
            if (occupant == null)
            {
                return;
            }

            var placeholder = occupant.GetComponent<StepPlaceholder>();
            if (placeholder == null)
            {
                return;
            }

            if (placeholder.orderIndex != orderedSlots[i].orderIndex)
            {
                return;
            }
        }

        completionFired = true;
        Completed?.Invoke();
    }
}
