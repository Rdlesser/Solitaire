using Abstracts;

namespace Interfaces
{
    public interface IMoveManager
    {
        void RecordMove(Move move);
        void UndoLastMove();
    }
}