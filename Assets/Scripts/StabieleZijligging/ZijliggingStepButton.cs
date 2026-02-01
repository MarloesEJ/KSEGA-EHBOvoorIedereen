using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZijliggingStepButton : MonoBehaviour
{
    [Min(1)]
    [SerializeField] private int stepNumber = 1;
    [SerializeField] private ZijliggingGameManager manager;
    [SerializeField] private ZijliggingSelectionVisuals selectionVisuals;

    private int selectionOrder;

    public int StepNumber => stepNumber;

    private void Awake()
    {
        if (selectionVisuals == null)
        {
            selectionVisuals = GetComponent<ZijliggingSelectionVisuals>();
        }
    }

    private void OnMouseDown()
    {
        Press();
    }

    public void Press()
    {
        var activeManager = manager;

        if (activeManager != null)
        {
            var selection = activeManager.RegisterSelection(this);
            if (selection != ZijliggingGameManager.SelectionChange.Selected)
            {
                return;
            }
        }

        if (activeManager != null)
        {
            activeManager.TryApplyStep(stepNumber);
        }
    }

    public void SetStepNumber(int number)
    {
        stepNumber = Mathf.Max(1, number);
    }

    public void SetManager(ZijliggingGameManager stepManager)
    {
        manager = stepManager;
    }

    public void SetSelectionVisuals(ZijliggingSelectionVisuals visuals)
    {
        selectionVisuals = visuals;
        if (selectionVisuals != null)
        {
            selectionVisuals.SetSelected(selectionOrder > 0, selectionOrder);
        }
    }

    public void SetSelectionOrder(int order)
    {
        selectionOrder = Mathf.Max(0, order);
        if (selectionVisuals != null)
        {
            selectionVisuals.SetSelected(selectionOrder > 0, selectionOrder);
        }
    }

    public void ClearSelection()
    {
        selectionOrder = 0;
        if (selectionVisuals != null)
        {
            selectionVisuals.SetSelected(false, 0);
        }
    }
}
