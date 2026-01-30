using System.Collections.Generic;
using UnityEngine;

public class BurnTreatmentEvaluator : MonoBehaviour
{
    private List<EHBOAction> correctOrder = new List<EHBOAction>
    {
        EHBOAction.Koelen,
        EHBOAction.Gaas,
        EHBOAction.Verband
    };

    private List<EHBOAction> playerActions = new List<EHBOAction>();

    public bool HasAction(EHBOAction action)
    {
        return playerActions.Contains(action);
    }

    public void RegisterAction(EHBOAction action)
    {
        if (!playerActions.Contains(action))
        {
            playerActions.Add(action);
            Debug.Log($"Actie toegevoegd: {action}");
        }
        else
        {
            Debug.Log($"{action} was al toegepast");
        }
    }

    public void ResetActions()
    {
        playerActions.Clear();
        Debug.Log("EHBO acties gereset");
    }

    public (int stars, int points) FinishTreatment()
    {
        int correctSteps = 0;

        for (int i = 0; i < playerActions.Count && i < correctOrder.Count; i++)
        {
            if (playerActions[i] == correctOrder[i])
                correctSteps++;
            else
                break;
        }

        int stars = 1;
        int points = 0;

        if (correctSteps == 3)
        {
            stars = 3;
            points = 300;
        }
        else if (correctSteps == 2)
        {
            stars = 2;
            points = 150;
        }
        else
        {
            stars = 1;
            points = 50;
        }

        GameManager.Instance.AddScore(points);

        return (stars, points);
    }

}
