using System.Collections.Generic;
using Abstracts;

public class MoveManager
{
    private Stack<Move> _moves = new();

    public void RecordMove(Move move)
    {
        _moves.Push(move);
    }

    public void UndoLastMove()
    {
        if (_moves.Count <= 0)
        {
            return;
        }

        Move lastMove = _moves.Pop();
        lastMove.Undo();
    }
}