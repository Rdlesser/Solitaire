using System.Collections.Generic;
using Interfaces;
using Moves;
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
    private IMoveManager _moveManager;
    private Statistics _statistics;
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
        _statistics = new Statistics();
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
        _statistics.StartTimer();
    }
    
    // Draws a card from the deck via the DeckManager
    public void DrawCard()
    {
        var card = _deckManager.DrawCard(_moveManager);

        if (card == null)
        {
            return;
        }
        
        _tracker.TrackEvent("DrawnCard", new Dictionary<string, string>{{"DrawnCard", $"{card}"}});
        _statistics.IncrementCardsDrawn();
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
        if (selectedCard.transform.parent == targetCard.transform || targetCard.transform.childCount > 0)
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
        
        _deckManager.MoveCardBottom(selected, target, _moveManager);
        _statistics.IncrementMoves();
        _tracker.TrackEvent("CardStacked", new Dictionary<string, string> { { "CardStacked", $"{selected}->{target}" } });
        
        Debug.Log($"Stacked {selectedCard.name} onto {targetCard.name}");
    }

    public void StackCardsInTop(GameObject selectedCard, GameObject targetCard)
    {
        var selected = selectedCard.GetComponent<Selectable>();
        var target = targetCard.GetComponent<Selectable>();

        _deckManager.MoveCardTop(selected, target, _moveManager);
        _statistics.IncrementMoves();
        _tracker.TrackEvent("CardSentToTop", new Dictionary<string, string> { { "CardStacked", $"{selected}->{target}" } });
    }
    
    // Undoes the last move
    public void UndoLastMove()
    {
        _moveManager.UndoLastMove();
        _statistics.IncrementUndos();
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
        var selectable = selected.GetComponent<Selectable>();
        selectable.FlipCard(true);
        _moveManager.RecordMove(new FlipMove(selectable));

        return true;

    }

    public void AutoStack(GameObject selected)
    {
        if (IsBlocked(selected))
        {
            return;
        }
        
        foreach (var topPosObject in _topPos)
        {
            var childCount = topPosObject.transform.childCount;
            GameObject target;

            if (childCount > 0)
            {
                var lastChild = topPosObject.transform.GetChild(childCount - 1);
                target = lastChild.gameObject;

                
            }
            else
            {
                target = topPosObject;
            }

            if (!CanStackTop(selected, target))
            {
                continue;
            }

            StackCardsInTop(selected, target);
            return;
        }
    }

    private bool IsBlocked(GameObject selectedCard)
    {
        var selectable = selectedCard.GetComponent<Selectable>();

        if (selectedCard.transform.childCount == 0 && selectable.IsFaceUp)
        {
            return false;
        }

        return !selectable.IsInDeckPile && !selectable.IsTop && IsCardBlocked(selectable.name, selectable.Row);
    }

    private bool IsCardBlocked(string cardName, int cardRow)
    {
        return _deckManager.IsCardBlocked(cardName, cardRow);
    }
}
