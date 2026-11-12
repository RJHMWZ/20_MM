using System;
using System.Collections.Generic;
using CodeKits.Singleton;

namespace CodeKits.Event
{
    public class EasyEvents
    {
        private static readonly EasyEvents _events = new();
        private readonly Dictionary<Type, IEasyEvent> _typeEvents = new();

        public static void Register<T>() where T : IEasyEvent, new()
        {
            _events.AddEvent<T>();
        }

        public static T Get<T>() where T : IEasyEvent
        {
            return _events.GetEvent<T>();
        }

        private void AddEvent<T>() where T : IEasyEvent, new()
        {
            _typeEvents.Add(typeof(T), new T());
        }

        private T GetEvent<T>() where T : IEasyEvent
        {
            return _typeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }
    }
}