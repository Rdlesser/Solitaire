using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] private bool _isTop;
    [SerializeField] private int _row;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public int Value { get; private set; }
    public string Suit { get; private set; }
    public bool IsFaceUp { get; private set; }
    public bool IsInDeckPile { get;
        set; }
    public bool IsTop
    {
        get => _isTop;
        set => _isTop = value;
    }

    public int Row
    {
        get => _row;
        set => _row = value;
    }

    private string _valueString;
    private Dictionary<string, Sprite> _cardFaceDictionary;
    private Sprite _cardBackSprite;
    private string _cardName;
    
    public void Initialize(int value, string suit, bool isFaceUp, bool isInDeckPile, Dictionary<string, Sprite> cardFaceDictionary, Sprite cardBackSprite)
    {
        Value = value;
        Suit = suit;
        IsFaceUp = isFaceUp;
        IsInDeckPile = isInDeckPile;
        _cardFaceDictionary = cardFaceDictionary;
        _cardBackSprite = cardBackSprite;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _cardName = Suit + ValueToString();
        UpdateSprite();
    }
    
    public void SetRow(int row)
    {
        Row = row;
    }
    
    public void FlipCard(bool faceUp)
    {
        IsFaceUp = faceUp;
        UpdateSprite();
    }
    
    private void UpdateSprite()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = IsFaceUp ? GetCardFaceSprite() : GetCardBackSprite();
    }
    
    private Sprite GetCardFaceSprite()
    {
        if (_cardFaceDictionary.TryGetValue(_cardName, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogError($"Sprite not found for card {_cardName}");
        return null;
    }
    
    private Sprite GetCardBackSprite()
    {
        return _cardBackSprite;
    }
    
    private string ValueToString()
    {
        switch (Value)
        {
            case 1:
                return "A";
            case 11:
                return "J";
            case 12:
                return "Q";
            case 13:
                return "K";
            default:
                return Value.ToString();
        }
    }
    
    // Method to highlight the card (e.g., when selected)
    public void HighlightCard(bool highlight)
    {
        _spriteRenderer.color = highlight ? Color.yellow : Color.white; 
    }
    
    // // Start is called before the first frame update
    // void Start()
    // {
    //     if (CompareTag(Tags.CARD))
    //     {
    //         Suit = transform.name[0].ToString();
    //
    //         for (int i = 1; i < transform.name.Length; i++)
    //         {
    //             var character = transform.name[i];
    //             _valueString += character;
    //         }
    //
    //         Value = _valueString switch
    //         {
    //             "A" => 1,
    //             "2" => 2,
    //             "3" => 3,
    //             "4" => 4,
    //             "5" => 5,
    //             "6" => 6,
    //             "7" => 7,
    //             "8" => 8,
    //             "9" => 9,
    //             "10" => 10,
    //             "J" => 11,
    //             "Q" => 12,
    //             "K" => 13,
    //             _ => throw new ArgumentOutOfRangeException()
    //         };
    //     }
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
