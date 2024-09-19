using System.Collections.Generic;

namespace Interfaces
{
    public interface ITracker
    {
        void TrackEvent(string eventName, Dictionary<string, string> eventParams);
    }
}