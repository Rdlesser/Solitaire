using UnityEngine;

namespace Interfaces
{
    public interface IGameController
    {
        bool CanStack(GameObject selectedCard, GameObject targetCard);
        void StackCards(GameObject selectedCard, GameObject targetCard);
        bool CanMoveToFoundation(GameObject selectedCard);
        void MoveToFoundation(GameObject selectedCard);
        void DrawCard();
        void UndoLastMove();
        bool TryFlip(GameObject card);
    }
}