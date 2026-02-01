using UnityEngine;

public class ClickShowBack : MonoBehaviour
{
    [Header("Back Sprite")]
    public Sprite backSprite;

    [Header("Hide Objects")]
    public GameObject bureauObject;

    [Header("Front Position")]
    public Transform frontPosition; // waar de rug moet komen

    [Header("Back View Scale")]
    public Vector3 backScale = new Vector3(1.8f, 1.8f, 1.8f);

    [Header("Click Settings")]
    public bool canClick = false;

    private SpriteRenderer sr;
    private bool clicked = false;

    public GameObject timingBarObject;
    public GameObject armUIObject;
    public GameObject hitTextObject;
    public HitSystem hitSystem;



    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void EnableClick()
    {
        canClick = true;
    }

    void OnMouseDown()
    {
        if (!canClick) return;
        if (clicked) return;
        clicked = true;

        // rug sprite
        if (backSprite != null)
            sr.sprite = backSprite;

        // bureau weg
        if (bureauObject != null)
            bureauObject.SetActive(false);

        // naar rug-positie
        if (frontPosition != null)
            transform.position = frontPosition.position;

        // rug groter maken
        transform.localScale = backScale;

        if (timingBarObject != null)
            timingBarObject.SetActive(true);

        if (armUIObject != null)
            armUIObject.SetActive(true);

        if (hitTextObject != null)
            hitTextObject.SetActive(true);

        if (hitSystem != null)
            hitSystem.ResetHits();


    }
}
