using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] private GameObject _slot1;
    [SerializeField] private Solitaire _solitaire;
    
    // Start is called before the first frame update
    void Start()
    {
        _slot1 = gameObject;
    }

    // Update is called once per frame

    void Update()
    {
        GetMouseClick();
    }

    public GameObject GetSlot1()
    {
        return _slot1;
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
                        HandleCardClick(hit.collider.gameObject);
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

    private void HandleCardClick(GameObject selected)
    {
        Debug.Log("Card Clicked");

        if (_slot1 == gameObject) // not null because we pass in this gameobject instead
        {
            _slot1 = selected;
        }
        
        else if (_slot1 != selected)
        {
            if (IsStackable(selected))
            {
                
            }
            else
            {
                _slot1 = selected;
            }
        }
    }

    private void HandleTopClick()
    {
        Debug.Log("Top Clicked");
    }

    private void HandleBottomClick()
    {
        Debug.Log("Bottom Clicked");
    }

    private bool IsStackable(GameObject selected)
    {
        var selectable = _slot1.GetComponent<Selectable>();
        var newSelectable = selected.GetComponent<Selectable>();

        if (newSelectable.IsTop)
        {
            if (selectable.Suit == newSelectable.Suit || selectable.Value == 1 && newSelectable.Suit == null)
            {
                if (selectable.Value == newSelectable.Value + 1)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (selectable.Value == newSelectable.Value - 1)
            {
                var isSelectableRed = true;
                var isNewSelectableRed = true;

                if (selectable.Suit == "C" || selectable.Suit == "S")
                {
                    isSelectableRed = false;
                }

                if (newSelectable.Suit == "C" || newSelectable.Suit == "S")
                {
                    isNewSelectableRed = false;
                }

                if (isSelectableRed == isNewSelectableRed)
                {
                    Debug.Log("Not stackable");

                    return false;
                }
                else
                {
                    Debug.Log("Stackable");

                    return true;
                }
            }
        }

        return false;
    }
}
