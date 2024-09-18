using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class Solitaire : MonoBehaviour
{

    [SerializeField] private Sprite[] _cardFaces;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _deckButton;
    [SerializeField] private GameObject[] _bottomPos;
    [SerializeField] private GameObject[] _topPos;
    [SerializeField] private UserInput _userInput;
    
    private float _cardInitialYOffset = 0f;
    private float _cardYOffsetIncrement = 0.3f;
    private float _cardInitialZOffset = 0.03f;
    private float _cardZOffsetIncrement = 0.03f;
    
    public static string[] Suits = new string[] { "C", "D", "H", "S" };
    public static string[] Values = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    
    private List<string>[] _bottoms;
    private List<string>[] _tops;
    private List<string> _tripsOnDisplay = new();
    private List<List<string>> _deckTrips = new();

    private List<string> _bottom0 = new();
    private List<string> _bottom1 = new();
    private List<string> _bottom2 = new();
    private List<string> _bottom3 = new();
    private List<string> _bottom4 = new();
    private List<string> _bottom5 = new();
    private List<string> _bottom6 = new();

    public List<string> _deck;
    public List<string> _discardPile = new();

    private int _deckLocation;
    private int _trips;
    private int _tripsRemainder;

    private Dictionary<string, Sprite> _cardFaceDictionary = new();

    private void OnEnable()
    {
        CreateCardFaceDictionary();
    }

    // Start is called before the first frame update

    void Start()
    {
        _bottoms = new List<string>[] { _bottom0, _bottom1, _bottom2, _bottom3, _bottom4, _bottom5, _bottom6 };
        PlayCards();
    }

    // Update is called once per frame

    void Update()
    {
        
    }

    private void CreateCardFaceDictionary()
    {
        foreach (var cardFace in _cardFaces)
        {
            _cardFaceDictionary[cardFace.name] = cardFace;
        }
    }

    public void PlayCards()
    {
        _deck = GenerateDeck();
        _deck.Shuffle();
        
        //test the cards in the deck:
        foreach (var card in _deck)
        {
            Debug.Log(card);
        }
        
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips();
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();

        foreach (var suit in Suits)
        {
            foreach (var value in Values)
            {
                newDeck.Add(suit + value);
            }
        }

        return newDeck;
    }
    
    public Sprite GetCardFace(string cardName)
    {
        return _cardFaceDictionary.GetValueOrDefault(cardName);
    }

    private IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < 7; i++)
        {
            var yOffset = _cardInitialYOffset;
            var zOffset = _cardInitialZOffset;
        
            foreach (var card in _bottoms[i])
            {
                yield return new WaitForSeconds(0.01f);
                var newCard = Instantiate(_cardPrefab, new Vector3(_bottomPos[i].transform.position.x, _bottomPos[i].transform.position.y - yOffset, _bottomPos[i].transform.position.z - zOffset), Quaternion.identity, _bottomPos[i].transform);
                newCard.name = card;
                newCard.GetComponent<Selectable>().Row = i;

                if (card == _bottoms[i][_bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().IsFaceUp = true;
                }
                
                newCard.GetComponent<UpdateSprite>().Inject(this, _userInput);

                yOffset += _cardYOffsetIncrement;
                zOffset += _cardZOffsetIncrement;
                _discardPile.Add(card);
            }
        }

        foreach (var card in _discardPile)
        {
            if (_deck.Contains(card))
            {
                _deck.Remove(card);
            }
        }
        
        _discardPile.Clear();
    }

    private void SolitaireSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                _bottoms[j].Add(_deck.Last());
                _deck.RemoveAt(_deck.Count - 1);
            }
        }
    }

    public void SortDeckIntoTrips()
    {
        _trips = _deck.Count / 3;
        _tripsRemainder = _deck.Count % 3;
        _deckTrips.Clear();

        var modifier = 0;

        for (int i = 0; i < _trips; i++)
        {
            var myTrips = new List<string>();

            for (int j = 0; j < 3; j++)
            {
                myTrips.Add(_deck[j + modifier]);
            }
            
            _deckTrips.Add(myTrips);
            modifier += 3;
        }

        if (_tripsRemainder != 0)
        {
            var myRemainders = new List<string>();
            modifier = 0;

            for (int k = 0; k < _tripsRemainder; k++)
            {
                myRemainders.Add(_deck[_deck.Count - _tripsRemainder + modifier]);
                modifier++;
            }

            _deckTrips.Add(myRemainders);
            _trips++;
        }

        _deckLocation = 0;
    }

    public void DealFromDeck()
    {
        // Add remaining cards to discard pile 
        foreach (Transform child in _deckButton.transform)
        {
            if (!child.CompareTag(Tags.CARD))
            {
                continue;
            }
            _deck.Remove(child.name);
            _discardPile.Add(child.name);
            Destroy(child.gameObject);
        }
        
        if (_deckLocation < _trips)
        {
            // draw 3 new cards
            _tripsOnDisplay.Clear();
            var xOffset = 2.5f;
            var zOffset = -0.2f;

            foreach (var card in _deckTrips[_deckLocation])
            {
                var newTopCard = Instantiate(_cardPrefab,
                    new Vector3(_deckButton.transform.position.x + xOffset,
                        _deckButton.transform.position.y,
                        _deckButton.transform.position.z + zOffset),
                    Quaternion.identity,
                    _deckButton.transform);

                xOffset += 0.5f;
                zOffset -= 0.2f;
                newTopCard.name = card;
                _tripsOnDisplay.Add(card);
                var selectable = newTopCard.GetComponent<Selectable>();
                selectable.IsFaceUp = true;
                selectable.IsInDeckPile = true;
                newTopCard.GetComponent<UpdateSprite>().Inject(this, _userInput);
            }

            _deckLocation++;
        }
        else
        {
            // Restack top deck
            RestackTopDeck();
        }
    }
    
    private void RestackTopDeck()
    {
        foreach (var card in _discardPile)
        {
            _deck.Add(card);
        }
        
        _discardPile.Clear();
        SortDeckIntoTrips();
    }

    public void RemoveTrip(string card)
    {
        _tripsOnDisplay.Remove(card);
    }

    public void RemoveCardInTopPos(int index)
    {
        // TODO: just dispatch event instead of this
        _topPos[index].GetComponent<Selectable>().Value = 0;
        _topPos[index].GetComponent<Selectable>().Suit = null;
    }

    public void ChangeTopPosValue(int index, int value)
    {
        _topPos[index].GetComponent<Selectable>().Value = value;
    }

    public void RemoveCardFromBottom(int index, string cardName)
    {
        _bottoms[index].Remove(cardName);
    }

    public void AddCardToTopPos(int index, int value, string suit)
    {
        _topPos[index].GetComponent<Selectable>().Value = value;
        _topPos[index].GetComponent<Selectable>().Suit = suit;
    }

    public bool IsLastCardInDraw(string cardName)
    {
        return _tripsOnDisplay.Last() == cardName;
    }

    public bool IsCardBlocked(string cardName, int cardRow)
    {
        return _bottoms[cardRow].Last() == cardName;
    }
}
