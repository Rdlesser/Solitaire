using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private Sprite[] _cardFaces;

    private List<string> _deck;
    private Dictionary<string, Sprite> _cardFaceDictionary = new();
    
    public static string[] Suits = new string[] { "C", "D", "H", "S" };
    public static string[] Values = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    private void Start()
    {
        _deck = GenerateDeck();
        CreateCardFaceDictionary();
    }
    
    public static List<string> GenerateDeck()
    {
        return Suits.SelectMany(suit => Values.Select(value => $"{suit}{value}")).ToList();
    }
    
    private void CreateCardFaceDictionary()
    {
        foreach (var cardFace in _cardFaces)
        {
            _cardFaceDictionary[cardFace.name] = cardFace;
        }
    }

    public Sprite GetCardFace(string cardName)
    {
        return _cardFaceDictionary.GetValueOrDefault(cardName);
    }
}