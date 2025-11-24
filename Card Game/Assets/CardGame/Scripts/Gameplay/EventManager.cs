using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static readonly Dictionary<Type, Delegate> events = new();

    public static void AddListener<T>(Action<T> listener) where T : class
    {
        if (!events.TryGetValue(typeof(T), out Delegate del))
        {
            events[typeof(T)] = listener;
        }
        else
        {
            events[typeof(T)] = Delegate.Combine(del, listener);
        }
    }

    public static void AddListener(Action listener)
    {
        var type = typeof(GameEvent);
        if (!events.TryGetValue(type, out Delegate del))
            events[type] = listener;
        else
            events[type] = Delegate.Combine(del, listener);
    }

    public static void RemoveListener<T>(Action<T> listener) where T : class
    {
        if (events.TryGetValue(typeof(T), out Delegate del))
        {
            var newDel = Delegate.Remove(del, listener);
            if (newDel == null) events.Remove(typeof(T));
            else events[typeof(T)] = newDel;
        }
    }

    public static void RemoveListener(Action listener)
    {
        var type = typeof(GameEvent);
        if (events.TryGetValue(type, out Delegate del))
        {
            var newDel = Delegate.Remove(del, listener);
            if (newDel == null) events.Remove(type);
            else events[type] = newDel;
        }
    }

    public static void Trigger<T>(T eventData) where T : class
    {
        if (events.TryGetValue(typeof(T), out Delegate del))
            (del as Action<T>)?.Invoke(eventData);
    }

    public static void Trigger()
    {
        if (events.TryGetValue(typeof(GameEvent), out Delegate del))
            (del as Action)?.Invoke();
    }

    public static void ClearAll() => events.Clear();
}

public class GameEvent { }