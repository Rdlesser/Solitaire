using Abstracts;

namespace Moves
{
    public class FlipMove : Move
    {
        private Selectable _selectable;
    
        public FlipMove(Selectable selectable)
        {
            _selectable = selectable;
        }

        public override void Undo()
        {
            _selectable.FlipCard(false);
        }
    }
}