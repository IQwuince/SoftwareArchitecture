using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Add this component to GameObjects that needs to subscribe to event GameObjects.
/// When event is published, the method(s) in the onEventPublished list will be called.
/// </summary>
public class GameEventSubscriber : MonoBehaviour
{
    [SerializeField]
    private GameEvent gameEvent;
    [SerializeField]
    private UnityEvent<EventData> onEventPublished;

    private void OnEnable()
    {
        gameEvent.RegisterSubscriber(this);
    }

    private void OnDisable()
    {
        gameEvent.UnregisterSubscriber(this);
    }

    //This is not meant to be setup in Unity Inspector, to prevent accidently called by
    //the component itself, which will cause recursive calling, the second parameter is added
    //to prevent Unity from showing this method in the inspector
    public void OnEventPublished(EventData eventData, int hideMethodInInspector = 0)
    {
        onEventPublished?.Invoke(eventData);
    }
}
