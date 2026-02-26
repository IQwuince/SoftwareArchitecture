using System;

/// <summary>
/// This is a simple implementation of the event bus pattern
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void Publish(T pEvent)
    {
        OnEvent?.Invoke(pEvent);
    }
}

/// <summary>
/// Base class for events, for this simple demonstration it is empty, more fields
/// could be added in real projects, e.g. eventData
/// </summary>
public abstract class Event { }



