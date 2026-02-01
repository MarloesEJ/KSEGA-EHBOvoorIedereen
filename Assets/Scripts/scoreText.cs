using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public BurnTreatmentEvaluator evaluator;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        var result = evaluator.GetCurrentScore();
        scoreText.text = $"Score: {result.points}";
    }
}
