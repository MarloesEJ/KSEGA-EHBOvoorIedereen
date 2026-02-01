using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitSystem : MonoBehaviour
{
    public RectTransform indicator;
    public RectTransform perfectZone;
    public TMP_Text hitText;

    [Header("Meisje")]
    public SpriteRenderer girlRenderer;
    public Sprite happySprite;

    [Header("UI Objects")]
    public GameObject timingBarObject;
    public GameObject armUIObject;

    [Header("Speech Bubble")]
    public GameObject speechBubbleObject;
    public TMP_Text speechBubbleText;

    [Header("Feedback")]
    public TMP_Text feedbackText;
    public float feedbackShowTime = 1.5f;

    [Header("Speed Feedback")]
    public float slowLimit = 2.5f; // seconden zonder input voordat "Wees sneller!" komt

    [Header("Scoring")]
    public int maxScore = 300;
    public int startScore = 300;
    public int pointsPerfectHit = 10;   // + bij perfect hit (max 300)
    public int pointsWrongHit = 20;     // - bij miss
    public int pointsTooSlow = 10;      // - bij te traag

    [Header("Finish")]
    [SerializeField] public GameObject resultPanel;
    [SerializeField] public Image[] stars; // 3 sterren
    [SerializeField] public Sprite inactiveStar; // grijze ster
    [SerializeField] public Sprite activeStar;   // gele ster
    [SerializeField] public TextMeshProUGUI scoreText;

    [Header("Result Animation")]
    public float scoreCountDuration = 1.2f; // hoe lang 0 -> eindscore telt

    public int hitsNeeded = 5;

    private int hitsDone = 0;
    private bool active = false;
    private int ignoreInputs = 0;

    private int wrongHits = 0;
    private float slowTimer = 0f;
    private int score;

    void Start()
    {
        score = startScore;
        UpdateText();

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    void Update()
    {
        if (!active) return;

        // te langzaam -> feedback + punten eraf
        slowTimer += Time.deltaTime;
        if (slowTimer >= slowLimit)
        {
            ShowFeedback("Wees sneller!");
            LosePoints(pointsTooSlow);
            slowTimer = 0f;
        }

        if (ignoreInputs > 0)
        {
            ignoreInputs--;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            slowTimer = 0f;
            TryHit();
        }
    }

    public void ResetHits()
    {
        StopAllCoroutines();

        hitsDone = 0;
        wrongHits = 0;
        slowTimer = 0f;

        score = startScore;

        active = true;
        ignoreInputs = 1;

        if (timingBarObject != null)
            timingBarObject.SetActive(true);

        if (armUIObject != null)
            armUIObject.SetActive(true);

        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);

        UpdateText();
    }

    void TryHit()
    {
        if (hitsDone >= hitsNeeded) return;

        if (IsInsidePerfectZone())
        {
            hitsDone++;
            wrongHits = 0;

            AddPoints(pointsPerfectHit);

            UpdateText();

            if (hitsDone >= hitsNeeded)
            {
                Complete();
            }
        }
        else
        {
            wrongHits++;
            LosePoints(pointsWrongHit);

            if (wrongHits >= 2)
            {
                ShowFeedback("Sla op het juiste moment!");
                wrongHits = 0;
            }
        }
    }

    void Complete()
    {
        active = false;

        // Meisje blij
        if (girlRenderer != null)
            girlRenderer.sprite = null;

        // "Goed gedaan!" tonen
        if (hitText != null)
            hitText.text = "Goed gedaan!";

        // Bar + arm verbergen
        if (timingBarObject != null)
            timingBarObject.SetActive(false);

        if (armUIObject != null)
            armUIObject.SetActive(false);

        // Speech bubble tonen + tekst
        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);

        // feedback weg
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        int activeStars = (int)Math.Round(score / 100.0);

        for (int i = 0; i < stars.Count(); i++)
        {
            stars[i].sprite = (i < activeStars) ? activeStar : inactiveStar;
        }

        // Stop game
        GameManager.Instance?.SetState(GameState.Win);

        GameManager.Instance?.AddScore(score);

        // Score tonen
        scoreText.text = $"Punten: {GameManager.Instance?.Score}";

        // UI aanpassen
        resultPanel.SetActive(true);
    }

    bool IsInsidePerfectZone()
    {
        float indX = indicator.position.x;

        float left = perfectZone.position.x -
            (perfectZone.rect.width * perfectZone.lossyScale.x) / 2f;

        float right = perfectZone.position.x +
            (perfectZone.rect.width * perfectZone.lossyScale.x) / 2f;

        return indX >= left && indX <= right;
    }

    void UpdateText()
    {
        int left = hitsNeeded - hitsDone;
        if (left < 0) left = 0;

        if (hitText != null)
            hitText.text = "Nog " + left + " slagen te gaan";
    }

    void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), feedbackShowTime);
    }

    void HideFeedback()
    {
        if (feedbackText == null) return;
        feedbackText.gameObject.SetActive(false);
    }

    void AddPoints(int amount)
    {
        score += amount;
        if (score > maxScore) score = maxScore;
    }

    void LosePoints(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;
    }
}
