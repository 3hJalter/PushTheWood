using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.DesignPattern
{
    public abstract class Dispatcher<T> : Singleton<T> where T : HMonoBehaviour
    {
        private readonly Dictionary<EventID, Action> _listenerEventDictionary = new();

        public void RegisterListenerEvent(EventID eventID, Action callback)
        {
            if (_listenerEventDictionary.ContainsKey(eventID))
                _listenerEventDictionary[eventID] += callback;
            else
                _listenerEventDictionary.Add(eventID, callback);
        }

        public void UnregisterListenerEvent(EventID eventID, Action callback)
        {
            if (_listenerEventDictionary.ContainsKey(eventID))
                _listenerEventDictionary[eventID] -= callback;
            else
                Debug.LogWarning("EventID " + eventID + " not found");
        }

        public void PostEvent(EventID eventID)
        {
            if (_listenerEventDictionary.TryGetValue(eventID, out Action value))
                value.Invoke();
            else
                Debug.LogWarning("EventID " + eventID + " not found");
        }

        public void ClearAllListenerEvent()
        {
            _listenerEventDictionary.Clear();
        }
    }

    public enum EventID
    {
        Pause = 0,
        UnPause = 1,
        StartGame = 2,
        EndGame = 3,
    }
}
