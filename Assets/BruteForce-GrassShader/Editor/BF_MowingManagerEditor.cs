using UnityEditor;
using UnityEngine;

public class BF_MowingManagerEditor : Editor
{
    [CustomEditor(typeof(BF_MowingManager))]
    private class MowingEditor : Editor
    {
        private bool isDebugShown;
        private SerializedProperty marginError;
        private SerializedProperty precisionValue;
        private GUIStyle style;
        private GUIStyle styleDebug;

        private void OnEnable()
        {
            precisionValue = serializedObject.FindProperty("precisionValue");
            marginError = serializedObject.FindProperty("marginError");
        }

        public override void OnInspectorGUI()
        {
            BF_MowingManager myTarget = (BF_MowingManager)target;
            EditorGUILayout.PropertyField(precisionValue);
            EditorGUILayout.PropertyField(marginError);

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.button);
                styleDebug = new GUIStyle(GUI.skin.button);
            }

            if (GUILayout.Button("Generate Markers", style))
            {
                myTarget.CreateMowingMarker(isDebugShown, precisionValue.intValue);
                style.normal.background = Texture2D.linearGrayTexture;
            }

            if (!isDebugShown)
            {
                if (GUILayout.Button("Show Debug", styleDebug))
                {
                    styleDebug.normal.background = Texture2D.whiteTexture;
                    isDebugShown = true;
                }
            }
            else
            {
                if (GUILayout.Button("Hide Debug", styleDebug))
                {
                    styleDebug.normal.background = Texture2D.linearGrayTexture;
                    isDebugShown = false;
                }
            }

            myTarget.changeDebugState(isDebugShown);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
