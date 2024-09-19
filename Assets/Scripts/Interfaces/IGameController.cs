using UnityEngine;

namespace Interfaces
{
    public interface IGameController
    {
        bool CanStackBottom(GameObject selectedCard, GameObject targetCard);
        bool CanStackTop(GameObject selectedCard, GameObject targetCard);
        void StackCardsInBottom(GameObject selectedCard, GameObject targetCard);
        void StackCardsInTop(GameObject selectedCard, GameObject targetCard);
        void DrawCard();
        void UndoLastMove();
        bool TryFlip(GameObject card);
        void AutoStack(GameObject card);
    }
}