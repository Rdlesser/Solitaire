using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{

    private const string DECK = "Deck";
    private const string CARD = "Card";
    private const string TOP = "Top";
    private const string BOTTOM = "Bottom";
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
                    if (hit.collider.CompareTag(DECK))
                    {
                        // clicked deck
                        HandleDeckClick();
                    }
                    else if (hit.collider.CompareTag(CARD))
                    {
                        // clicked card
                        HandleCardClick();
                    }
                    else if (hit.collider.CompareTag(TOP))
                    {
                        // clicked top
                        HandleTopClick();
                    }
                    else if (hit.collider.CompareTag(BOTTOM))
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
