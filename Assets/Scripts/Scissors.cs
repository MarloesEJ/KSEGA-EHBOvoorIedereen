using UnityEngine;
using UnityEngine.UI;

public class Scissors : MonoBehaviour
{
    public BurnTreatmentEvaluator burnEvaluator;
    public Button gaasButton;
    public Button verbandButton;

    public void CutOff()
    {
        // Alleen gaas en verband verwijderen
        burnEvaluator.RemoveAction(EHBOAction.Gaas);
        burnEvaluator.RemoveAction(EHBOAction.Verband);

        // Knoppen weer bruikbaar maken
        gaasButton.interactable = true;
        verbandButton.interactable = true;

        Debug.Log("Gaas en verband verwijderd met schaar");
    }
}
