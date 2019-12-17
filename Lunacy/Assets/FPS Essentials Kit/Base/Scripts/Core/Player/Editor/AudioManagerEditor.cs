using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private static readonly string[] m_ScriptField = new string[] { "m_Script" };

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();
        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, m_ScriptField);
        serializedObject.ApplyModifiedProperties();
    }
}