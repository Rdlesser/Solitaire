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
        GameObject DrawCard(IMoveManager moveManager);
        bool IsCardBlocked(string cardName, int cardRow);
        void MoveCardBottom(Selectable selected, Selectable target);
        void MoveCardTop(Selectable selected, Selectable target);
        void TryAutoStack(GameObject selected);
    }

}
