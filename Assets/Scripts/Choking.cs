using UnityEngine;
using System.Collections;

public class ChokingSequence : MonoBehaviour
{
    [Header("Sprites in volgorde")]
    public Sprite eatingSprite;
    public Sprite chokingSprite;
    public Sprite chokingSprite2;

    [Header("Timing")]
    public float eatTime = 2f;
    public float chokeTime = 2f;

    [Header("Move Closer")]
    public Transform moveTarget;
    public float moveSpeed = 2.5f;
    public float stopDistance = 0.01f;

    [Header("Scaling")]
    public Vector3 startScale = new Vector3(1f, 1f, 1f);
    public Vector3 endScale = new Vector3(1.6f, 1.6f, 1.6f);

    [Header("Speech Bubble")]
    public GameObject speechBubble;

    //link naar click script
    [Header("Click Interaction")]
    public ClickShowBack clickShowBack;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("GEEN SpriteRenderer gevonden!");
        }

        transform.localScale = startScale;

        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // --- ETEN ---
        if (eatingSprite != null)
            sr.sprite = eatingSprite;

        yield return new WaitForSeconds(eatTime);

        // --- VERSLIKKEN ---
        if (chokingSprite != null)
            sr.sprite = chokingSprite;

        yield return new WaitForSeconds(chokeTime);

        // --- VERSLIKKEN 2 ---
        if (chokingSprite2 != null)
            sr.sprite = chokingSprite2;

        // --- NAAR VOREN LOPEN ---
        if (moveTarget != null)
            yield return StartCoroutine(MoveToTarget());

        // --- SPEECH BUBBLE TONEN ---
        if (speechBubble != null)
            speechBubble.SetActive(true);

      
        if (clickShowBack != null)
            clickShowBack.EnableClick();
    }

    IEnumerator MoveToTarget()
    {
        Vector3 startPos = transform.position;
        float totalDistance = Vector3.Distance(startPos, moveTarget.position);

        if (totalDistance <= 0.001f)
            yield break;

        while (Vector3.Distance(transform.position, moveTarget.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                moveTarget.position,
                moveSpeed * Time.deltaTime
            );

            float movedDistance = Vector3.Distance(startPos, transform.position);
            float t = movedDistance / totalDistance;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        transform.localScale = endScale;
    }
}
