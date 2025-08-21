// ===== 인스펙터 읽기전용 표시용(선택) =====
using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)] public class ReadOnlyAttribute : PropertyAttribute { }
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; EditorGUI.PropertyField(position, property, label, true); GUI.enabled = true;
    }
}
#endif