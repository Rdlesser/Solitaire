using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{

    public interface IDeckManager
    {
        void GenerateDeck();
        void ShuffleDeck();
        IEnumerator DealCards();
        GameObject DrawCard();
        List<string> GetDiscardPile();
        bool IsCardBlocked(string cardName, int cardRow);
        void MoveCard(Selectable selected, int targetRow);
    }

}
