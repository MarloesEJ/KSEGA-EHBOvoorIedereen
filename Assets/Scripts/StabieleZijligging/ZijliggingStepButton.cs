using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZijliggingStepButton : MonoBehaviour
{
    [Min(1)]
    [SerializeField] private int stepNumber = 1;
    [SerializeField] private ZijliggingGameManager manager;

    private void OnMouseDown()
    {
        Press();
    }

    public void Press()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyZijliggingStep(stepNumber);
            return;
        }

        if (manager != null)
        {
            manager.TryApplyStep(stepNumber);
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
}
