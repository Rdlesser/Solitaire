using System.Collections.Generic;
using Abstracts;
using UnityEngine;

namespace Moves
{
    public class CardMove : Move
    {
        private Selectable _card;
        private Transform _originalParent;
        private Vector3 _localPosition;
        private List<string> _discardPile;
        private bool _wasInDeckPile;
        private int _originalRow;
        private List<string>[] _bottoms;
        private bool _wasTop;
        private bool _wasContainedInBottoms;

        public CardMove(List<string> discardPile, Selectable card, List<string>[] bottoms)
        {
            _card = card;
            _originalParent = card.transform.parent;
            _localPosition = card.transform.localPosition;
            _discardPile = discardPile;
            _wasInDeckPile = card.IsInDeckPile;
            _originalRow = card.Row;
            _bottoms = bottoms;
            _wasTop = card.IsTop;
            _wasContainedInBottoms = _bottoms[_card.Row].Contains(_card.name);
        }

        public override void Undo()
        {
            if (_wasInDeckPile)
            {
                _card.IsInDeckPile = true;
                _discardPile.Add(_card.gameObject.name); 
            }
        
            _card.transform.SetParent(_originalParent);
            _card.transform.localPosition = _localPosition;
            _card.SetRow(_originalRow);
            _card.IsTop = _wasTop;
        
            if (!_wasInDeckPile && !_wasTop && _wasContainedInBottoms)
            {
                _bottoms[_card.Row].Add(_card.name);
            }
        
            Debug.Log($"Undid move for card {_card.name}");
        }
    }
}