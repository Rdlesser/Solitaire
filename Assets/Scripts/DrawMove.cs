using System.Collections.Generic;
using Abstracts;
using UnityEngine;

public class DrawMove : Move
{
    private string _card;
    private List<string> _discardPile;

    public DrawMove(GameObject card, List<string> discardPile)
    {
        _card = card.name;
        _discardPile = discardPile;
    }

    public override void Undo()
    {
        _discardPile.Remove(_card);
        Debug.Log($"Undid draw for card {_card}");
    }
}