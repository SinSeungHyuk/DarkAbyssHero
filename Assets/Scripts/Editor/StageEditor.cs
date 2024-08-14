using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stage))]
public class StageEditor : IdentifiedObjectEditor
{
    private SerializedProperty stagePrefabProperty;
    private SerializedProperty monsterParametersProperty;


    protected override void OnEnable()
    {
        base.OnEnable();

        stagePrefabProperty = serializedObject.FindProperty("stagePrefab");
        monsterParametersProperty = serializedObject.FindProperty("monsterParameters");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update(); // ����ȭ ���� ������Ʈ ����


        EditorGUILayout.PropertyField(stagePrefabProperty);
        EditorGUILayout.PropertyField(monsterParametersProperty);


        serializedObject.ApplyModifiedProperties(); // ����� ���� ����
    }
}