using UnityEngine;
using UnityEngine.UI;

public class Scissors : MonoBehaviour
{
    public BurnTreatmentEvaluator burnEvaluator;
    public Button gaasButton;
    public Button verbandButton;

    public void CutOff()
    {
        burnEvaluator.ResetActions();

        gaasButton.interactable = true;
        verbandButton.interactable = true;

        Debug.Log("Alles verwijderd met schaar");
    }
}
