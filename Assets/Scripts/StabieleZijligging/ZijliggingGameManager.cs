using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZijliggingGameManager : MonoBehaviour
{
    public enum SelectionChange
    {
        Blocked = 0,
        Selected = 1,
        Deselected = 2
    }

    [Header("Display")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Sprite[] stepSprites;
    [SerializeField] private bool setFirstSpriteOnReset = true;
    [SerializeField] private GameObject dialog = null;
    [SerializeField] private GameObject stepImages = null;

    [Header("Finish")]
    [SerializeField] public GameObject resultPanel;
    [SerializeField] public Image[] stars; // 3 sterren
    [SerializeField] public Sprite inactiveStar; // grijze ster
    [SerializeField] public Sprite activeStar;   // gele ster
    [SerializeField] public TextMeshProUGUI scoreText;

    private int nextStepIndex;
    private bool sequenceCompleted;
    private int nextSelectionOrder = 1;
    private readonly System.Collections.Generic.Dictionary<ZijliggingStepButton, int> selectionOrders =
        new System.Collections.Generic.Dictionary<ZijliggingStepButton, int>();
    private readonly System.Collections.Generic.List<ZijliggingStepButton> stepButtons =
        new System.Collections.Generic.List<ZijliggingStepButton>();
    private DialogueUI dialogueUI = null;

    private int errorCount = 0;

    private void Start()
    {
        AutoWireStepButtons();
        ResetSequence();

        dialog.TryGetComponent(out dialogueUI);
    }

    public void OnStepButtonPressed(int stepNumber)
    {
        TryApplyStep(stepNumber);
    }

    public bool TryApplyStep(int stepNumber)
    {
        // Disable dialog when busy
        dialog.SetActive(false);

        // Start from second element, element 0 is starting position
        stepNumber++;

        if (stepSprites == null || stepSprites.Length == 0)
        {
            Debug.LogWarning("ZijliggingGameManager: stepSprites is empty.");
            return false;
        }

        if (targetRenderer == null)
        {
            Debug.LogWarning("ZijliggingGameManager: targetRenderer is not set.");
            return false;
        }

        int index = stepNumber - 1;
        if (index < 0 || index >= stepSprites.Length)
        {
            Debug.LogWarning($"ZijliggingGameManager: step {stepNumber} is out of range.");
            return false;
        }

        var sprite = stepSprites[index];
        if (sprite == null)
        {
            Debug.LogWarning($"ZijliggingGameManager: sprite for step {stepNumber} is missing.");
            return false;
        }

        targetRenderer.sprite = sprite;

        if (sequenceCompleted)
        {
            return false;
        }

        if (index != nextStepIndex)
        {
            return false;
        }

        nextStepIndex++;

        if (nextStepIndex >= stepSprites.Length)
        {
            sequenceCompleted = true;
        }

        return true;
    }

    public void ResetSequence()
    {
        nextStepIndex = 0;
        sequenceCompleted = false;
        ResetSelections();

        if (setFirstSpriteOnReset && targetRenderer != null && stepSprites != null && stepSprites.Length > 0)
        {
            targetRenderer.sprite = stepSprites[0];
        }
    }

    public void SetTargetRenderer(SpriteRenderer renderer)
    {
        targetRenderer = renderer;
    }

    public void SetStepSprites(Sprite[] sprites)
    {
        stepSprites = sprites;
    }

    public SelectionChange RegisterSelection(ZijliggingStepButton button)
    {
        if (button == null)
        {
            return SelectionChange.Blocked;
        }

        if (selectionOrders.TryGetValue(button, out int existing))
        {
            if (existing == nextSelectionOrder - 1)
            {
                selectionOrders.Remove(button);
                nextSelectionOrder = Mathf.Max(1, nextSelectionOrder - 1);
                button.ClearSelection();
                return SelectionChange.Deselected;
            }

            button.SetSelectionOrder(existing);
            return SelectionChange.Blocked;
        }

        int order = nextSelectionOrder++;
        selectionOrders[button] = order;
        button.SetSelectionOrder(order);
        return SelectionChange.Selected;
    }

    public void DoneButtonClicked()
    {
        bool ok = IsSelectionSequenceCorrect();
        if (!ok)
        {
            dialog.SetActive(true);
            dialogueUI?.SetText("Niet correct. Volg de stappen in de juiste volgorde om hem veilig in zijligging te leggen.");
            ResetSequence();
            errorCount++;
        }
        else
        {
            Finish();
        }
    }

    private void ResetSelections()
    {
        nextSelectionOrder = 1;
        selectionOrders.Clear();
        for (int i = 0; i < stepButtons.Count; i++)
        {
            var button = stepButtons[i];
            if (button != null)
            {
                button.ClearSelection();
            }
        }
    }

    private bool IsSelectionSequenceCorrect()
    {
        int expectedCount = GetExpectedSelectionCount();
        if (expectedCount <= 0)
        {
            return true;
        }

        if (selectionOrders.Count != expectedCount)
        {
            return false;
        }

        foreach (var entry in selectionOrders)
        {
            if (entry.Key == null)
            {
                return false;
            }

            if (entry.Key.StepNumber != entry.Value)
            {
                return false;
            }
        }

        return true;
    }

    private int GetExpectedSelectionCount()
    {
        if (stepButtons.Count > 0)
        {
            return stepButtons.Count;
        }

        if (stepSprites != null && stepSprites.Length > 0)
        {
            return stepSprites.Length;
        }

        return 0;
    }

    private void AutoWireStepButtons()
    {
        stepButtons.Clear();

        var placeholders = GetComponentsInChildren<StepPlaceholder>(true);
        if (placeholders == null || placeholders.Length == 0)
        {
            placeholders = FindObjectsOfType<StepPlaceholder>(true);
        }

        foreach (var placeholder in placeholders)
        {
            if (placeholder == null)
            {
                continue;
            }

            var button = placeholder.GetComponent<ZijliggingStepButton>();
            if (button == null)
            {
                button = placeholder.gameObject.AddComponent<ZijliggingStepButton>();
            }

            button.SetStepNumber(placeholder.orderIndex);
            button.SetManager(this);

            var visuals = placeholder.GetComponent<ZijliggingSelectionVisuals>();
            if (visuals == null)
            {
                visuals = placeholder.gameObject.AddComponent<ZijliggingSelectionVisuals>();
            }

            visuals.RefreshLayout();
            button.SetSelectionVisuals(visuals);
            stepButtons.Add(button);
        }
    }

    public void Finish()
    {

        int activeStars = stars.Count() - errorCount;

        for (int i = 0; i < stars.Count(); i++)
        {
            stars[i].sprite = (i < activeStars) ? activeStar : inactiveStar;
        }

        // Stop game
        GameManager.Instance?.SetState(GameState.Win);

        GameManager.Instance?.AddScore(activeStars * 100);

        // Score tonen
        scoreText.text = $"Punten: {GameManager.Instance?.Score}";

        // UI aanpassen
        resultPanel.SetActive(true);
        stepImages.SetActive(false);
    }

    public void Nextlevel()
    {
        GameManager.Instance?.LoadNextLevel();
    }
}
