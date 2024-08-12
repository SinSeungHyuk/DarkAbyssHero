using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stat))]
public class StatEditor : IdentifiedObjectEditor
{
    private SerializedProperty isUseMaxValueProperty;
    private SerializedProperty maxValueProperty;
    private SerializedProperty defaultValueProperty;
    private SerializedProperty valuePerLevelProperty;
    private SerializedProperty goldPerLevelProperty;
    private SerializedProperty defaultGoldProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        isUseMaxValueProperty = serializedObject.FindProperty("isUseMaxValue");
        maxValueProperty = serializedObject.FindProperty("maxValue");
        defaultValueProperty = serializedObject.FindProperty("defaultValue");
        valuePerLevelProperty = serializedObject.FindProperty("valuePerLevel");
        goldPerLevelProperty = serializedObject.FindProperty("goldPerLevel");
        defaultGoldProperty = serializedObject.FindProperty("defaultGold");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update(); // 직렬화 변수 업데이트 시작

        if (DrawFoldoutTitle("Setting"))
        {
            EditorGUILayout.PropertyField(isUseMaxValueProperty);
            EditorGUILayout.PropertyField(maxValueProperty);
            EditorGUILayout.PropertyField(defaultValueProperty);
            EditorGUILayout.PropertyField(valuePerLevelProperty);
            EditorGUILayout.PropertyField(goldPerLevelProperty);
            EditorGUILayout.PropertyField(defaultGoldProperty);

            serializedObject.ApplyModifiedProperties(); // 변경된 내용 저장
        }
    }
}