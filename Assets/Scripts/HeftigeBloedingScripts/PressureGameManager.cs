using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PressureGameManager : MonoBehaviour
{
    
    [Header("Instellingen")]
    public List<GameObject> pressurePoints;
    public float shrinkSpeed = 0.5f;
    public float targetScale = 0.2f;

    private int currentIndex = 0;
    private bool isPressing = false;

    public HeftigeBloedingLevelManager levelManager;

    void Start()
    {
        foreach(GameObject obj in pressurePoints){
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if(trigger == null) trigger = obj.AddComponent<EventTrigger>();

            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { OnPress(); });
            trigger.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { OnRelease(); });
            trigger.triggers.Add(entryUp);

            obj.SetActive(false);
        }

        if(pressurePoints.Count > 0){
            pressurePoints[0].SetActive(true);
        }
    }

    void Update()
    {
        if(isPressing && currentIndex < pressurePoints.Count){
            ShrinkCurrentPoint();
        }
    }

    void ShrinkCurrentPoint(){
        GameObject currentObj = pressurePoints[currentIndex];

        currentObj.transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;

        if(currentObj.transform.localScale.x <= targetScale){
            NextImage();
        }
    }

    void NextImage(){
        pressurePoints[currentIndex].SetActive(false);
        currentIndex++;

        isPressing = false;

        if(currentIndex < pressurePoints.Count){
            pressurePoints[currentIndex].SetActive(true);
        } else {
            Debug.Log("Pressure game completed!");
            levelManager.NextStep();
            isPressing = false;
        }
    }

    public void OnPress()
    {
        isPressing = true;
    }

    public void OnRelease()
    {
        isPressing = false;
    }

}
