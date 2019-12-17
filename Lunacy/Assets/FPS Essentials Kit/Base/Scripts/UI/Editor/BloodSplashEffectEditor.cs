using UnityEditor;

[CustomEditor(typeof(BloodSplashEffect))]
public class BloodSplashEffectEditor : Editor
{
    private static readonly string[] m_ScriptField = new string[] { "m_Script" };

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, m_ScriptField);
        serializedObject.ApplyModifiedProperties();
    }
}
