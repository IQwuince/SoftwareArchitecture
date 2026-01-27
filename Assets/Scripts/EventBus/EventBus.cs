using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> subs = new();

    public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type t = typeof(T);
        if(!subs.TryGetValue(t, out List<Delegate> list ))
        {
            list = new();
            subs[t] = list;
        }
        list.Add(handler);
    }
    public static void UnSubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type t = typeof(T);
        if (!subs.TryGetValue(t, out List<Delegate> list))
        {
            list = new();
            subs[t] = list;
        }
        list.Remove(handler);
    }
    public static void Publish<T>(T ev) where T : IGameEvent
    {
        Type t = typeof(T);
        if(subs.TryGetValue(t, out List<Delegate> list))
        {
            Delegate[] copy = list.ToArray();
            foreach (var d in copy)
            {
                ((Action<T>)d)(ev);
            }
        }
    }
}
