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
    [SerializeField] private GameObject[] _bottomPos;
    [SerializeField] private GameObject[] _topPos;
    [SerializeField] private float _cardInitialYOffset = 0f;
    [SerializeField] private float _cardYOffsetIncrement = 0.3f;
    [SerializeField] private float _cardInitialZOffset = 0.03f;
    [SerializeField] private float _cardZOffsetIncrement = 0.03f;
    
    public static string[] Suits = new string[] { "C", "D", "H", "S" };
    public static string[] Values = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public List<string>[] _bottoms;
    public List<string>[] _tops;

    private List<string> _bottom0 = new();
    private List<string> _bottom1 = new();
    private List<string> _bottom2 = new();
    private List<string> _bottom3 = new();
    private List<string> _bottom4 = new();
    private List<string> _bottom5 = new();
    private List<string> _bottom6 = new();
    

    public List<string> _deck;

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

                if (card == _bottoms[i][_bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().IsFaceUp = true;
                }
                
                newCard.GetComponent<UpdateSprite>().InjectSolitaire(this);

                yOffset += _cardYOffsetIncrement;
                zOffset += _cardZOffsetIncrement;
            }
        }
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
}
