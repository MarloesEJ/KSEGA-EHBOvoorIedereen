using UnityEngine;

public class EHBOOverlayToggle : MonoBehaviour
{
    public GameObject ehboOverlay;

    public void ToggleEHBO()
    {
        ehboOverlay.SetActive(!ehboOverlay.activeSelf);
    }
}
