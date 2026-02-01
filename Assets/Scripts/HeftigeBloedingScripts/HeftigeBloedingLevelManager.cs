using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeftigeBloedingLevelManager : MonoBehaviour
{
    public GameObject GameScene;
    public GameObject IntroScene;
    
    public enum GameStep{Gloves, GetGauze, PressureGame, GetBandage, Done};
    public GameStep currentStep = GameStep.Gloves;
    
    [Header("Dialog")]
    public DialogueUI dialogueScript;
    public GameObject finishButton;
    public GameObject startButton;

    [Header("Game Objects")]
    public EHBOOverlayToggle EHBOOverlayToggle;
    public GameObject glovesBox;
    public GameObject gauzeOnArm;
    public GameObject woundedArm;
    public GameObject bandageOnArm;
    public GameObject pressureGameParent;

    [Header("Man states")]
    public GameObject manWithWound;
    public GameObject manWithBandage;

    [Header("Finish")]
    [SerializeField] public GameObject resultPanel;
    [SerializeField] public Image[] stars; // 3 sterren
    [SerializeField] public Sprite inactiveStar; // grijze ster
    [SerializeField] public Sprite activeStar;   // gele ster
    [SerializeField] public TextMeshProUGUI scoreText;

    public int points = 0;

    void Start(){
        GameScene.SetActive(false);
        pressureGameParent.SetActive(false);
        finishButton.SetActive(false);
    }

    public void startGame()
    {
        IntroScene.SetActive(false);
        startButton.SetActive(false);
        GameScene.SetActive(true);
        currentStep = GameStep.Gloves;
        UpdateUI();
    }

    public void NextStep(){
        switch(currentStep){
            case GameStep.Gloves:
                currentStep = GameStep.GetGauze;
                break;
            case GameStep.GetGauze:
                gauzeOnArm.SetActive(true);
                EHBOOverlayToggle.ToggleEHBO();
                currentStep = GameStep.PressureGame;
                pressureGameParent.SetActive(true);
                break;
            case GameStep.PressureGame:
                pressureGameParent.SetActive(false);
                currentStep = GameStep.GetBandage;
                break;
            case GameStep.GetBandage:
                woundedArm.SetActive(false);
                bandageOnArm.SetActive(true);
                EHBOOverlayToggle.ToggleEHBO();
                currentStep = GameStep.Done;
                break;
        }
        UpdateUI();
    }

    void UpdateUI(){
        switch(currentStep){
            case GameStep.Gloves:
                dialogueScript.startingText = "Doe handschoenen aan voordat je de wond behandelt.";
                break;
            case GameStep.GetGauze:
                dialogueScript.startingText = "Eerst moet er een gaasje op de wond geplaatst worden.";
                break;
            case GameStep.PressureGame:
                dialogueScript.startingText = "Er moet druk op de wond worden uitgeoefend om het bloeden te stoppen.";
                break;
            case GameStep.GetBandage:
                dialogueScript.startingText = "Pak nu een verband om het gaasje vast te zetten.";
                break;
            case GameStep.Done:
                dialogueScript.startingText = "Nu kan meneer rustig naar de docter toe.";
                finishButton.SetActive(true);
                break;
        }
        dialogueScript.SetText(dialogueScript.startingText);
    }

    public void FinishGame(){

        manWithWound.SetActive(false);
        manWithBandage.SetActive(true);

        resultPanel.SetActive(true);

        int activeStars = (int)Math.Round(points / 100.0);

        for (int i = 0; i < stars.Count(); i++)
        {
            stars[i].sprite = (i < activeStars) ? activeStar : inactiveStar;
        }

        // Stop game
        GameManager.Instance?.SetState(GameState.Win);

        GameManager.Instance?.AddScore(points);

        // Score tonen
        scoreText.text = $"Punten: {GameManager.Instance?.Score}";

        // UI aanpassen
        resultPanel.SetActive(true);
    }

    public void GlovesOnHands(){

        if(currentStep == GameStep.Gloves){
            points += 100;
            NextStep();
        }
        else if(currentStep > GameStep.Gloves){
            points -= 50;
        }
    }

    public void GauzeOnWound(){
        if(currentStep == GameStep.GetGauze){
            points += 100;
            NextStep();
        }
        else if(currentStep > GameStep.GetGauze){
            points -= 50;
        }
        else{
            currentStep = GameStep.GetGauze;
            points += 50;
            NextStep();
        }
    }

    public void BandageOnWound(){
        if(currentStep == GameStep.GetBandage){
            points += 100;
            NextStep();
        }
        else if(currentStep < GameStep.GetBandage){
            currentStep = GameStep.GetBandage;
            NextStep();
        }
    }

    public void Nextlevel()
    {
        GameManager.Instance?.LoadLevelByIndex(3);
    }

}
