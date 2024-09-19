using DefaultNamespace;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    [SerializeField] private GameObject _slot1;
    [SerializeField] private Solitaire _solitaire;

    private float _timer;
    private float _doubleClickTime = 0.3f;
    private int _clickCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _slot1 = gameObject;
    }

    // Update is called once per frame

    void Update()
    {
        if (_clickCount == 1)
        {
            _timer += Time.deltaTime;
        }

        if (_clickCount == 3)
        {
            _timer = 0;
            _clickCount = 1;
        }

        if (_timer > _doubleClickTime)
        {
            _timer = 0;
            _clickCount = 0;
        }
        GetMouseClick();
    }

    public GameObject GetSlot1()
    {
        return _slot1;
    }

    //TODO: Is this the best way to get mouse click? 
    private void GetMouseClick()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        _clickCount++;
        if (Camera.main == null)
        {
            return;
        }
        
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
                HandleTopClick(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag(Tags.BOTTOM))
            {
                // clicked bottom
                HandleBottomClick(hit.collider.gameObject);
            }
        }
    }

    private void HandleDeckClick()
    {
        Debug.Log("Deck Clicked");
        _solitaire.DealFromDeck();
        _slot1 = gameObject;
    }

    private void HandleCardClick(GameObject selected)
    {
        Debug.Log("Card Clicked");

        // card click actions
        if (!selected.GetComponent<Selectable>().IsFaceUp)
        {
            // if the card clicked on is not blocked}
            if (!IsBlocked(selected))
            {
                // flip it over
                selected.GetComponent<Selectable>().IsFaceUp = true;
                _slot1 = gameObject;
            }
        }
        else if (selected.GetComponent<Selectable>().IsInDeckPile)
        {
            if (!IsBlocked(selected))
            {
                if (_slot1 == selected)
                {
                    // same card clicked on twice
                    if (IsDoubleClick())
                    {
                        // attempt auto stack
                        AutoStack(selected);
                    }
                }
                else
                {
                    _slot1 = selected;
                }
            }
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
        else if (_slot1 == selected)
        {
            if (IsDoubleClick())
            {
                // Attempt auto stack
                AutoStack(selected);
            }
        }
    }

    private void HandleTopClick(GameObject selected)
    {
        Debug.Log("Top Clicked");

        if (_slot1.CompareTag(Tags.CARD))
        {
            if (_slot1.GetComponent<Selectable>().Value == 1)
            {
                Stack(selected);
            }
        }
    }

    private void HandleBottomClick(GameObject selected)
    {
        Debug.Log("Bottom Clicked");
        // if the card is a king and the empty slot is bottom then stack
        if (_slot1.CompareTag(Tags.CARD))
        {
            if (_slot1.GetComponent<Selectable>().Value == 13)
            {
                Stack(selected);
            }
        }
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

    private bool IsBlocked(GameObject selected)
    {
        var s2 = selected.GetComponent<Selectable>();

        if (s2.IsInDeckPile)
        {
            if (_solitaire.IsLastCardInDraw(s2.name))
            {
                return false;
            }
            else
            {
                Debug.Log($"{s2.name} is blocked!");

                return true;
            }
        }
        else
        {
            if (_solitaire.IsCardBlocked(s2.name, s2.Row))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private bool IsDoubleClick()
    {
        return _timer < _doubleClickTime && _clickCount == 2;
    }

    private void AutoStack(GameObject selected)
    {
        for (int i = 0; i < _solitaire.GetTopPosCount(); i++)
        {
            var topPosCard = _solitaire.GetTopPos(i).GetComponent<Selectable>();

            if (selected.GetComponent<Selectable>().Value == 1) // if it is an ace
            {
                if (topPosCard.Value == 0) // and the top position is empty 
                {
                    _slot1 = selected;
                    Stack(topPosCard.gameObject); // stack the ace up top
                    break; // in the first empty position found
                }
            }
            else
            {
                if (_solitaire.GetTopPos(i).GetComponent<Selectable>().Suit == _slot1.GetComponent<Selectable>().Suit && _solitaire.GetTopPos(i).GetComponent<Selectable>().Value == _slot1.GetComponent<Selectable>().Value - 1)
                {
                    // if it is the last card (has no children)
                    if (DoesCardHaveChildren(_slot1))
                    {
                        return;
                    }
                    
                    _slot1 = selected;

                    // find a top spot that matches the conditions for auto stacking if it exists
                    var lastCardName = topPosCard.Value switch
                    {
                        1 => $"{topPosCard.Suit}A",
                        11 => $"{topPosCard.Suit}J",
                        12 => $"{topPosCard.Suit}Q",
                        13 => $"{topPosCard.Suit}K",
                        _ => topPosCard.Suit + topPosCard.Value.ToString()
                    };

                    var lastCard = GameObject.Find(lastCardName);
                    Stack(lastCard);
                    break;
                }
            }
        }
    }

    private bool DoesCardHaveChildren(GameObject card)
    {
        return card.transform.childCount != 0;
    }
}
