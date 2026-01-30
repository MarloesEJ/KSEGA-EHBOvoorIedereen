using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggablePlaceholder : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool snapToSlots = true;
    [SerializeField] private int dragSortingOrder = 50;
    [SerializeField] private DropSlotLayout dropSlotLayout;
    [SerializeField] private float minDragDistance = 0.05f;
    [SerializeField] private bool returnToHomeOnFail = true;

    private Vector3 dragOffset;
    private float dragZ;
    private Vector3 dragStartPosition;
    private Vector3 homePosition;
    private bool homePositionSet;
    private bool dragging;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (dropSlotLayout == null)
        {
            dropSlotLayout = FindObjectOfType<DropSlotLayout>();
        }
    }

    private void Start()
    {
        if (!homePositionSet)
        {
            homePosition = transform.position;
            homePositionSet = true;
        }
    }

    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    public void SetDropSlotLayout(DropSlotLayout layout)
    {
        dropSlotLayout = layout;
    }

    public void SetHomePosition(Vector3 position)
    {
        homePosition = position;
        homePositionSet = true;
    }

    private void OnMouseDown()
    {
        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        dragging = false;
        dragStartPosition = transform.position;
        if (!homePositionSet)
        {
            homePosition = dragStartPosition;
            homePositionSet = true;
        }
        dragZ = cam.WorldToScreenPoint(transform.position).z;
        dragOffset = transform.position - GetPointerWorldPosition(cam);

        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
            if (dragSortingOrder > originalSortingOrder)
            {
                spriteRenderer.sortingOrder = dragSortingOrder;
            }
        }
    }

    private void OnMouseDrag()
    {
        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        if (!dragging)
        {
            float delta = (GetPointerWorldPosition(cam) + dragOffset - dragStartPosition).sqrMagnitude;
            if (delta < minDragDistance * minDragDistance)
            {
                return;
            }

            dragging = true;
            dropSlotLayout?.ReleaseOccupant(transform);
        }

        transform.position = GetPointerWorldPosition(cam) + dragOffset;
    }

    private void OnMouseUp()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }

        if (!dragging)
        {
            transform.position = dragStartPosition;
            dragging = false;
            return;
        }

        if (snapToSlots && dropSlotLayout != null)
        {
            bool snapped = dropSlotLayout.TrySnapToClosestSlot(transform, dragStartPosition);
            if (!snapped)
            {
                transform.position = returnToHomeOnFail && homePositionSet ? homePosition : dragStartPosition;
            }
        }

        dragging = false;
    }

    private Vector3 GetPointerWorldPosition(Camera cam)
    {
        var screenPoint = Input.mousePosition;
        screenPoint.z = dragZ;
        return cam.ScreenToWorldPoint(screenPoint);
    }
}
