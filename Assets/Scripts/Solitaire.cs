using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class Solitaire : MonoBehaviour
{

    [SerializeField] private Sprite[] _cardFaces;
    [SerializeField] private GameObject _cardPrefab; 
    
    public static string[] Suits = new string[] { "C", "D", "H", "S" };
    public static string[] Values = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public List<string> _deck;

    private Dictionary<string, Sprite> _cardFaceDictionary = new();

    private void OnEnable()
    {
        CreateCardFaceDictionary();
    }

    // Start is called before the first frame update

    void Start()
    {
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
        
        SolitaireDeal();
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

    private void SolitaireDeal()
    {
        var yOffset = 0f;
        var zOffset = 0.03f;
        
        foreach (var card in _deck)
        {
            var newCard = Instantiate(_cardPrefab, new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z - zOffset), Quaternion.identity);
            newCard.name = card;
            newCard.GetComponent<Selectable>().IsFaceUp = true;
            newCard.GetComponent<UpdateSprite>().InjectSolitaire(this);

            yOffset += 0.3f;
            zOffset += 0.03f;
        }
    }
}
