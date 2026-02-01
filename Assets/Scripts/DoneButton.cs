using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        // Sterren resetten
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = inactiveStar;
        }

        // Actieve sterren instellen
        for (int i = 0; i < result.stars && i < stars.Length; i++)
        {
            stars[i].sprite = activeStar;
        }

        // Score tonen
        scoreText.text = $"Punten: {result.points}";

        // UI aanpassen
        resultPanel.SetActive(true);
        ehboDoos.SetActive(false);
        gameObject.SetActive(false); // Done knop zelf weg
    }
}
