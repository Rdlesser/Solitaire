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
        
        if (Input.touchCount > 0)  // Touch input for Android
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchInput(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))  // Mouse input for desktop
        {
            HandleMouseInput(Input.mousePosition);
        }

        // Detect input for undo move
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _gameController.UndoLastMove();  // Undo the last move via the game controller
        }
    }
    
    // Handle mouse input (for desktop)
    private void HandleMouseInput(Vector3 inputPosition)
    {
        _clickCount++;
        if (Camera.main == null)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPosition), Vector2.zero);

        if (hit.collider != null)
        {
            ProcessHit(hit.collider);
        }
    }
    
    // Handle touch input (for Android)
    private void HandleTouchInput(Vector3 inputPosition)
    {
        _clickCount++;
        if (Camera.main == null)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPosition), Vector2.zero);

        if (hit.collider != null)
        {
            ProcessHit(hit.collider);
        }
    }
    
    // Process hits for both touch and mouse input
    private void ProcessHit(Collider2D hitCollider)
    {
        if (hitCollider.CompareTag(Tags.CARD))  // If a card is clicked or touched
        {
            HandleCardClick(hitCollider.gameObject);  // Handle card click (tableau, foundation, or deck)
        }
        else if (hitCollider.CompareTag(Tags.DECK))  // If the deck is clicked or touched
        {
            HandleDeckClick();  // Handle deck click event
        }
        else if (hitCollider.CompareTag(Tags.BOTTOM))
        {
            HandleBottomClick(hitCollider.gameObject);
        }
        else if (hitCollider.CompareTag(Tags.TOP))
        {
            HandleTopClick(hitCollider.gameObject);
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
                if (IsDoubleClick())
                {
                    _gameController.AutoStack(card);
                }

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
                if (IsDoubleClick())
                {
                    _gameController.AutoStack(card);
                }
                
                DeselectCard();
            }
        }
        else if (selectable.IsTop)  // Check if the card is in the foundation (top)
        {
            if (_selectedCard == null || !_gameController.CanStackTop(_selectedCard, card))
            {
                return;
            }

            // Move card to foundation
            _gameController.StackCardsInTop(_selectedCard, card);
            DeselectCard();  // After a successful move, deselect
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
    
    private bool IsDoubleClick()
    {
        return _timer < _doubleClickTime && _clickCount == 2;
    }
}
