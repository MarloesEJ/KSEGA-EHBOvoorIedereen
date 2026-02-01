using UnityEngine;
using UnityEngine.UI;

public class ApplyVerband : MonoBehaviour
{
    public BurnTreatmentEvaluator burnEvaluator;
    public Button button;

    public void Apply()
    {
        if (burnEvaluator.HasAction(EHBOAction.Verband)) return;

        burnEvaluator.RegisterAction(EHBOAction.Verband);
        button.interactable = false;
    }
}
