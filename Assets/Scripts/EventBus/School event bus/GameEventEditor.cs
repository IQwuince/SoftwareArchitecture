using UnityEngine;
using UnityEditor;

/// <summary>
/// An editor script to display event information in Unity Inspector
/// </summary>
[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameEvent gameEvent = (GameEvent)target;
        EditorGUILayout.LabelField("Subscribers");

        if (Application.isPlaying)
        {
            foreach (GameEventSubscriber subscriber in gameEvent.subscribers)
            {
                if (subscriber != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.ObjectField(subscriber.name, subscriber, typeof(GameObject), false);
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.LabelField("Most recent publisher");
            if (gameEvent.mostRecentPublisher != null)
            {
                EditorGUILayout.ObjectField(gameEvent.mostRecentPublisher.name, gameEvent.mostRecentPublisher, typeof(GameObject), false);
            }
            else
            {
                GUIStyle style = new GUIStyle(EditorStyles.textArea);
                style.wordWrap = true;
                //If there's a most recent published event but the publisher is null,
                //the publisher has already been destroyed
                if (gameEvent.mostRecentPublishedEventData != null)
                    EditorGUILayout.TextArea("GameObject " + gameEvent.mostRecentPublisherName
                        + " published this event, it was already destroyed", style);
                else
                    EditorGUILayout.TextArea("None");
            }

            EditorGUILayout.LabelField("Most recent published event data");
            if (gameEvent.mostRecentPublishedEventData != null)
            {
                EditorGUILayout.TextArea(gameEvent.mostRecentPublishedEventData.ToString());
            }
            else
            {
                EditorGUILayout.TextArea("None");
            }
        }
    }
    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }
}
