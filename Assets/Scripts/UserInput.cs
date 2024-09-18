using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] private Solitaire _solitaire;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    
    //TODO: Is this the best way to get mouse click? 
    private void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main != null)
            {
                var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit)
                {
                    if (hit.collider.CompareTag(Tags.DECK))
                    {
                        // clicked deck
                        HandleDeckClick();
                    }
                    else if (hit.collider.CompareTag(Tags.CARD))
                    {
                        // clicked card
                        HandleCardClick();
                    }
                    else if (hit.collider.CompareTag(Tags.TOP))
                    {
                        // clicked top
                        HandleTopClick();
                    }
                    else if (hit.collider.CompareTag(Tags.BOTTOM))
                    {
                        // clicked bottom
                        HandleBottomClick();
                    }
                }
            }
        }
    }

    private void HandleDeckClick()
    {
        Debug.Log("Deck Clicked");
        _solitaire.DealFromDeck();
    }
    
    private void HandleCardClick()
    {
        Debug.Log("Card Clicked");
    }
    
    private void HandleTopClick()
    {
        Debug.Log("Top Clicked");
    }
    
    private void HandleBottomClick()
    {
        Debug.Log("Bottom Clicked");
    }
}
