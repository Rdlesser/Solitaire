using System.Collections.Generic;

namespace Interfaces
{

    public interface IDeckManager
    {
        List<string> GenerateDeck();
        void ShuffleDeck();
        void DealCards();
    }

}
