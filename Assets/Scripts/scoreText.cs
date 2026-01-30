using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Update()
    {
        if (GameManager.Instance == null) return;

        scoreText.text = "Score: " + GameManager.Instance.Score;
    }
}
