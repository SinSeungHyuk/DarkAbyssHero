using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stat))]
public class StatEditor : IdentifiedObjectEditor
{
    private SerializedProperty defaultValueProperty;
    private SerializedProperty valuePerLevelProperty;
    private SerializedProperty goldPerLevelProperty;
    private SerializedProperty defaultGoldProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        defaultValueProperty = serializedObject.FindProperty("defaultValue");
        valuePerLevelProperty = serializedObject.FindProperty("valuePerLevel");
        goldPerLevelProperty = serializedObject.FindProperty("goldPerLevel");
        defaultGoldProperty = serializedObject.FindProperty("defaultGold");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update(); // ����ȭ ���� ������Ʈ ����

        if (DrawFoldoutTitle("Setting"))
        {
            EditorGUILayout.PropertyField(defaultValueProperty);
            EditorGUILayout.PropertyField(valuePerLevelProperty);
            EditorGUILayout.PropertyField(goldPerLevelProperty);
            EditorGUILayout.PropertyField(defaultGoldProperty);

            serializedObject.ApplyModifiedProperties(); // ����� ���� ����
        }
    }
}