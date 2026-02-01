using UnityEngine;
using UnityEngine.UI;

public class SchaafWondManager : MonoBehaviour
{
    // public bool hasGloves = false;
    public bool hasGauze = false;
    public bool gauzeIsWet = false;
    // public bool hasDisinfectant = false;

    public GameObject GameScene;
    public GameObject KraanScene;
    public GameObject GameGame;

    public enum GameStepp{
        Start, Gloves, GetGauze, WetGauze, CleanWound, Disinfectant, PutPleister, Finished
    }
    public GameStepp currentStep = GameStepp.Start;

    [Header("UI Elements")]
    public DialogueUI dialogueScript;
    public GameObject beginButton;
    public GameObject startGameButton;
    public GameObject finishButton;
    public GameObject GotoButton;
    public GameObject Table;

    [Header("Objects")]
    public EHBOOverlayToggle ehboOverlayToggle;
    public GameObject pleisterObject;
    public GameObject Kind;
    public GameObject childWithWound;
    public GameObject childWithBandage;

    public int points = 0;

    void Start(){
        UpdateText();
        GameScene.SetActive(true);
        KraanScene.SetActive(false);
        GameGame.SetActive(false);
        Kind.SetActive(true);
        Table.SetActive(true);
    }

    void Update(){
        if (currentStep != GameStepp.Start){
            beginButton.SetActive(false);
        }
    }

    public void PutOnGloves()
    {
        if(currentStep == GameStepp.Gloves)
        {
            points += 50;
            // hasGloves = true;
            NextStep();
        }
        else if(currentStep > GameStepp.Gloves)
        {
            points -= 25;
        }
        else{
            currentStep = GameStepp.Gloves;
            NextStep();
        }
    }

    public void PickUpGauze(){
        if(currentStep == GameStepp.GetGauze)
        {
            points += 50;
            hasGauze = true;
            NextStep();
        }
        else if(currentStep > GameStepp.GetGauze)
        {
            points -= 25;
        }
        else{
            currentStep = GameStepp.GetGauze;
            NextStep();
        }
    }

    public void WetGauze(){
        if(currentStep == GameStepp.WetGauze)
        {
            points += 50;
            gauzeIsWet = true;
            NextStep();
        }
        else if(currentStep > GameStepp.WetGauze)
        {
            points -= 25;
        }
        else if(!hasGauze){
            dialogueScript.startingText = "Je moet eerst gaas pakken voordat je het nat kan maken.";
            dialogueScript.SetText(dialogueScript.startingText);
        }
    }

    public void UseDisinfectant(){
        if(currentStep == GameStepp.Disinfectant)
        {
            points += 50;
            // hasDisinfectant = true;
            NextStep();
        }
        else if(currentStep > GameStepp.Disinfectant)
        {
            points -= 25;
        }
        else{
            currentStep = GameStepp.Disinfectant;
            NextStep();
        }
    }

    public void PickUpBandage(){
        if(currentStep == GameStepp.PutPleister)
        {
            points += 50;
            NextStep();
        }
        else if(currentStep > GameStepp.PutPleister)
        {
            points -= 25;
        }
        else{
            currentStep = GameStepp.PutPleister;
            NextStep();
        }
    }

    public void GoToFaucet(){
        GameScene.SetActive(false);
        KraanScene.SetActive(true);
    }

    public void ReturnFromFaucet(){
        KraanScene.SetActive(false);
        GameScene.SetActive(true);
    }

    public void StartGame(){
        Debug.Log("Game Started");
        Table.SetActive(false);
        Kind.SetActive(false);
        startGameButton.SetActive(false);
        GotoButton.SetActive(false);
        GameGame.SetActive(true);
    }

    public void FinishGame(){
        points += 50;
        Table.SetActive(true);
        Kind.SetActive(true);
        GameGame.SetActive(false);
        currentStep = GameStepp.CleanWound;
        NextStep();
    }

    public void NextStep(){
        switch(currentStep){
            case GameStepp.Start:
                beginButton.SetActive(false);
                currentStep = GameStepp.Gloves;
                break;
            case GameStepp.Gloves:
                currentStep = GameStepp.GetGauze;
                break;
            case GameStepp.GetGauze:
                currentStep = GameStepp.WetGauze;
                ehboOverlayToggle.ToggleEHBO();
                GotoButton.SetActive(true);
                break;
            case GameStepp.WetGauze:
                startGameButton.SetActive(true);
                currentStep = GameStepp.CleanWound;
                break;
            case GameStepp.CleanWound:
                currentStep = GameStepp.Disinfectant;
                break;
            case GameStepp.Disinfectant:
                ehboOverlayToggle.ToggleEHBO();
                currentStep = GameStepp.PutPleister;
                break;
            case GameStepp.PutPleister:
                currentStep = GameStepp.Finished;
                finishButton.SetActive(true);
                childWithWound.SetActive(false);
                childWithBandage.SetActive(true);
                ehboOverlayToggle.ToggleEHBO();
                startGameButton.SetActive(false);
                break;
            case GameStepp.Finished:
                break;
        }
        UpdateText();
    }

    public void UpdateText(){
        switch(currentStep){
            case GameStepp.Start:
                dialogueScript.startingText = "Ik ben gevallen en heb nu een pijnlijke knee. Mama zegt dat deze schoongemaakt moet worden.";
                break;
            case GameStepp.Gloves:
                dialogueScript.startingText = "Doe handschoenen aan voordat je begint met de wondverzorging.";
                break;
            case GameStepp.GetGauze:
                dialogueScript.startingText = "Pak wat gaas om de wond schoon te maken.";
                break;
            case GameStepp.WetGauze:
                dialogueScript.startingText = "Maak het gaas nat onder de kraan.";
                break;
            case GameStepp.CleanWound:
                dialogueScript.startingText = "Maak de wond schoon met het natte gaas.";
                break;
            case GameStepp.Disinfectant:
                dialogueScript.startingText = "Desinfecteer de wond met Jodium.";
                break;
            case GameStepp.PutPleister:
                dialogueScript.startingText = "Doe een pleister op de wond.";
                break;
            case GameStepp.Finished:
                dialogueScript.startingText = "Je hebt de wond succesvol verzorgd!";
                break;
        }
        dialogueScript.SetText(dialogueScript.startingText);
    }
}
