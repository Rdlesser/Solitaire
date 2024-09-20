using System.Collections.Generic;
using Abstracts;
using UnityEngine;

namespace Moves
{
    public class DrawMove : Move
    {
        private Selectable _card;
        private List<Selectable> _drawnCards;
        private List<string> _discardPile;
        private List<string> _deck;

        public DrawMove(Selectable card, List<Selectable> drawnCards, List<string> discardPile, List<string> deck)
        {
            _card = card;
            _drawnCards = drawnCards;
            _discardPile = discardPile;
            _deck = deck;
        }

        public override void Undo()
        {
            _drawnCards.Remove(_card);
            _discardPile.Remove(_card.name);
            // TODO: Use object pooling
            Object.Destroy(_card.gameObject);
            _deck.Insert(0, _card.name);
            Debug.Log($"Undid draw for card {_card}");
        }
    }
}