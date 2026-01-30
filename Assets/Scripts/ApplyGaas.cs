using UnityEngine;
using UnityEngine.UI;

public class ApplyGaas : MonoBehaviour
{
    public BurnTreatmentEvaluator burnEvaluator;
    public Button button;

    public void Apply()
    {
        if (burnEvaluator.HasAction(EHBOAction.Gaas)) return;

        burnEvaluator.RegisterAction(EHBOAction.Gaas);
        button.interactable = false; // knop uitschakelen
    }
}
