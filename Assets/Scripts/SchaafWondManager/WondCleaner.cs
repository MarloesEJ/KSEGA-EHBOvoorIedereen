using UnityEngine;
using System.Collections.Generic;

public class WondCleaner : MonoBehaviour
{
    public SchaafWondManager gameManager;
    public float cleanSpeed = 1.5f;
    private SpriteRenderer spriteRenderer;

    public List<GameObject> dirtObjects = new List<GameObject>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        GameObject[] allDirt = GameObject.FindGameObjectsWithTag("Dirt");
        dirtObjects.AddRange(allDirt);
    }

    void Update()
    {
        if(gameManager.gauzeIsWet)
        {
            spriteRenderer.enabled = true;
            MoveToMouse();
        }
    }

    void MoveToMouse(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(Input.GetMouseButton(0) && other.CompareTag("Dirt"))
        {
            CleanDirt(other.gameObject);
        }
    }

    void CleanDirt(GameObject dirt)
    {
        SpriteRenderer dirtSprite = dirt.GetComponent<SpriteRenderer>();
        if(dirtSprite != null)
        {
            Color c = dirtSprite.color;
            c.a -= cleanSpeed * Time.deltaTime;
            dirtSprite.color = c;

            if(c.a <= 0.0f){
                dirt.SetActive(false);
                CheckIfAllClean();
            }
        }
    }

    public void CheckIfAllClean(){
        foreach(GameObject dirt in dirtObjects){
            if(dirt.activeSelf) return;

            gameManager.FinishGame();
        }
    }
}
