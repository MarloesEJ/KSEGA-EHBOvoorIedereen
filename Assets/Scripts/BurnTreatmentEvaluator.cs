using System.Collections.Generic;
using UnityEngine;
using System;

public class BurnTreatmentEvaluator : MonoBehaviour
{
    [Header("Correct Order")]
    private List<EHBOAction> correctOrder = new List<EHBOAction>
    {
        EHBOAction.Koelen,
        EHBOAction.Gaas,
        EHBOAction.Verband
    };

    [Header("Runtime Actions")]
    private List<EHBOAction> playerActions = new List<EHBOAction>();

    [Header("Visuals")]
    public SpriteRenderer[] woundRenderers;

    public Sprite normalWound;
    public Sprite cooledWound;
    public Sprite gaasWound;
    public Sprite verbandWound;
    public event Action<EHBOAction> OnActionApplied;

    public DialogueUI dialogueUI;
    // =========================
    // ACTION MANAGEMENT
    // =========================

    void Start()
    {
        OnActionApplied += dialogueUI.ShowActionText;
    }
    public bool HasAction(EHBOAction action)
    {
        return playerActions.Contains(action);
    }

    public void RegisterAction(EHBOAction action)
    {
        if (playerActions.Contains(action))
        {
            Debug.Log($"{action} was al toegepast");
            return;
        }

        playerActions.Add(action);
        Debug.Log($"Actie toegevoegd: {action}");

        OnActionApplied?.Invoke(action);

        UpdateWoundSprite();
    }

    public void RemoveAction(EHBOAction action)
    {
        if (playerActions.Contains(action))
        {
            playerActions.Remove(action);
            Debug.Log($"Actie verwijderd: {action}");
            OnActionApplied?.Invoke(EHBOAction.None);
            UpdateWoundSprite();
        }
    }

    public void RemoveLastBandageAction()
    {
        for (int i = playerActions.Count - 1; i >= 0; i--)
        {
            if (playerActions[i] == EHBOAction.Gaas ||
                playerActions[i] == EHBOAction.Verband)
            {
                Debug.Log($"Verwijderd: {playerActions[i]}");
                playerActions.RemoveAt(i);
                OnActionApplied?.Invoke(EHBOAction.None);
                UpdateWoundSprite();
                return;
            }
        }

        Debug.Log("Geen gaas of verband om te verwijderen");
    }

    public void ResetActions()
    {
        playerActions.Clear();
        UpdateWoundSprite();
        Debug.Log("EHBO acties gereset");
    }

    // =========================
    // SCORE EVALUATION
    // =========================

    public (int stars, int points) GetCurrentScore()
    {
        if (playerActions.Count == 0)
        {
            return (0, 0);
        }

        return EvaluateActions();
    }

    public (int stars, int points) FinishTreatment()
    {
        if (playerActions.Count == 0)
        {
            return (0, 0);
        }

        var result = EvaluateActions();
        GameManager.Instance.AddScore(result.points);

        Debug.Log($"EHBO Resultaat → Punten: {result.points}, Sterren: {result.stars}");
        return result;
    }

    private (int stars, int points) EvaluateActions()
    {
        int points = 0;
        int stars = 1;

        bool didKoelen = playerActions.Contains(EHBOAction.Koelen);
        bool didGaas = playerActions.Contains(EHBOAction.Gaas);
        bool didVerband = playerActions.Contains(EHBOAction.Verband);

        if (playerActions.Count == 3 &&
            playerActions[0] == EHBOAction.Koelen &&
            playerActions[1] == EHBOAction.Gaas &&
            playerActions[2] == EHBOAction.Verband)
        {
            points = 300;
            stars = 3;
        }
        else if (playerActions.Count >= 2 &&
                 playerActions[0] == EHBOAction.Koelen &&
                 playerActions[1] == EHBOAction.Verband)
        {
            points = 250;
            stars = 2;
        }
        else if (playerActions.Count >= 2 &&
                 playerActions[0] == EHBOAction.Gaas &&
                 playerActions[1] == EHBOAction.Verband)
        {
            points = 150;
            stars = 2;
        }
        else if ((didGaas && !didVerband && !didKoelen) ||
                 (didVerband && !didGaas && !didKoelen))
        {
            points = 50;
            stars = 1;
        }
        else
        {
            points = 150;
            stars = 2;
        }

        return (stars, points);
    }

    // =========================
    // VISUAL UPDATE
    // =========================

    private void UpdateWoundSprite()
    {
        if (playerActions.Contains(EHBOAction.Verband))
        {
            SetSpriteForAll(verbandWound);
        }
        else if (playerActions.Contains(EHBOAction.Gaas))
        {
            SetSpriteForAll(gaasWound);
        }
        else if (playerActions.Contains(EHBOAction.Koelen))
        {
            SetSpriteForAll(cooledWound);
        }
        else
        {
            SetSpriteForAll(normalWound);
        }
    }

    private void SetSpriteForAll(Sprite sprite)
    {
        foreach (var renderer in woundRenderers)
        {
            if (renderer != null)
            {
                renderer.sprite = sprite;
            }
        }
    }
}
