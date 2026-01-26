using UnityEditor;
using UnityEngine;

namespace IQwuince.Quests.Editor
{
    [CustomEditor(typeof(QuestSO), true)]
    public class QuestSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            QuestSO quest = (QuestSO)target;

            // Show validation status
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

            if (!quest.Validate(out string error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Quest data is valid", MessageType.Info);
            }
        }
    }
}
