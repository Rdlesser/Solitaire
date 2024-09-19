using DefaultNamespace;
using Interfaces;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private IGameController _gameController;  // Use interface for loose coupling
    private GameObject _selectedCard;  // Track the currently selected card
    private float _timer;  // For double-click detection
    private float _doubleClickTime = 0.3f;  // Time window for detecting double clicks
    private int _clickCount = 0;  // Track the number of clicks
    
    // Inject the game controller (Solitaire or other implementation)
    public void Initialize(IGameController gameController)
    {
        _gameController = gameController;
    }

    // Update is called once per frame

    void Update()
    {
        // Detect mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null)
            {
                return;
            }
            
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(Tags.CARD))  // If a card is clicked
                {
                    HandleCardClick(hit.collider.gameObject);  // Handle card click (tableau, foundation, or deck)
                }
                else if (hit.collider.CompareTag(Tags.DECK))  // If the deck is clicked
                {
                    HandleDeckClick();  // Handle deck click event
                }
                else if (hit.collider.CompareTag(Tags.BOTTOM))
                {
                    HandleBottomClick(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag(Tags.TOP))
                {
                    HandleTopClick(hit.collider.gameObject);
                }
            }
        }

        // Double-click detection logic
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

        // Detect input for undo move
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _gameController.UndoLastMove();  // Undo the last move via the game controller
        }
    }
        
    // Method to handle clicking on a card (Tableau or Foundation)
    private void HandleCardClick(GameObject card)
    {
        var selectable = card.GetComponent<Selectable>();

        // Check if the card is on the tableau (bottom) facing up
        if (!selectable.IsInDeckPile && !selectable.IsTop && selectable.IsFaceUp)
        {
            if (_selectedCard == null)
            {
                // First selection
                SelectCard(card);
            }
            else if (_selectedCard == card)
            {
                // Same card selected again, unselect it
                DeselectCard();
            }
            else
            {
                // Attempt to stack the selected card on the tableau or foundation
                if (_gameController.CanStackBottom(_selectedCard, card))
                {
                    // Move the card
                    _gameController.StackCardsInBottom(_selectedCard, card);
                    DeselectCard();  // After a successful move, deselect
                }
                else
                {
                    // Invalid move, select new card
                    SelectCard(card);
                }
            }
        }
        else if (!selectable.IsInDeckPile && !selectable.IsFaceUp)
        {
            if (_gameController.TryFlip(card))
            {
                DeselectCard();
            }
        }
        else if (selectable.IsInDeckPile)
        {
            if (_selectedCard == null)
            {
                // First selection
                SelectCard(card);
            }
            else if (_selectedCard == card)
            {
                // Same card selected again, unselect it
                DeselectCard();
            }
        }
        else if (selectable.IsTop)  // Check if the card is in the foundation (top)
        {
            if (_selectedCard != null)
            {
                // Check if you can move the selected card to the foundation
                if (_gameController.CanStackTop(_selectedCard, card))
                {
                    // Move card to foundation
                    _gameController.StackCardsInTop(_selectedCard, card);
                    DeselectCard();  // After a successful move, deselect
                }
            }
        }
    }
    
    private void HandleBottomClick(GameObject selected)
    {
        Debug.Log("Bottom Clicked");

        if (_selectedCard == null || !_selectedCard.CompareTag(Tags.CARD))
        {
            return;
        }

        if (_selectedCard.GetComponent<Selectable>().Value != 13)
        {
            return;
        }

        // Attempt to stack the selected card on the tableau or foundation
        if (!_gameController.CanStackBottom(_selectedCard, selected))
        {
            return;
        }

        // Move the card
        _gameController.StackCardsInBottom(_selectedCard, selected);
        DeselectCard();  // After a successful move, deselect
    }
    
    private void HandleTopClick(GameObject selectedCard)
    {
        Debug.Log("Top Clicked");
        
        if (_selectedCard == null || !_selectedCard.CompareTag(Tags.CARD))
        {
            return;
        }

        var selected = selectedCard.GetComponent<Selectable>();

        if (!_gameController.CanStackTop(_selectedCard, selectedCard))
        {
            return;
        }
        
        // Move the card
        _gameController.StackCardsInTop(_selectedCard, selectedCard);
        DeselectCard();  // After a successful move, deselect
    }
    
    // Method to select and highlight a card
    private void SelectCard(GameObject card)
    {
        if (_selectedCard != null)
        {
            // Unhighlight the previously selected card
            _selectedCard.GetComponent<Selectable>().HighlightCard(false);
        }

        // Highlight the newly selected card
        _selectedCard = card;
        _selectedCard.GetComponent<Selectable>().HighlightCard(true);
    }

    // Method to deselect the currently selected card
    private void DeselectCard()
    {
        if (_selectedCard != null)
        {
            // Unhighlight the previously selected card
            _selectedCard.GetComponent<Selectable>().HighlightCard(false);
            _selectedCard = null;  // Deselect the card
        }
    }

    // Method to handle deck click (drawing cards)
    private void HandleDeckClick()
    {
        _gameController.DrawCard();  // Logic for drawing new cards from the deck
    }

    // public GameObject GetSlot1()
    // {
    //     return _slot1;
    // }

    //TODO: Is this the best way to get mouse click? 
    // private void GetMouseClick()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         _clickCount++;
    //         if (Camera.main == null)
    //         {
    //             return;
    //         }
    //
    //         var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
    //         RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    //
    //         if (hit)
    //         {
    //             if (hit.collider.CompareTag(Tags.DECK))
    //             {
    //                 // clicked deck
    //                 HandleDeckClick();
    //             }
    //             else if (hit.collider.CompareTag(Tags.CARD))
    //             {
    //                 // clicked card
    //                 HandleCardClick(hit.collider.gameObject);
    //             }
    //             else if (hit.collider.CompareTag(Tags.TOP))
    //             {
    //                 // clicked top
    //                 HandleTopClick(hit.collider.gameObject);
    //             }
    //             else if (hit.collider.CompareTag(Tags.BOTTOM))
    //             {
    //                 // clicked bottom
    //                 HandleBottomClick(hit.collider.gameObject);
    //             }
    //         }
    //     }
    // }
    //
    // private void HandleDeckClick()
    // {
    //     Debug.Log("Deck Clicked");
    //     _solitaire.DealFromDeck();
    //     _slot1 = gameObject;
    // }

    // private void HandleCardClick(GameObject selected)
    // {
    //     Debug.Log("Card Clicked");
    //
    //     // card click actions
    //     if (!selected.GetComponent<Selectable>().IsFaceUp)
    //     {
    //         // if the card clicked on is not blocked}
    //         if (!IsBlocked(selected))
    //         {
    //             // flip it over
    //             selected.GetComponent<Selectable>().IsFaceUp = true;
    //             _slot1 = gameObject;
    //         }
    //     }
    //     else if (selected.GetComponent<Selectable>().IsInDeckPile)
    //     {
    //         if (!IsBlocked(selected))
    //         {
    //             if (_slot1 == selected)
    //             {
    //                 // same card clicked on twice
    //                 if (IsDoubleClick())
    //                 {
    //                     // attempt auto stack
    //                     AutoStack(selected);
    //                 }
    //             }
    //             else
    //             {
    //                 _slot1 = selected;
    //             }
    //         }
    //     }
    //
    //     if (_slot1 == gameObject) // not null because we pass in this gameobject instead
    //     {
    //         _slot1 = selected;
    //     }
    //     
    //     else if (_slot1 != selected)
    //     {
    //         if (IsStackable(selected))
    //         {
    //             Stack(selected);
    //         }
    //         else
    //         {
    //             _slot1 = selected;
    //         }
    //     }
    //     else if (_slot1 == selected)
    //     {
    //         if (IsDoubleClick())
    //         {
    //             // Attempt auto stack
    //             AutoStack(selected);
    //         }
    //     }
    // }

    // private void HandleTopClick(GameObject selected)
    // {
    //     Debug.Log("Top Clicked");
    //
    //     if (_slot1.CompareTag(Tags.CARD))
    //     {
    //         if (_slot1.GetComponent<Selectable>().Value == 1)
    //         {
    //             Stack(selected);
    //         }
    //     }
    // }

    // private void HandleBottomClick(GameObject selected)
    // {
    //     Debug.Log("Bottom Clicked");
    //     // if the card is a king and the empty slot is bottom then stack
    //     if (_slot1.CompareTag(Tags.CARD))
    //     {
    //         if (_slot1.GetComponent<Selectable>().Value == 13)
    //         {
    //             Stack(selected);
    //         }
    //     }
    // }

    // private bool IsStackable(GameObject selected)
    // {
    //     var s1 = _slot1.GetComponent<Selectable>();
    //     var s2 = selected.GetComponent<Selectable>();
    //
    //     if (s2.IsInDeckPile)
    //     {
    //         return false;
    //     }
    //     
    //     if (s2.IsTop)
    //     {
    //         if (s1.Suit == s2.Suit || s1.Value == 1 && s2.Suit == null)
    //         {
    //             if (s1.Value == s2.Value + 1)
    //             {
    //                 return true;
    //             }
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    //     else
    //     {
    //         if (s1.Value == s2.Value - 1)
    //         {
    //             var isSelectableRed = true;
    //             var isNewSelectableRed = true;
    //
    //             if (s1.Suit == "C" || s1.Suit == "S")
    //             {
    //                 isSelectableRed = false;
    //             }
    //
    //             if (s2.Suit == "C" || s2.Suit == "S")
    //             {
    //                 isNewSelectableRed = false;
    //             }
    //
    //             if (isSelectableRed == isNewSelectableRed)
    //             {
    //                 Debug.Log("Not stackable");
    //
    //                 return false;
    //             }
    //             else
    //             {
    //                 Debug.Log("Stackable");
    //
    //                 return true;
    //             }
    //         }
    //     }
    //
    //     return false;
    // }
    //
    // private void Stack(GameObject selected)
    // {
    //     var s1 = _slot1.GetComponent<Selectable>();
    //     var s2 = selected.GetComponent<Selectable>();
    //
    //     var yOffset = 0.3f;
    //     var zOffset = 0.01f;
    //
    //     if (s2.IsTop || s1.Value == 13)
    //     {
    //         yOffset = 0;
    //     }
    //
    //     _slot1.transform.position = new Vector3(selected.transform.position.x,
    //         selected.transform.position.y - yOffset,
    //         selected.transform.position.z - zOffset);
    //
    //     _slot1.transform.parent = selected.transform; // makes children move with parents
    //
    //     if (s1.IsInDeckPile)
    //     {
    //         _solitaire.RemoveTrip(_slot1.name);
    //     }
    //     else if (s1.IsTop && s2.IsTop && s1.Value == 1)
    //     {
    //         _solitaire.RemoveCardInTopPos(s1.Row);
    //     }
    //     else if (s1.IsTop)
    //     {
    //         _solitaire.ChangeTopPosValue(s1.Row, s1.Value - 1);
    //     }
    //     else
    //     {
    //         _solitaire.RemoveCardFromBottom(s1.Row, s1.name);
    //     }
    //
    //     s1.IsInDeckPile = false;
    //     s1.Row = s2.Row;
    //
    //     if (s2.IsTop)
    //     {
    //         _solitaire.AddCardToTopPos(s1.Row, s1.Value, s1.Suit);
    //         s1.IsTop = true;
    //     }
    //     else
    //     {
    //         s1.IsTop = false;
    //     }
    //
    //     _slot1 = gameObject;
    // }
    //
    // private bool IsBlocked(GameObject selected)
    // {
    //     var s2 = selected.GetComponent<Selectable>();
    //
    //     if (s2.IsInDeckPile)
    //     {
    //         if (_solitaire.IsLastCardInDraw(s2.name))
    //         {
    //             return false;
    //         }
    //         else
    //         {
    //             Debug.Log($"{s2.name} is blocked!");
    //
    //             return true;
    //         }
    //     }
    //     else
    //     {
    //         if (_solitaire.IsCardBlocked(s2.name, s2.Row))
    //         {
    //             return false;
    //         }
    //         else
    //         {
    //             return true;
    //         }
    //     }
    // }
    //
    // private bool IsDoubleClick()
    // {
    //     return _timer < _doubleClickTime && _clickCount == 2;
    // }
    //
    // private void AutoStack(GameObject selected)
    // {
    //     for (int i = 0; i < _solitaire.GetTopPosCount(); i++)
    //     {
    //         var topPosCard = _solitaire.GetTopPos(i).GetComponent<Selectable>();
    //
    //         if (selected.GetComponent<Selectable>().Value == 1) // if it is an ace
    //         {
    //             if (topPosCard.Value == 0) // and the top position is empty 
    //             {
    //                 _slot1 = selected;
    //                 Stack(topPosCard.gameObject); // stack the ace up top
    //                 break; // in the first empty position found
    //             }
    //         }
    //         else
    //         {
    //             if (_solitaire.GetTopPos(i).GetComponent<Selectable>().Suit == _slot1.GetComponent<Selectable>().Suit && _solitaire.GetTopPos(i).GetComponent<Selectable>().Value == _slot1.GetComponent<Selectable>().Value - 1)
    //             {
    //                 // if it is the last card (has no children)
    //                 if (DoesCardHaveChildren(_slot1))
    //                 {
    //                     return;
    //                 }
    //                 
    //                 _slot1 = selected;
    //
    //                 // find a top spot that matches the conditions for auto stacking if it exists
    //                 var lastCardName = topPosCard.Value switch
    //                 {
    //                     1 => $"{topPosCard.Suit}A",
    //                     11 => $"{topPosCard.Suit}J",
    //                     12 => $"{topPosCard.Suit}Q",
    //                     13 => $"{topPosCard.Suit}K",
    //                     _ => topPosCard.Suit + topPosCard.Value.ToString()
    //                 };
    //
    //                 var lastCard = GameObject.Find(lastCardName);
    //                 Stack(lastCard);
    //                 break;
    //             }
    //         }
    //     }
    // }
    //
    // private bool DoesCardHaveChildren(GameObject card)
    // {
    //     return card.transform.childCount != 0;
    // }
}
