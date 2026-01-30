using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoneButton : MonoBehaviour
{
    public BurnTreatmentEvaluator burnEvaluator;

    [Header("Result UI")]
    public GameObject resultPanel;
    public Image[] stars; // 3 sterren
    public Sprite inactiveStar; // grijze ster
    public Sprite activeStar;   // gele ster
    public TextMeshProUGUI scoreText;

    [Header("Disable On Finish")]
    public GameObject ehboDoos;

    public void Finish()
    {
        Debug.Log("Done pressed!");

        // Stop game
        GameManager.Instance.SetState(GameState.Win);

        // Resultaat ophalen
        var result = burnEvaluator.FinishTreatment();

        // Sterren resetten naar inactive sprite
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = inactiveStar;
        }

        // Juiste aantal sterren instellen op active sprite
        for (int i = 0; i < result.stars; i++)
        {
            stars[i].sprite = activeStar;
        }

        // Score tonen
        scoreText.text = $"Punten: {result.points}";

        // UI aanpassen
        resultPanel.SetActive(true);
        ehboDoos.SetActive(false);
    }
}
