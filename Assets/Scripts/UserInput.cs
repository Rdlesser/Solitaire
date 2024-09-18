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

        // card click actions
        if (!selected.GetComponent<Selectable>().IsFaceUp)
        {
            // if the card clicked on is not blocked
            // flip it over
            selected.GetComponent<Selectable>().IsFaceUp = true;
            _slot1 = gameObject;
        }

        if (_slot1 == gameObject) // not null because we pass in this gameobject instead
        {
            _slot1 = selected;
        }
        
        else if (_slot1 != selected)
        {
            if (IsStackable(selected))
            {
                Stack(selected);
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
        var s1 = _slot1.GetComponent<Selectable>();
        var s2 = selected.GetComponent<Selectable>();

        if (s2.IsInDeckPile)
        {
            return false;
        }
        
        if (s2.IsTop)
        {
            if (s1.Suit == s2.Suit || s1.Value == 1 && s2.Suit == null)
            {
                if (s1.Value == s2.Value + 1)
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
            if (s1.Value == s2.Value - 1)
            {
                var isSelectableRed = true;
                var isNewSelectableRed = true;

                if (s1.Suit == "C" || s1.Suit == "S")
                {
                    isSelectableRed = false;
                }

                if (s2.Suit == "C" || s2.Suit == "S")
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

    private void Stack(GameObject selected)
    {
        var s1 = _slot1.GetComponent<Selectable>();
        var s2 = selected.GetComponent<Selectable>();

        var yOffset = 0.3f;
        var zOffset = 0.01f;

        if (s2.IsTop || s1.Value == 13)
        {
            yOffset = 0;
        }

        _slot1.transform.position = new Vector3(selected.transform.position.x,
            selected.transform.position.y - yOffset,
            selected.transform.position.z - zOffset);

        _slot1.transform.parent = selected.transform; // makes children move with parents

        if (s1.IsInDeckPile)
        {
            _solitaire.RemoveTrip(_slot1.name);
        }
        else if (s1.IsTop && s2.IsTop && s1.Value == 1)
        {
            _solitaire.RemoveCardInTopPos(s1.Row);
        }
        else if (s1.IsTop)
        {
            _solitaire.ChangeTopPosValue(s1.Row, s1.Value - 1);
        }
        else
        {
            _solitaire.RemoveCardFromBottom(s1.Row, s1.name);
        }

        s1.IsInDeckPile = false;
        s1.Row = s2.Row;

        if (s2.IsTop)
        {
            _solitaire.AddCardToTopPos(s1.Row, s1.Value, s1.Suit);
            s1.IsTop = true;
        }
        else
        {
            s1.IsTop = false;
        }

        _slot1 = gameObject;
    }
}
