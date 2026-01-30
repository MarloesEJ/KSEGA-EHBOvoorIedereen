using UnityEngine;
using UnityEngine.UI;

public class PrecisionPourGame : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform playerBar;
    public RectTransform safeZone;
    public Image woundImage;
    public GameObject minigameUI;
    public GameObject startButton;
    public GameObject gameGuide;
    public GameObject endText;
    public BurnTreatmentEvaluator burnEvaluator;


    [Header("Settings")]
    public float acceleration = 2000f;     // Hoeveel kracht input geeft
    public float maxSpeed = 1000f;         // Max snelheid balk
    public float friction = 0.95f;         // Hoeveel hij afremt per frame
    public float shrinkSpeed = 0.5f;       // Hoe snel wond kleiner wordt
    public float requiredTimeInSafe = 20f; // Tijd in groene zone om te winnen
    public float defaultSpeed = 300f;      // Standaard snelheid in laatst gekozen richting

    private float velocity = 0f;           // Huidige snelheid balk
    private bool gameActive = false;
    private float timeInSafe = 0f;
    private int lastDirection = 1;         // 1 = rechts, -1 = links

    void Start()
    {
        minigameUI.SetActive(false);
        startButton.SetActive(true);
        gameGuide.SetActive(false);
    }

    void Update()
    {
        if (!gameActive) return;

        // Input: -1 = links, 1 = rechts, 0 = niks
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
        {
            // Update lastDirection
            lastDirection = (int)Mathf.Sign(input);

            // Voeg acceleratie toe
            velocity += input * acceleration * Time.deltaTime;

            // Clamp
            velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
        }
        else
        {
            // Geen input → blijf bewegen in laatst gekozen richting met defaultSpeed
            if (Mathf.Abs(velocity) < defaultSpeed)
            {
                velocity = defaultSpeed * lastDirection;
            }
        }

        // Update positie
        float newX = playerBar.anchoredPosition.x + velocity * Time.deltaTime;

        // Limieten balk
        float halfWidth = playerBar.rect.width / 2;
        float parentWidth = safeZone.parent.GetComponent<RectTransform>().rect.width / 2;
        newX = Mathf.Clamp(newX, -parentWidth + halfWidth, parentWidth - halfWidth);

        playerBar.anchoredPosition = new Vector2(newX, playerBar.anchoredPosition.y);

        // Frictie
        velocity *= friction;

        // Safe zone check
        if (IsInSafeZone())
        {
            timeInSafe += Time.deltaTime;

            if (woundImage.fillAmount > 0f)
                woundImage.fillAmount -= shrinkSpeed * Time.deltaTime;
        }

        // Win conditie
        if (timeInSafe >= requiredTimeInSafe)
        {
            gameActive = false;

            // REGISTREER JUISTE EHBO-STAP
            burnEvaluator.RegisterAction(EHBOAction.Koelen);

            Debug.Log("Koelen succesvol uitgevoerd!");
            EndGame();
        }
    }

    bool IsInSafeZone()
    {
        float barLeft = playerBar.anchoredPosition.x - playerBar.rect.width / 2;
        float barRight = playerBar.anchoredPosition.x + playerBar.rect.width / 2;

        float zoneLeft = safeZone.anchoredPosition.x - safeZone.rect.width / 2;
        float zoneRight = safeZone.anchoredPosition.x + safeZone.rect.width / 2;

        return barLeft >= zoneLeft && barRight <= zoneRight;
    }

    public void StartGame()
    {
        startButton.SetActive(false);
        minigameUI.SetActive(true);
        gameGuide.SetActive(true);
        endText.SetActive(false);

        gameActive = true;
        velocity = defaultSpeed;  // Start met standaard snelheid
        timeInSafe = 0f;
        woundImage.fillAmount = 1f;

        // Willekeurige startpositie
        float halfWidth = playerBar.rect.width / 2;
        float parentWidth = safeZone.parent.GetComponent<RectTransform>().rect.width;
        float startX = Random.Range(-parentWidth / 2 + halfWidth, parentWidth / 2 - halfWidth);
        playerBar.anchoredPosition = new Vector2(startX, playerBar.anchoredPosition.y);

        // Willekeurige start richting
        lastDirection = Random.value < 0.5f ? -1 : 1;
    }

    private void EndGame()
    {
        minigameUI.SetActive(false);
        startButton.SetActive(false);
        gameGuide.SetActive(false);
        endText.SetActive(true);
    }

}
