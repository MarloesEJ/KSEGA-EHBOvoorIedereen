using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoneButton : MonoBehaviour
{
    public BurnTreatmentEvaluator burnEvaluator;

    [Header("Result UI")]
    public GameObject resultPanel;
    public Image[] stars;             // 3 sterren (Image)
    public Sprite inactiveStar;       // grijze ster
    public Sprite activeStar;         // gele ster
    public TextMeshProUGUI scoreText;

    [Header("Disable On Finish")]
    public GameObject ehboDoos;

    public void Finish()
    {
        Debug.Log("Done pressed!");

        // Game stoppen
        GameManager.Instance.SetState(GameState.Win);

        // 👉 HUIDIGE score ophalen (live evaluatie)
        var result = burnEvaluator.GetCurrentScore();

        // 👉 Score NU pas toevoegen aan GameManager
        GameManager.Instance.AddScore(result.points);

        int activeStars = (int)Math.Round(result.points / 100.0);

        for (int i = 0; i < stars.Count(); i++)
        {
            stars[i].sprite = (i < activeStars) ? activeStar : inactiveStar;
        }

        // Score tonen
        scoreText.text = $"Punten: {GameManager.Instance.Score}";

        // UI aanpassen
        resultPanel.SetActive(true);
        //ehboDoos.SetActive(false);
        //gameObject.SetActive(false); // Done knop zelf weg
    }

    public void NextLevel()
    {
        GameManager.Instance?.LoadLevelByIndex(2);
    }
}
