using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Interfaces;
using UnityEngine;

public class Solitaire : MonoBehaviour, IGameController
{
    [SerializeField] private Sprite[] _cardFaces;
    [SerializeField] private Sprite _cardBackSprite;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _deckButton;
    [SerializeField] private GameObject[] _bottomPos;
    [SerializeField] private GameObject[] _topPos;
    [SerializeField] private Transform _deckPosition;
    [SerializeField] private Transform _drawnCardsPosition;
    [SerializeField] private UserInput _userInput;
    
    private IDeckManager _deckManager;
    private MoveManager _moveManager;
    private GameStatistics _statistics;
    private ITracker _tracker;
    
    private Dictionary<string, Sprite> _cardFaceDictionary = new();

    private void OnEnable()
    {
        CreateCardFaceDictionary();

        _deckManager = new DeckManager(bottomPos: _bottomPos,
            topPos: _topPos,
            cardPrefab: _cardPrefab,
            deckPosition: _deckPosition,
            drawnCardsPosition: _drawnCardsPosition,
            cardFaceDictionary: _cardFaceDictionary,
            cardBackSprite: _cardBackSprite
        );
        
        _moveManager = new MoveManager();
        _statistics = new GameStatistics();
        _tracker = new SimpleEventTracker();  // Simple tracker implementation for event tracking
        
        // Initialize the UserInput and inject the game controller interface
        _userInput.Initialize(this);  // Pass `this` because Solitaire implements IGameController
    }

    // Start is called before the first frame update
    void Start()
    {
        // Generate and shuffle the deck, then deal the cards
        _deckManager.GenerateDeck();
        _deckManager.ShuffleDeck();
        StartCoroutine(_deckManager.DealCards());  // Deal cards using the DeckManager
        _tracker.TrackEvent("GameStarted", new Dictionary<string, string> { { "EventType", "Start" } });
    }
    
    // Draws a card from the deck via the DeckManager
    public void DrawCard()
    {
        var card = _deckManager.DrawCard();

        if (card == null)
        {
            return;
        }

        _moveManager.RecordMove(new DrawMove(card, _deckManager.GetDiscardPile()));  // Record the draw move
        _tracker.TrackEvent("DrawnCard", new Dictionary<string, string>{{"DrawnCard", $"{card}"}});
    }

    private void CreateCardFaceDictionary()
    {
        foreach (var cardFace in _cardFaces)
        {
            if (cardFace.name == "CardBack")
            {
                _cardBackSprite = cardFace;  // Assign the card back sprite
            }
            else
            {
                _cardFaceDictionary[cardFace.name] = cardFace;  // Add card face to the dictionary
            }
        }
        
        // Log an error if no card back sprite is found
        if (_cardBackSprite == null)
        {
            Debug.LogError("Card back sprite could not be found");
        }
    }
    
    public bool CanStackBottom(GameObject selectedCard, GameObject targetCard)
    {
        if (selectedCard.transform.parent == targetCard.transform)
        {
            return false;
        }
        
        var selected = selectedCard.GetComponent<Selectable>();
        var target = targetCard.GetComponent<Selectable>();

        // Solitaire rule: Cards can only be stacked if they have alternating colors and consecutive ranks
        var isDifferentColor = selected.Suit is "C" or "S" && target.Suit is "H" or "D" or null || selected.Suit is "H" or "D" && target.Suit is "C" or "S" or null;
        var isNextRank = selected.Value == target.Value - 1 || selected.Value == 13 && target.Value == 0;

        return isDifferentColor && isNextRank;
    }

    public bool CanStackTop(GameObject selectedCard, GameObject targetCard)
    {
        var selected = selectedCard.GetComponent<Selectable>();
        var target = targetCard.GetComponent<Selectable>();

        if (selected.Value == 1 && target.Value == 0)
        {
            return true;
        }

        var isSameSuit = selected.Suit == target.Suit;
        var isNextRank = selected.Value == target.Value + 1;

        return isSameSuit && isNextRank;
    }
    
    // Stacks the selected card onto the target card (in the tableau)
    public void StackCardsInBottom(GameObject selectedCard, GameObject targetCard)
    {
        var selected = selectedCard.GetComponent<Selectable>();
        var target = targetCard.GetComponent<Selectable>();
       
        var parent = selectedCard.transform.parent;
        _moveManager.RecordMove(new CardMove(selectedCard, parent.gameObject));
        _deckManager.MoveCardBottom(selected, target);
        _tracker.TrackEvent("CardStacked", new Dictionary<string, string> { { "CardStacked", $"{selected}->{target}" } });

        // Record the move in the MoveManager (for undo purposes)

        Debug.Log($"Stacked {selectedCard.name} onto {targetCard.name}");
    }

    public void StackCardsInTop(GameObject selectedCard, GameObject targetCard)
    {
        var selected = selectedCard.GetComponent<Selectable>();
        var target = targetCard.GetComponent<Selectable>();

        var parent = selectedCard.transform.parent;
        _deckManager.MoveCardTop(selected, target);
        _tracker.TrackEvent("CardSentToTop", new Dictionary<string, string> { { "CardStacked", $"{selected}->{target}" } });
        
        // Record the move in the MoveManager (for undo purposes)
        _moveManager.RecordMove(new CardMove(selectedCard, parent.gameObject));
    }
    
    // Determines if the selected card can be moved to the foundation
    public bool CanMoveToFoundation(GameObject selectedCard)
    {
        var selected = selectedCard.GetComponent<Selectable>();

        // Solitaire rule: Only aces can be moved to an empty foundation pile, or cards of the same suit and in sequential order
        if (selected.Value == 1)  // Ace case
        {
            return true;
        }

        // Check if it can be stacked on the current foundation pile
        foreach (var topPos in _topPos)
        {
            var topCard = topPos.GetComponentInChildren<Selectable>();
            if (topCard != null && topCard.Suit == selected.Suit && topCard.Value == selected.Value - 1)
            {
                return true;
            }
        }

        return false;
    }
    
    // Moves the selected card to the foundation pile
    public void MoveToFoundation(GameObject selectedCard)
    {
        var selected = selectedCard.GetComponent<Selectable>();

        // Find an empty spot or the correct foundation pile
        foreach (var topPos in _topPos)
        {
            var topCard = topPos.GetComponentInChildren<Selectable>();

            // Place the card on an empty foundation pile or stack it on the correct pile
            if (topCard == null || (topCard.Suit == selected.Suit && topCard.Value == selected.Value - 1))
            {
                selectedCard.transform.position = topPos.transform.position;
                var parent = selectedCard.transform.parent;
                selectedCard.transform.SetParent(topPos.transform);

                // Record the move in the MoveManager
                _moveManager.RecordMove(new CardMove(selectedCard, parent.gameObject));

                Debug.Log($"Moved {selectedCard.name} to foundation");
                return;
            }
        }
    }
    
    // Helper methods to extract card value and suit from a card name (e.g., "H2" -> "H", 2)
    private int GetCardValue(string cardName)
    {
        var value = cardName[1..];  // Get the value part of the card (e.g., "2", "10", "J")
        return value switch
        {
            "A" => 1,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            _ => int.Parse(value)
        };
    }

    private string GetCardSuit(string cardName)
    {
        return cardName[..1];  // Get the suit part of the card (e.g., "H" for hearts)
    }
    
    // Undoes the last move
    public void UndoLastMove()
    {
        _moveManager.UndoLastMove();
        Debug.Log("Undid the last move");
    }

    public bool TryFlip(GameObject selected)
    {
        // if the card clicked on is not blocked}
        if (IsBlocked(selected))
        {
            return false;
        }

        // flip it over
        selected.GetComponent<Selectable>().FlipCard(true);

        return true;

    }
    
    private bool IsBlocked(GameObject selectedCard)
    {
        var selectable = selectedCard.GetComponent<Selectable>();
        return IsCardBlocked(selectable.name, selectable.Row);
    }

    private bool IsCardBlocked(string cardName, int cardRow)
    {
        return _deckManager.IsCardBlocked(cardName, cardRow);
    }
}
