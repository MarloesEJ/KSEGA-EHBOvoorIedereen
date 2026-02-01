using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class FinishManager : MonoBehaviour
{
    [Header("Finish")]
    [SerializeField] public Image[] stars; // 3 sterren
    [SerializeField] public Sprite inactiveStar; // grijze ster
    [SerializeField] public Sprite activeStar;   // gele ster
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public int maxPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int score = GameManager.Instance?.Score ?? 0;
        int pointsPerStar = maxPoints / stars.Count();
        int activeStars = Mathf.Clamp(score / pointsPerStar, 0, stars.Count());

        for (int i = 0; i < stars.Count(); i++)
        {
            stars[i].sprite = (i < activeStars) ? activeStar : inactiveStar;
        }

        // Stop game
        GameManager.Instance?.SetState(GameState.Win);

        // Score tonen
        scoreText.text = $"Punten: {GameManager.Instance?.Score}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
