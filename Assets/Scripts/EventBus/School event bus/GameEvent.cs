using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// As ScriptableObject that represents a game event, can be subscribed in Unity Inspector.
/// </summary>
[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{
    // List of all subscribers listening to this event
    [HideInInspector]
    public List<GameEventSubscriber> subscribers;

    // Information about the most recent publisher of the event
    [HideInInspector]
    public GameObject mostRecentPublisher;
    [HideInInspector]
    public string mostRecentPublisherName;
    [HideInInspector]
    public EventData mostRecentPublishedEventData;

    /// <summary>
    /// Registers a new subscriber to this event.
    /// Typically called from the subscriber's OnEnable or Start method.
    /// </summary>
    /// <param name="subscriber">The subscriber to add.</param>
    public void RegisterSubscriber(GameEventSubscriber subscriber)
    {
        subscribers.Add(subscriber);
    }

    /// <summary>
    /// Unregisters a subscriber from this event.
    /// Typically called from the subscriber's OnDisable or OnDestroy method.
    /// </summary>
    /// <param name="subscriber">The subscriber to remove.</param>
    public void UnregisterSubscriber(GameEventSubscriber subscriber)
    {
        subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Publishes the event to all registered subscribers.
    /// Also stores metadata about the publisher and event data for debugging or re-use.
    /// </summary>
    /// <param name="eventData">Data associated with the event.</param>
    /// <param name="publisher">The GameObject that published the event.</param>
    public void Publish(EventData eventData, GameObject publisher)
    {
        mostRecentPublisher = publisher;
        mostRecentPublisherName = publisher.name;
        mostRecentPublishedEventData = eventData;

        foreach (GameEventSubscriber subscriber in subscribers)
        {
            subscriber.OnEventPublished(eventData); // Notify each subscriber
        }
    }

    /// <summary>
    /// Unity callback called when the ScriptableObject is loaded.
    /// Clears previous event metadata to ensure a clean state.
    /// </summary>
    private void OnEnable()
    {
        mostRecentPublisher = null;
        mostRecentPublisherName = "";
        mostRecentPublishedEventData = null;
    }
}

[Serializable]
public abstract class EventData
{
    public string name;
}