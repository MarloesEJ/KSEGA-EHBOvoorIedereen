using UnityEngine;

public class ZijliggingGameManager : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Sprite[] stepSprites;
    [SerializeField] private bool setFirstSpriteOnReset = true;

    private int nextStepIndex;
    private bool sequenceCompleted;
    private bool registeredWithGameManager;

    private void OnEnable()
    {
        TryRegisterWithGameManager();
    }

    private void Start()
    {
        TryRegisterWithGameManager();
        ResetSequence();
    }

    private void Update()
    {
        if (!registeredWithGameManager)
        {
            TryRegisterWithGameManager();
        }
    }

    private void OnDisable()
    {
        if (registeredWithGameManager && GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterZijliggingGameManager(this);
            registeredWithGameManager = false;
        }
    }

    public void OnStepButtonPressed(int stepNumber)
    {
        TryApplyStep(stepNumber);
    }

    public bool TryApplyStep(int stepNumber)
    {
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
            Debug.Log($"ZijliggingGameManager: step {stepNumber} is out of order. Expected {nextStepIndex + 1}.");
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

    private void TryRegisterWithGameManager()
    {
        if (registeredWithGameManager || GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.RegisterZijliggingGameManager(this);
        registeredWithGameManager = true;
    }
}
