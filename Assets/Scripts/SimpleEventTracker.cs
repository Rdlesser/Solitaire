using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

// This is a simplistic event tracker to be switched by an appropriate tracker (GameAnalytics and such)
public class SimpleEventTracker : ITracker
{
    public void TrackEvent(string eventName, Dictionary<string, string> eventParams)
    {
        Debug.Log($"Event: {eventName}, Params: {string.Join(", ", eventParams.Select(kv => kv.Key + "=" + kv.Value))}");
    }
}