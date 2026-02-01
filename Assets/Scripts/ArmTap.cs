using UnityEngine;
using System.Collections;

public class ArmTap : MonoBehaviour
{
    [Header("Tap instellingen")]
    public Vector2 tapOffset = new Vector2(60f, -10f); // richting rug
    public float tapDuration = 0.08f;                  // heen
    public float returnDuration = 0.10f;               // terug

    [Header("Input")]
    public bool allowTap = true;                       // kan aan/uit als je wil
    private int ignoreInputs = 0;                      // negeer 1e klik na enable

    private RectTransform rt;
    private Vector2 startPos;
    private bool isTapping;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
    }

    void OnEnable()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;

        //negeer de klik die ArmUI net active maakt
        ignoreInputs = 1;
    }

    void Update()
    {
        if (!allowTap) return;

        // eerst input negeren
        if (ignoreInputs > 0)
        {
            ignoreInputs--;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Tap();
        }
    }

    public void Tap()
    {
        if (isTapping) return;
        StartCoroutine(TapRoutine());
    }

    IEnumerator TapRoutine()
    {
        isTapping = true;

        Vector2 hitPos = startPos + tapOffset;

        // heen
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / tapDuration;
            rt.anchoredPosition = Vector2.Lerp(startPos, hitPos, t);
            yield return null;
        }

        // terug
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / returnDuration;
            rt.anchoredPosition = Vector2.Lerp(hitPos, startPos, t);
            yield return null;
        }

        rt.anchoredPosition = startPos;
        isTapping = false;
    }
}
