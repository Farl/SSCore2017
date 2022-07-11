using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(SSContentSizeFitter), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the ContentSizeFitter Component.
    ///   Extend this class to write a custom editor for an ContentSizeFitter-derived component.
    /// </summary>
    public class SSContentSizeFitterEditor : SelfControllerEditor
    {
        SerializedProperty m_HorizontalFit;
        SerializedProperty m_VerticalFit;
        SerializedProperty m_HorizontalPreferredClamp;
        SerializedProperty m_VerticalPreferredClamp;

        protected virtual void OnEnable()
        {
            m_HorizontalFit = serializedObject.FindProperty("m_HorizontalFit");
            m_VerticalFit = serializedObject.FindProperty("m_VerticalFit");
            m_HorizontalPreferredClamp = serializedObject.FindProperty("m_HorizontalPreferredClamp");
            m_VerticalPreferredClamp = serializedObject.FindProperty("m_VerticalPreferredClamp");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_HorizontalFit, true);
            if (m_HorizontalFit.intValue == (int)SSContentSizeFitter.FitMode.PreferredClamp)
            {
                EditorGUILayout.PropertyField(m_HorizontalPreferredClamp, true);
            }
            EditorGUILayout.PropertyField(m_VerticalFit, true);
            if (m_VerticalFit.intValue == (int)SSContentSizeFitter.FitMode.PreferredClamp)
            {
                EditorGUILayout.PropertyField(m_VerticalPreferredClamp, true);
            }
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}