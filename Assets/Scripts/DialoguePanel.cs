using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Sleep hier je TMP tekst in
    public float typeSpeed = 0.05f;      // Tijd tussen letters
    public string startingText = "Au… mijn hand doet pijn! Ik heb per ongeluk een hete pan aangeraakt… Mijn mama zegt dat je het moet koelen…"; // Tekst die automatisch wordt getoond

    private Coroutine typingCoroutine;

    void Start()
    {
        dialogueText.text = ""; // start leeg
        Show();                 // toon de balk
        SetText(startingText);  // start typewriter automatisch
    }

    // Zet nieuwe tekst met typewriter effect
    public void SetText(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    // Laat dialoogbalk zien
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // Verberg dialoogbalk
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Optioneel: stop typewriter en laat volledige tekst zien
    public void SkipTyping(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = message;
    }

    public void ShowActionText(EHBOAction action)
    {
        string message = "";

        switch (action)
        {
            case EHBOAction.Koelen:
                message = "Het is nu nat en koeler maar doet gelukkig minder pijn. ";
                break;
            case EHBOAction.Gaas:
                message = "oohhh we doen gaas op mijn wond? Dat zorgt vast voor bescherming!";
                break;
            case EHBOAction.Verband:
                message = "Het doet nu geen pijn meer! En kan er niet meer aan zitten. Dankjewel voor het helpen!";
                break;
            case EHBOAction.None:
                message = "Je hebt het gaas en verband weg gehaald? Wat nu?";
                break;

        }

        SetText(message);
    }

}
