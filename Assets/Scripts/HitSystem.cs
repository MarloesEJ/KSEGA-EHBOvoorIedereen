using UnityEngine;
using TMPro;

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

    public int hitsNeeded = 5;

    private int hitsDone = 0;
    private bool active = false;
    private int ignoreInputs = 0;

    void Start()
    {
        UpdateText();
    }

    void Update()
    {
        if (!active) return;

        if (ignoreInputs > 0)
        {
            ignoreInputs--;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryHit();
        }
    }

    public void ResetHits()
    {
        hitsDone = 0;
        active = true;
        ignoreInputs = 1;
        UpdateText();
    }

    void TryHit()
    {
        if (hitsDone >= hitsNeeded) return;

        if (IsInsidePerfectZone())
        {
            hitsDone++;
            UpdateText();

            if (hitsDone >= hitsNeeded)
            {
                Complete();
            }
        }
    }

    void Complete()
    {
        active = false;

        // Meisje blij
        if (girlRenderer != null && happySprite != null)
            girlRenderer.sprite = happySprite;

        // Tekst boven de bar
        if (hitText != null)
            hitText.text = "Goed gedaan!";

        // Bar + arm verbergen
        if (timingBarObject != null)
            timingBarObject.SetActive(false);

        if (armUIObject != null)
            armUIObject.SetActive(false);

        // Speech bubble tonen + tekst veranderen
        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(true);

        if (speechBubbleText != null)
            speechBubbleText.text = "\"Dank je! Ik kan weer ademen\"";
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

        hitText.text = "Nog " + left + " slagen te gaan";
    }
}
