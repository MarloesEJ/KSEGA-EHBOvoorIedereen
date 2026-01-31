using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Result UI")]
    public GameObject resultsPanel;     // panel met trofeeën + score
    public TMP_Text scoreText;          // "Score: 240"
    public GameObject[] trophyIcons;    // 3 trofee GameObjects in volgorde 1..3
    public float resultDelay = 1f;      // 1 sec wachten na "Goed gedaan!"

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
        UpdateScoreUI();
        UpdateTrophies();

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (resultsPanel != null)
            resultsPanel.SetActive(false);
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
        UpdateScoreUI();
        UpdateTrophies();

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

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        // optioneel: tijdens reset ook trofeeën meteen uit als resultpanel aan/uit issues geeft
        if (trophyIcons != null)
        {
            for (int i = 0; i < trophyIcons.Length; i++)
                if (trophyIcons[i] != null)
                    trophyIcons[i].SetActive(false);
        }

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
        if (girlRenderer != null && happySprite != null)
            girlRenderer.sprite = happySprite;

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
            speechBubbleObject.SetActive(true);

        if (speechBubbleText != null)
            speechBubbleText.text = "\"Dank je! Ik kan weer ademen\"";

        // feedback weg
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        // na 1 sec result panel tonen + score animatie
        StartCoroutine(ShowResultsAfterDelay());
    }

    IEnumerator ShowResultsAfterDelay()
    {
        yield return new WaitForSeconds(resultDelay);

        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        // Start animatie van score + trofeeën één voor één
        yield return StartCoroutine(AnimateResults());
    }

    IEnumerator AnimateResults()
    {
        int target = score;
        int current = 0;

        // trofeeën eerst uit
        if (trophyIcons != null)
        {
            for (int i = 0; i < trophyIcons.Length; i++)
                if (trophyIcons[i] != null)
                    trophyIcons[i].SetActive(false);
        }

        // score start op 0
        if (scoreText != null)
            scoreText.text = "Score: 0";

        float duration = Mathf.Max(0.1f, scoreCountDuration);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            int newScore = Mathf.RoundToInt(Mathf.Lerp(0, target, p));

            if (newScore != current)
            {
                current = newScore;

                if (scoreText != null)
                    scoreText.text = "Score: " + current;

                // trofeeën één voor één bij thresholds
                if (trophyIcons != null)
                {
                    if (current >= 100 && trophyIcons.Length > 0 && trophyIcons[0] != null)
                        trophyIcons[0].SetActive(true);

                    if (current >= 200 && trophyIcons.Length > 1 && trophyIcons[1] != null)
                        trophyIcons[1].SetActive(true);

                    if (current >= 300 && trophyIcons.Length > 2 && trophyIcons[2] != null)
                        trophyIcons[2].SetActive(true);
                }
            }

            yield return null;
        }

        // final clamp (exact eindscore)
        if (scoreText != null)
            scoreText.text = "Score: " + target;

        if (trophyIcons != null)
        {
            if (target >= 100 && trophyIcons.Length > 0 && trophyIcons[0] != null) trophyIcons[0].SetActive(true);
            if (target >= 200 && trophyIcons.Length > 1 && trophyIcons[1] != null) trophyIcons[1].SetActive(true);
            if (target >= 300 && trophyIcons.Length > 2 && trophyIcons[2] != null) trophyIcons[2].SetActive(true);
        }
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

        UpdateScoreUI();
        UpdateTrophies();
    }

    void LosePoints(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;

        UpdateScoreUI();
        UpdateTrophies();
    }

    void UpdateScoreUI()
    {
        // Tijdens het spelen mag score live updaten (als je dat wil)
        // Op results scherm wordt dit overschreven door AnimateResults()
        if (scoreText != null && (resultsPanel == null || !resultsPanel.activeSelf))
            scoreText.text = "Score: " + score;
    }

    void UpdateTrophies()
    {
        // Tijdens het spelen: optioneel.
        // Als je pas trofeeën wil tonen op het eind, kan je deze functie leeg laten.
        if (trophyIcons == null || trophyIcons.Length == 0) return;

        int trophies = 0;
        if (score >= 100) trophies = 1;
        if (score >= 200) trophies = 2;
        if (score >= 300) trophies = 3;

        // Alleen updaten als resultsPanel NIET actief is (anders doet AnimateResults het)
        if (resultsPanel != null && resultsPanel.activeSelf) return;

        for (int i = 0; i < trophyIcons.Length; i++)
        {
            if (trophyIcons[i] != null)
                trophyIcons[i].SetActive(i < trophies);
        }
    }
}
