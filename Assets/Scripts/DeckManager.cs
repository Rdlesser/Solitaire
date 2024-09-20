using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Moves;
using UnityEngine;

public class DeckManager : IDeckManager
{
    private static readonly string[] Suits = { "C", "D", "H", "S" };
    private static readonly string[] Values = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    
    private List<string> _deck = new();  // Cards in the deck
    private List<string> _discardPile = new();  // Discarded cards
    private List<Selectable> _drawnCards = new();
    
    private GameObject[] _bottomPos;  // Reference to the bottom positions (e.g. Tableau piles in Solitaire)
    private GameObject[] _topPos;     // Reference to the top positions (e.g. Foundation piles in Solitaire)
    private GameObject _cardPrefab;   // The prefab used to instantiate a card in the scene
    private Transform _deckPosition;  // Reference to the deck's position on the board
    private Transform _drawnCardsPosition;
    
    private List<string>[] _bottoms;  // List to store cards in the bottom (tableau) piles
    
    private Dictionary<string, Sprite> _cardFaceDictionary;
    private Sprite _cardBackSprite;
    
    public DeckManager(GameObject[] bottomPos, GameObject[] topPos, GameObject cardPrefab, Transform deckPosition, Transform drawnCardsPosition, Dictionary<string, Sprite> cardFaceDictionary, Sprite cardBackSprite)
    {
        _bottomPos = bottomPos;
        _topPos = topPos;
        _cardPrefab = cardPrefab;
        _deckPosition = deckPosition;
        _drawnCardsPosition = drawnCardsPosition;
        _cardFaceDictionary = cardFaceDictionary;
        _cardBackSprite = cardBackSprite;
    }
    
    public void GenerateDeck()
    {
        _deck.Clear();  // Ensure the deck is empty before filling it
        foreach (string suit in Suits)
        {
            foreach (var value in Values)
            {
                // Create a card name (e.g., "C2" for 2 of clubs)
                var cardName = suit + value;

                // // Instantiate the card prefab and set its properties
                // var newCard = Object.Instantiate(_cardPrefab, _deckPosition.position, Quaternion.identity);
                // newCard.name = cardName;

                // Initialize the card's Selectable component with its value and suit
                // var selectable = newCard.GetComponent<Selectable>();
                // selectable.Initialize(GetCardValue(value), suit, false, true, _cardFaceDictionary, _cardBackSprite);

                // Add the card to the deck
                _deck.Add(cardName);
            }
        }

        Debug.Log($"Generated deck with {_deck.Count} cards.");
    }
    
    public void ShuffleDeck()
    {
        _deck.Shuffle();    
    }
    
    // Draws a card from the deck
    public GameObject DrawCard(IMoveManager moveManager)
    {
        if (_deck.Count > 0)
        {
            var cardName = _deck[0];  // Get the top card from the deck
            _deck.RemoveAt(0);  // Remove it from the deck
            // Instantiate the card prefab and set its properties
            var newCard = Object.Instantiate(_cardPrefab, _drawnCardsPosition.position, Quaternion.identity, _drawnCardsPosition);
            newCard.name = cardName;

            // Initialize the card's Selectable component with its value and suit
            var selectable = newCard.GetComponent<Selectable>();
            selectable.Initialize(GetCardValue(cardName), GetCardSuit(cardName), false, true, _cardFaceDictionary, _cardBackSprite);

            // Move the card to the discard pile and flip it face-up
            newCard.transform.position = new Vector3(_drawnCardsPosition.position.x, _drawnCardsPosition.position.y, _drawnCardsPosition.position.z + _discardPile.Count * -0.02f);
            _discardPile.Add(cardName);
            selectable.FlipCard(true);

            _drawnCards.Add(selectable);
            moveManager.RecordMove(new DrawMove(selectable, _drawnCards, _discardPile, _deck));
            return newCard;
        }
        else
        {
            // Reset the deck
            
            _deck = new(_discardPile);
            // Destroy all instantiated objects in discard pile that have not been placed
            foreach (var card in _drawnCards)
            {
                if (card.IsInDeckPile)
                {
                    card.gameObject.SetActive(false);
                    Object.Destroy(card.gameObject);
                }
            }
            
            // clear drawn cards
            _drawnCards.Clear();
            _discardPile.Clear();
            Debug.Log("No more cards in the deck to draw.");
            return null;
        }
    }

    public List<string> GetDiscardPile()
    {
        return _discardPile;
    }

    public IEnumerator DealCards()
    {
        _bottoms = new List<string>[_bottomPos.Length];

        // Distribute cards to the tableau piles (bottom positions)
        for (int i = 0; i < _bottomPos.Length; i++)
        {
            _bottoms[i] = new List<string>();
            for (int j = 0; j <= i; j++)  // Increasing number of cards for each column
            {
                var card = _deck[0];
                _deck.RemoveAt(0);

                // Add card to the current tableau pile
                _bottoms[i].Add(card);

                // Instantiate the card in the scene
                GameObject newCard = Object.Instantiate(_cardPrefab, _deckPosition.position, Quaternion.identity, _bottomPos[i].transform);
                newCard.name = card;
                newCard.transform.position = _bottomPos[i].transform.position + new Vector3(0, -j * 0.3f, -j * 0.1f -0.1F);  // Stagger the cards visually
                var selectable = newCard.GetComponent<Selectable>();
                var isFaceUp = j == i;
                
                selectable.Initialize(
                    value: GetCardValue(card),
                    suit: GetCardSuit(card),
                    isFaceUp: isFaceUp,
                    isInDeckPile: false,
                    cardFaceDictionary: _cardFaceDictionary,
                    cardBackSprite: _cardBackSprite
                );
                
                selectable.SetRow(i);

                yield return new WaitForSeconds(0.02f);
            }
        }

        // The remaining cards stay in the deck for future draws
    }
    
    private int GetCardValue(string card)
    {
        var value = card[1..];  // Get value part of the string (e.g., "A", "2", "J")
        return value switch
        {
            "A" => 1,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            _ => int.Parse(value)  // Parse numeric values
        };
    }
    
    private string GetCardSuit(string card)
    {
        return card[..1];  // Get suit part of the string (e.g., "C" for clubs)
    }
    
    public bool IsCardBlocked(string cardName, int cardRow)
    {
        return _bottoms[cardRow].Last() != cardName;
    }

    public void MoveCardBottom(Selectable selected, Selectable target, IMoveManager moveManager)
    {
        moveManager.RecordMove(new CardMove(_discardPile, selected, _bottoms));
        if (!selected.IsInDeckPile && !selected.IsTop && _bottoms[selected.Row].Contains(selected.name))
        {
            _bottoms[selected.Row].Remove(selected.name);
        }
        
        selected.SetRow(target.Row);

        foreach (Transform child in selected.transform)
        {
            var selectedChild = child.GetComponent<Selectable>();
            selectedChild.SetRow(target.Row);
        }
        
        var yOffset = 0.31f;
        if (target.Value == 0)
        {
            yOffset = 0;
        }
        selected.transform.position = new Vector3(target.transform.position.x, target.transform.position.y - yOffset, target.transform.position.z - 0.1f);
        selected.transform.SetParent(target.transform);  // Make the selected card a child of the target card
        
        if (selected.IsInDeckPile)
        {
            selected.IsInDeckPile = false;
            _discardPile.Remove(selected.name);
        }
    }

    public void MoveCardTop(Selectable selected, Selectable target, IMoveManager moveManager)
    {
        moveManager.RecordMove(new CardMove(_discardPile, selected, _bottoms));
        if (!selected.IsInDeckPile && !selected.IsTop && _bottoms[selected.Row].Contains(selected.name))
        {
            _bottoms[selected.Row].Remove(selected.name);
        }
        
        selected.SetRow(target.Row);
        selected.IsTop = true;
        selected.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - 0.1f);
        selected.transform.SetParent(_topPos[target.Row].transform);  // Make the selected card a child of the target pile

        if (!selected.IsInDeckPile)
        {
            return;
        }

        selected.IsInDeckPile = false;
        _discardPile.Remove(selected.name);
    }

    public void TryAutoStack(GameObject selected)
    {
        
    }
}