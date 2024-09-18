using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public static string[] Suits = new string[] { "C", "D", "H", "S" };
    public static string[] Values = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public List<string> _deck;
    
    // Start is called before the first frame update
    void Start()
    {
        PlayCards();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
